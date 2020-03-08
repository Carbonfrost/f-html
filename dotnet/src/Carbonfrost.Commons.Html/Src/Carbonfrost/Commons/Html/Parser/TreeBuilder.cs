//
// Copyright 2012, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
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
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html.Parser {

    abstract class TreeBuilder {

        protected CharacterReader reader;
        internal Tokeniser tokeniser;
        protected HtmlDocument doc; // current doc we are building into
        protected DescendableLinkedList<DomContainer> stack; // the stack of open elements
        protected Uri baseUri; // current base uri, for creating new elements
        protected Token currentToken; // currentToken is used only for error tracking.
        protected HtmlParseErrorCollection errors; // null when not tracking errors

        public DomContainer CurrentElement {
            get {
                return stack.Last.Value;
            }
        }

        internal HtmlSchema TagLibrary {
            get {
                return HtmlSchema.Html5;
            }
        }

        protected virtual void InitialiseParse(string input, Uri baseUri, HtmlParseErrorCollection errors) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }

            this.doc = new HtmlDocument(baseUri);
            this.reader = new CharacterReader(input);
            this.errors = errors;
            this.tokeniser = new Tokeniser(reader, errors);
            this.stack = new DescendableLinkedList<DomContainer>();
            this.baseUri = baseUri;
        }

        public virtual HtmlDocument Parse(string input, Uri baseUri, HtmlParseErrorCollection errors) {
            InitialiseParse(input, baseUri, errors);
            RunParser();
            return doc;
        }

        protected void RunParser() {
            Token token = null;

            do {
                token = tokeniser.Read();
                Process(token);

            } while (token.Type != TokenType.EOF);
        }

        public abstract bool Process(Token token);

    }
}
