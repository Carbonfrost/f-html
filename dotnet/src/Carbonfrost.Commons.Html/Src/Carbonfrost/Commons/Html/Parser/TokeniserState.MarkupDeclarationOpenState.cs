//
// - TokeniserState.MarkupDeclarationOpenState.cs -
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

    partial class TokeniserState {

        class MarkupDeclarationOpenState : TokeniserState {

            public override void Read(Tokeniser t, CharacterReader r) {

                if (r.MatchConsume("--")) {
                    t.CreateCommentPending();
                    t.Transition(CommentStart);

                } else if (r.MatchConsumeIgnoreCase("DOCTYPE")) {
                    t.Transition(Doctype);

                } else if (r.MatchConsume("[CDATA[")) {
                    // TODO: should actually check current namepspace, and only non-html allows cdata. until namespace
                    // is implemented properly, keep handling as cdata (HtmlCDataSection)
                    //} else if (!t.currentNodeInHtmlNS() && r.matchConsume("[CDATA[")) {
                    t.Transition(CdataSection);

                } else {
                    t.Error(this);
                    t.AdvanceTransition(BogusComment); // advance so this char gets in bogus comment data's rewind
                }
            }
        }

    }
}
