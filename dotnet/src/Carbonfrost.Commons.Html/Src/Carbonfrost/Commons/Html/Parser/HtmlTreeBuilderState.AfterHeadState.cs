//
// - HtmlTreeBuilderState.AfterHeadState.cs -
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

        class AfterHeadState : HtmlTreeBuilderState {

            public override bool Process(Token t, HtmlTreeBuilder tb) {

                if (IsWhitespace(t)) {
                    tb.Insert(t.AsCharacter());

                } else if (t.IsComment) {
                    tb.Insert(t.AsComment());

                } else if (t.IsDoctype) {
                    tb.Error(this);

                } else if (t.IsStartTag) {
                    Token.StartTag startTag = t.AsStartTag();
                    string name = startTag.Name;

                    switch (name) {
                        case "html":
                            return tb.Process(t, InBody);

                        case "body":
                            tb.Insert(startTag);
                            tb.FramesetOK = false;
                            tb.Transition(InBody);
                            break;

                        case "frameset":
                            tb.Insert(startTag);
                            tb.Transition(InFrameset);
                            break;

                        case "base":
                        case "basefont":
                        case "bgsound":
                        case "link":
                        case "meta":
                        case "noframes":
                        case "script":
                        case "style":
                        case "title":
                            tb.Error(this);

                            HtmlElement head = tb.HeadElement;
                            tb.Push(head);
                            tb.Process(t, InHead);
                            tb.RemoveFromStack(head);
                            break;

                        case "head":
                            tb.Error(this);
                            return false;

                        default:
                            AnythingElse(t, tb);
                            break;
                    }

                } else if (t.IsEndTag) {
                    if (StringUtil.In(t.AsEndTag().Name, "body", "html")) {
                        AnythingElse(t, tb);

                    } else {
                        tb.Error(this);
                        return false;
                    }

                } else {
                    AnythingElse(t, tb);
                }
                return true;
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.Process(new Token.StartTag("body"));
                tb.FramesetOK = true;
                return tb.Process(t);
            }
        }

    }
}
