//
// - HtmlTreeBuilderState.InTableState.cs -
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

namespace Carbonfrost.Commons.Html.Parser {

    abstract partial class HtmlTreeBuilderState {

        class InTableState : HtmlTreeBuilderState {

            public override bool Process(Token t, HtmlTreeBuilder tb) {

                if (t.IsCharacter) {
                    tb.NewPendingTableCharacters();
                    tb.MarkInsertionMode();
                    tb.Transition(InTableText);
                    return tb.Process(t);

                } else if (t.IsComment) {
                    tb.Insert(t.AsComment());
                    return true;

                } else if (t.IsDoctype) {
                    tb.Error(this);
                    return false;

                } else if (t.IsStartTag) {
                    Token.StartTag startTag = t.AsStartTag();
                    string name = startTag.Name;

                    switch (name) {
                        case "caption":
                            tb.ClearStackToTableContext();
                            tb.InsertMarkerToFormattingElements();
                            tb.Insert(startTag);
                            tb.Transition(InCaption);
                            break;

                        case "colgroup":
                            tb.ClearStackToTableContext();
                            tb.Insert(startTag);
                            tb.Transition(InColumnGroup);
                            break;

                        case "col":
                            tb.Process(new Token.StartTag("colgroup"));
                            return tb.Process(t);

                        case "table":
                            tb.Error(this);
                            bool processed = tb.Process(new Token.EndTag("table"));
                            if (processed) // only ignored if in fragment
                                return tb.Process(t);
                            break;

                        case "tbody":
                        case "tfoot":
                        case "thead":
                            tb.ClearStackToTableContext();
                            tb.Insert(startTag);
                            tb.Transition(InTableBody);
                            break;

                        case "td":
                        case "th":
                        case "tr":
                            tb.Process(new Token.StartTag("tbody"));
                            return tb.Process(t);

                        case "style":
                        case "script":
                            return tb.Process(t, InHead);

                        case "input":
                            if (!startTag.Attributes["type"]
                                .Equals("hidden", StringComparison.OrdinalIgnoreCase)) {
                                return AnythingElse(t, tb);

                            } else {
                                tb.InsertEmpty(startTag);
                            }
                            break;

                        case "form":
                            tb.Error(this);
                            if (tb.FormElement != null)
                                return false;

                            else {
                                HtmlElement form = tb.InsertEmpty(startTag);
                                tb.FormElement = form;
                            }
                            break;
                        default:
                            return AnythingElse(t, tb);
                    }


                } else if (t.IsEndTag) {
                    Token.EndTag endTag = t.AsEndTag();
                    string name = endTag.Name;

                    switch (name) {
                        case "table":
                            if (!tb.InTableScope(name)) {
                                tb.Error(this);
                                return false;

                            } else {
                                tb.PopStackToClose("table");
                            }
                            tb.ResetInsertionMode();
                            break;

                        case "body":
                        case "caption":
                        case "col":
                        case "colgroup":
                        case "html":
                        case "tbody":
                        case "td":
                        case "tfoot":
                        case "th":
                        case "thead":
                        case "tr":
                            tb.Error(this);
                            return false;

                        default:
                            return AnythingElse(t, tb);
                    }

                } else if (t.IsEOF) {
                    if (tb.CurrentElement.NodeName.Equals("html"))
                        tb.Error(this);

                    return true; // stops parsing
                }

                return AnythingElse(t, tb);
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.Error(this);
                bool processed = true;

                if (StringUtil.In(tb.CurrentElement.NodeName,
                                   "table", "tbody", "tfoot", "thead", "tr")) {

                    tb.SetFosterInserts(true);
                    processed = tb.Process(t, InBody);
                    tb.SetFosterInserts(false);

                } else {
                    processed = tb.Process(t, InBody);
                }

                return processed;
            }
        }

    }
}
