//
// - Tokeniser.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

// The MIT License
//
// Copyright (c) 2009, 2010, 2011, 2012 Jonathan Hedley <jonathan@hedley.net>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Carbonfrost.Commons.Html.Parser {

    class Tokeniser {

        public const char replacementChar = '\uFFFD'; // replaces null char
        public const string replacementStr = "\uFFFD"; // replaces null char

        private CharacterReader reader; // html input
        private HtmlParseErrorCollection errors; // errors found while tokenising

        public TokeniserState state = TokeniserState.Data; // current tokenisation state
        private Token emitPending; // the token we are about to emit on next read
        private bool isEmitPending = false;
        private StringBuilder charBuffer = new StringBuilder(); // buffers characters to output as one token
        internal StringBuilder dataBuffer; // buffers data looking for </script>

        internal Token.Tag tagPending; // tag we are building up
        internal Token.Doctype doctypePending; // doctype building up
        internal Token.Comment commentPending; // comment building up
        private Token.StartTag lastStartTag; // the last start tag emitted, to test appropriate end tag
        private bool selfClosingFlagAcknowledged = true;

        public TokeniserState State {
            get {
                return state;
            }
        }

        public Tokeniser(CharacterReader reader, HtmlParseErrorCollection errors) {
            this.reader = reader;
            this.errors = errors;
        }

        public Token Read() {
            if (!selfClosingFlagAcknowledged) {
                ParseError.SelfClosingTagNotAcknowledged(this);
                selfClosingFlagAcknowledged = true;
            }

            while (!isEmitPending)
                state.Read(this, reader);

            // if emit is pending, a non-char token was found: return any chars in buffer, and leave token for next read:
            if (charBuffer.Length > 0) {
                string str = charBuffer.ToString();
                charBuffer.Remove(0, charBuffer.Length);
                return new Token.Character(str);

            } else {
                isEmitPending = false;
                return emitPending;
            }
        }

        public void Emit(Token token) {
            if (isEmitPending)
                HtmlWarning.UnreadTokenPending();

            emitPending = token;
            isEmitPending = true;
            if (token.Type == TokenType.StartTag) {
                Token.StartTag startTag = (Token.StartTag) token;
                lastStartTag = startTag;

                if (startTag.selfClosing)
                    selfClosingFlagAcknowledged = false;

            } else if (token.Type == TokenType.EndTag) {
                Token.EndTag endTag = (Token.EndTag) token;

                if (endTag.Attributes.Count > 0)
                    ParseError.AttributesPresentOnEndTagError(this);
            }
        }

        public void Emit(string str) {
            // buffer strings up until last string token found, to emit only one token for a run of char refs etc.
            // does not set isEmitPending; read checks that
            charBuffer.Append(str);
        }

        public void Emit(char c) {
            charBuffer.Append(c);
        }

        public void Transition(TokeniserState state) {
            this.state = state;
        }

        public void AdvanceTransition(TokeniserState state) {
            reader.Advance();
            this.state = state;
        }

        public void AcknowledgeSelfClosingFlag() {
            selfClosingFlagAcknowledged = true;
        }

        public string ConsumeCharacterReference(char? additionalAllowedCharacter, bool inAttribute) {
            if (reader.IsEmpty)
                return null;

            if (additionalAllowedCharacter.HasValue && additionalAllowedCharacter.Value == reader.Current)
                return null;

            if (reader.MatchesAny('\t', '\n', '\f', ' ', '<', '&'))
                return null;

            reader.Mark();
            if (reader.MatchConsume("#")) { // numbered
                bool isHexMode = reader.MatchConsumeIgnoreCase("X");
                string numRef = isHexMode ? reader.ConsumeHexSequence() : reader.ConsumeDigitSequence();

                if (numRef.Length == 0) { // didn't match anything
                    ParseError.NumericReferenceWithNoNumerals(this);
                    reader.RewindToMark();
                    return null;
                }

                if (!reader.MatchConsume(";"))
                    ParseError.MissingSemicolon(this);

                int charval = -1;
                try {
                    int base2 = isHexMode ? 16 : 10;
                    charval = Convert.ToInt32(numRef, base2);

                } catch (FormatException) {
                } // skip

                if (charval == -1 || (charval >= 0xD800 && charval <= 0xDFFF) || charval > 0x10FFFF) {
                    ParseError.CharOutsideRange(this);
                    return replacementStr;

                } else {
                    // TODO: implement number replacement table
                    // TODO: check for extra illegal unicode points as parse errors
                    return Char.ConvertFromUtf32(charval);
                }

            } else { // named
                // get as many letters as possible, and look for matching entities. unconsume backwards till a match is found
                string nameRef = reader.ConsumeLetterThenDigitSequence();
                string origNameRef = nameRef; // for error reporting. nameRef gets chomped looking for matches

                bool looksLegit = reader.Matches(';');
                bool found = false;
                while (nameRef.Length > 0 && !found) {

                    if (HtmlEncoder.IsNamedEntity(nameRef))
                        found = true;
                    else {
                        nameRef = nameRef.Substring(0, nameRef.Length - 1);
                        reader.Unconsume();
                    }
                }

                if (!found) {
                    if (looksLegit) // named with semicolon
                        ParseError.InvalidNamedReference(this, origNameRef);

                    reader.RewindToMark();
                    return null;
                }

                if (inAttribute && (reader.MatchesLetter() || reader.MatchesDigit() || reader.MatchesAny('=', '-', '_'))) {
                    // don't want that to match
                    reader.RewindToMark();
                    return null;
                }

                if (!reader.MatchConsume(";"))
                    ParseError.MissingSemicolon(this);

                return HtmlEncoder.GetCharacterByName(nameRef);
            }
        }

        public Token.Tag CreateTagPending(bool start) {
            tagPending = start ? ((Token.Tag) new Token.StartTag()) : new Token.EndTag();
            return tagPending;
        }

        public void EmitTagPending() {
            tagPending.FinaliseTag();
            Emit(tagPending);
        }

        public void CreateCommentPending() {
            commentPending = new Token.Comment();
        }

        public void EmitCommentPending() {
            Emit(commentPending);
        }

        public void CreateDoctypePending() {
            doctypePending = new Token.Doctype();
        }

        public void EmitDoctypePending() {
            Emit(doctypePending);
        }

        public void CreateTempBuffer() {
            dataBuffer = new StringBuilder();
        }

        public bool IsAppropriateEndTagToken() {
            return tagPending.Name.Equals(lastStartTag.Name);
        }

        public string AppropriateEndTagName() {
            return lastStartTag.Name;
        }

        public void Error(string errorMsg) {
            errors.Add(reader.Position, errorMsg);
        }

        public void Error(TokeniserState state) {
            ParseError.UnexpectedChar(errors, reader, state);
        }

        public void EofError(TokeniserState state) {
            ParseError.UnexpectedlyReachedEof(errors, reader, state);
        }

        private bool CurrentNodeInHtmlNS() {
            // TODO: implememnt namespaces correctly
            return true;
            //    Element currentNode = currentNode();
            //    return currentNode != null && currentNode.@namespace().equals("HTML");
        }
    }
}

