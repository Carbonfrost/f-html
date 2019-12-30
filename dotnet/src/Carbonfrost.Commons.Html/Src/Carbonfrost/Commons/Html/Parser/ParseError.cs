//
// - ParseError.cs -
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

using System;
using System.Diagnostics;

namespace Carbonfrost.Commons.Html.Parser {

    static class ParseError {

        public static void AttributesPresentOnEndTagError(Tokeniser t) {
            t.Error("Attributes incorrectly present on end tag");
        }

        public static void UnexpectedToken(this HtmlParseErrorCollection errors, int readerPos, HtmlTreeBuilderState state, Token currentToken) {
            errors.Add(readerPos, "Unexpected token [{0}] when in state [{1}]", currentToken.TokenTypeName, state);
        }

        public static void SelfClosingTagNotAcknowledged(Tokeniser t) {
            t.Error("Self closing flag not acknowledged");
        }

        public static void NumericReferenceWithNoNumerals(Tokeniser t) {
            t.CharacterReferenceError("numeric reference with no numerals");
        }

        public static void MissingSemicolon(Tokeniser t) {
            t.CharacterReferenceError("missing semicolon"); // missing semi
        }

        public static void CharOutsideRange(Tokeniser t) {
            t.CharacterReferenceError("char outside of valid range");
        }

        public static void InvalidNamedReference(Tokeniser t, string origNameRef) {
            t.CharacterReferenceError(string.Format("invalid named referenece '{0}'", origNameRef));
        }

        public static void UnexpectedChar(HtmlParseErrorCollection errors, CharacterReader reader, TokeniserState state) {
            errors.Add(reader.Position, "Unexpected char '{0}' in input state [{1}]", reader.Current, state);
        }

        public static void UnexpectedlyReachedEof(HtmlParseErrorCollection errors, CharacterReader reader, TokeniserState state) {
            errors.Add(reader.Position, "Unexpectedly reached end of file (EOF) in input state [{0}]", state);
        }

        static void CharacterReferenceError(this Tokeniser t, string message) {
            t.Error(string.Format("Invalid char reference: {0}", message));
        }
    }
}

