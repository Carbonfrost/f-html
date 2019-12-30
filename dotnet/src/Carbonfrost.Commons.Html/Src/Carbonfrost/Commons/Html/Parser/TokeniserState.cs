//
// - TokeniserState.cs -
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

namespace Carbonfrost.Commons.Html.Parser {

    internal abstract partial class TokeniserState {

        public abstract void Read(Tokeniser t, CharacterReader r);

        private const char nullChar = '\u0000';
        private const char replacementChar = Tokeniser.replacementChar;
        private const string replacementStr = Tokeniser.replacementStr;
        private const char eof = CharacterReader.EOF;

        public static readonly TokeniserState Data = new DataState();
        public static readonly TokeniserState CharacterReferenceInData = new CharacterReferenceInDataState();
        public static readonly TokeniserState Rcdata = new RcdataState();
        public static readonly TokeniserState CharacterReferenceInRcdata = new CharacterReferenceInRcdataState();
        public static readonly TokeniserState Rawtext = new RawtextState();
        public static readonly TokeniserState ScriptData = new ScriptDataState();
        public static readonly TokeniserState PLAINTEXT = new PLAINTEXTState();
        public static readonly TokeniserState TagOpen = new TagOpenState();
        public static readonly TokeniserState EndTagOpen = new EndTagOpenState();
        public static readonly TokeniserState TagName = new TagNameState();
        public static readonly TokeniserState RcdataLessthanSign = new RcdataLessthanSignState();
        public static readonly TokeniserState RCDATAEndTagOpen = new RCDATAEndTagOpenState();
        public static readonly TokeniserState RCDATAEndTagName = new RCDATAEndTagNameState();
        public static readonly TokeniserState RawtextLessthanSign = new RawtextLessthanSignState();
        public static readonly TokeniserState RawtextEndTagOpen = new RawtextEndTagOpenState();
        public static readonly TokeniserState RawtextEndTagName = new RawtextEndTagNameState();
        public static readonly TokeniserState ScriptDataLessthanSign = new ScriptDataLessthanSignState();
        public static readonly TokeniserState ScriptDataEndTagOpen = new ScriptDataEndTagOpenState();
        public static readonly TokeniserState ScriptDataEndTagName = new ScriptDataEndTagNameState();
        public static readonly TokeniserState ScriptDataEscapeStart = new ScriptDataEscapeStartState();
        public static readonly TokeniserState ScriptDataEscapeStartDash = new ScriptDataEscapeStartDashState();
        public static readonly TokeniserState ScriptDataEscaped = new ScriptDataEscapedState();
        public static readonly TokeniserState ScriptDataEscapedDash = new ScriptDataEscapedDashState();
        public static readonly TokeniserState ScriptDataEscapedDashDash = new ScriptDataEscapedDashDashState();
        public static readonly TokeniserState ScriptDataEscapedLessThanSign = new ScriptDataEscapedLessthanSignState();
        public static readonly TokeniserState ScriptDataEscapedEndTagOpen = new ScriptDataEscapedEndTagOpenState();
        public static readonly TokeniserState ScriptDataEscapedEndTagName = new ScriptDataEscapedEndTagNameState();
        public static readonly TokeniserState ScriptDataDoubleEscapeStart = new ScriptDataDoubleEscapeStartState();
        public static readonly TokeniserState ScriptDataDoubleEscaped = new ScriptDataDoubleEscapedState();
        public static readonly TokeniserState ScriptDataDoubleEscapedDash = new ScriptDataDoubleEscapedDashState();
        public static readonly TokeniserState ScriptDataDoubleEscapedDashDash = new ScriptDataDoubleEscapedDashDashState();
        public static readonly TokeniserState ScriptDataDoubleEscapedLessthanSign = new ScriptDataDoubleEscapedLessthanSignState();
        public static readonly TokeniserState ScriptDataDoubleEscapeEnd = new ScriptDataDoubleEscapeEndState();
        public static readonly TokeniserState BeforeAttributeName = new BeforeAttributeNameState();
        public static readonly TokeniserState AttributeName = new AttributeNameState();
        public static readonly TokeniserState AfterAttributeName = new AfterAttributeNameState();
        public static readonly TokeniserState BeforeAttributeValue = new BeforeAttributeValueState();
        public static readonly TokeniserState AttributeValue_doubleQuoted = new AttributeValue_doubleQuotedState();
        public static readonly TokeniserState AttributeValue_singleQuoted = new AttributeValue_singleQuotedState();
        public static readonly TokeniserState AttributeValue_unquoted = new AttributeValue_unquotedState();
        public static readonly TokeniserState AfterAttributeValue_quoted = new AfterAttributeValue_quotedState();
        public static readonly TokeniserState SelfClosingStartTag = new SelfClosingStartTagState();
        public static readonly TokeniserState BogusComment = new BogusCommentState();
        public static readonly TokeniserState MarkupDeclarationOpen = new MarkupDeclarationOpenState();
        public static readonly TokeniserState CommentStart = new CommentStartState();
        public static readonly TokeniserState CommentStartDash = new CommentStartDashState();
        public static readonly TokeniserState Comment = new CommentState();
        public static readonly TokeniserState CommentEndDash = new CommentEndDashState();
        public static readonly TokeniserState CommentEnd = new CommentEndState();
        public static readonly TokeniserState CommentEndBang = new CommentEndBangState();
        public static readonly TokeniserState Doctype = new DoctypeState();
        public static readonly TokeniserState BeforeDoctypeName = new BeforeDoctypeNameState();
        public static readonly TokeniserState DoctypeName = new DoctypeNameState();
        public static readonly TokeniserState AfterDoctypeName = new AfterDoctypeNameState();
        public static readonly TokeniserState AfterDoctypePublicKeyword = new AfterDoctypePublicKeywordState();
        public static readonly TokeniserState BeforeDoctypePublicIdentifier = new BeforeDoctypePublicIdentifierState();
        public static readonly TokeniserState DoctypePublicIdentifier_doubleQuoted = new DoctypePublicIdentifier_doubleQuotedState();
        public static readonly TokeniserState DoctypePublicIdentifier_singleQuoted = new DoctypePublicIdentifier_singleQuotedState();
        public static readonly TokeniserState AfterDoctypePublicIdentifier = new AfterDoctypePublicIdentifierState();
        public static readonly TokeniserState BetweenDoctypePublicAndSystemIdentifiers = new BetweenDoctypePublicAndSystemIdentifiersState();
        public static readonly TokeniserState AfterDoctypeSystemKeyword = new AfterDoctypeSystemKeywordState();
        public static readonly TokeniserState BeforeDoctypeSystemIdentifier = new BeforeDoctypeSystemIdentifierState();
        public static readonly TokeniserState DoctypeSystemIdentifier_doubleQuoted = new DoctypeSystemIdentifier_doubleQuotedState();
        public static readonly TokeniserState DoctypeSystemIdentifier_singleQuoted = new DoctypeSystemIdentifier_singleQuotedState();
        public static readonly TokeniserState AfterDoctypeSystemIdentifier = new AfterDoctypeSystemIdentifierState();
        public static readonly TokeniserState BogusDoctype = new BogusDoctypeState();
        public static readonly TokeniserState CdataSection = new CdataSectionState();
    }
}
