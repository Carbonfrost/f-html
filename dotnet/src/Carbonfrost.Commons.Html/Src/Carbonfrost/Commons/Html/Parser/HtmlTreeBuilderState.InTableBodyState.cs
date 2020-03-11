//
// - HtmlTreeBuilderState.InTableBodyState.cs -
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
using System.Linq;

namespace Carbonfrost.Commons.Html.Parser {

    abstract partial class HtmlTreeBuilderState {

        class InTableBodyState : HtmlTreeBuilderState {

            public override bool Process(Token t, HtmlTreeBuilder tb) {
                switch (t.Type) {
                    case TokenType.StartTag:
                        Token.StartTag startTag = t.AsStartTag();
                        string name = startTag.Name;

                        if (name.Equals("tr")) {
                            tb.ClearStackToTableBodyContext();
                            tb.Insert(startTag);
                            tb.Transition(InRow);

                        } else if (StringUtil.In(name, "th", "td")) {
                            tb.Error(this);
                            tb.Process(new Token.StartTag("tr"));
                            return tb.Process(startTag);

                        } else if (StringSet.Create("caption col colgroup tbody tfoot thead").Contains(name)) {
                            return ExitTableBody(t, tb);

                        } else
                            return AnythingElse(t, tb);

                        break;

                    case TokenType.EndTag:
                        Token.EndTag endTag = t.AsEndTag();
                        name = endTag.Name;

                        if (StringUtil.In(name, "tbody", "tfoot", "thead")) {
                            if (!tb.InTableScope(name)) {
                                tb.Error(this);
                                return false;
                            } else {
                                tb.ClearStackToTableBodyContext();
                                tb.Pop();
                                tb.Transition(InTable);
                            }

                        } else if (name.Equals("table")) {
                            return ExitTableBody(t, tb);

                        } else if (StringSet.Create("body caption col colgroup html td th tr").Contains(name)) {
                            tb.Error(this);
                            return false;

                        } else
                            return AnythingElse(t, tb);

                        break;

                    default:
                        return AnythingElse(t, tb);
                }
                return true;
            }

            private bool ExitTableBody(Token t, HtmlTreeBuilder tb) {
                if (!(tb.InTableScope("tbody")
                      || tb.InTableScope("thead")
                      || tb.InScope("tfoot"))) {
                    // frag case
                    tb.Error(this);
                    return false;
                }

                tb.ClearStackToTableBodyContext();
                tb.Process(new Token.EndTag(tb.CurrentElement.NodeName)); // tbody, tfoot, thead
                return tb.Process(t);
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                return tb.Process(t, InTable);
            }
        }
    }
}
