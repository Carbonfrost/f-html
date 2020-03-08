//
// - HtmlTreeBuilderState.InCaptionState.cs -
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

        class InCaptionState : HtmlTreeBuilderState {

            public override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsEndTag && t.AsEndTag().Name.Equals("caption")) {
                    Token.EndTag endTag = t.AsEndTag();
                    string name = endTag.Name;

                    if (!tb.InTableScope(name)) {
                        tb.Error(this);
                        return false;

                    } else {

                        tb.GenerateImpliedEndTags();
                        if (!tb.CurrentElement.NodeName.Equals("caption"))
                            tb.Error(this);

                        tb.PopStackToClose("caption");
                        tb.ClearFormattingElementsToLastMarker();
                        tb.Transition(InTable);
                    }

                } else if ((t.IsStartTag
                            && StringSet.Create("caption col colgroup tbody td tfoot th thead tr").Contains(t.AsStartTag().Name) ||
                            t.IsEndTag && t.AsEndTag().Name.Equals("table"))
                          ) {
                    tb.Error(this);
                    bool processed = tb.Process(new Token.EndTag("caption"));
                    if (processed)
                        return tb.Process(t);

                } else if (t.IsEndTag && StringSet.Create("body col colgroup html tbody td tfoot th thead tr").Contains(t.AsEndTag().Name)) {
                    tb.Error(this);
                    return false;

                } else {
                    return tb.Process(t, InBody);
                }

                return true;
            }
        }
    }
}
