//
// - HtmlTreeBuilderState.InColumnGroupState.cs -
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

    abstract partial class HtmlTreeBuilderState {

        class InColumnGroupState : HtmlTreeBuilderState {

            public override bool Process(Token t, HtmlTreeBuilder tb) {
                if (IsWhitespace(t)) {
                    tb.Insert(t.AsCharacter());
                    return true;
                }

                switch (t.Type) {
                    case TokenType.Comment:
                        tb.Insert(t.AsComment());
                        break;

                    case TokenType.Doctype:
                        tb.Error(this);
                        break;

                    case TokenType.StartTag:
                        Token.StartTag startTag = t.AsStartTag();
                        string name = startTag.Name;

                        if (name.Equals("html"))
                            return tb.Process(t, InBody);

                        else if (name.Equals("col"))
                            tb.InsertEmpty(startTag);

                        else
                            return AnythingElse(t, tb);
                        break;

                    case TokenType.EndTag:
                        Token.EndTag endTag = t.AsEndTag();
                        name = endTag.Name;

                        if (name.Equals("colgroup")) {
                            if (tb.CurrentElement.NodeName.Equals("html")) { // frag case
                                tb.Error(this);
                                return false;
                            } else {
                                tb.Pop();
                                tb.Transition(InTable);
                            }

                        } else
                            return AnythingElse(t, tb);
                        break;

                    case TokenType.EOF:
                        if (tb.CurrentElement.NodeName.Equals("html"))
                            return true; // stop parsing; frag case
                        else
                            return AnythingElse(t, tb);

                    default:
                        return AnythingElse(t, tb);
                }

                return true;
            }

            private bool AnythingElse(Token t, TreeBuilder tb) {
                bool processed = tb.Process(new Token.EndTag("colgroup"));
                if (processed) // only ignored in frag case
                    return tb.Process(t);

                return true;
            }
        }
    }
}
