//
// - TokeniserState.TagNameState.cs -
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

        class TagNameState : TokeniserState {

            // from < or </ in data, will have start or end tag pending
            public override void Read(Tokeniser t, CharacterReader r) {
                // previous TagOpen state did NOT consume, will have a letter char in current
                string tagName = r.ConsumeToAny('\t', '\n', '\f', ' ', '/', '>', nullChar).ToLowerInvariant();
                t.tagPending.AppendTagName(tagName);

                switch (r.Consume()) {
                    case '\t':
                    case '\n':
                    case '\f':
                    case ' ':
                        t.Transition(BeforeAttributeName);
                        break;

                    case '/':
                        t.Transition(SelfClosingStartTag);
                        break;

                    case '>':
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;

                    case nullChar: // replacement
                        t.tagPending.AppendTagName(replacementStr);
                        break;

                    case eof: // should emit pending tag?
                        t.EofError(this);
                        t.Transition(Data);
                        // no default, as covered with above consumeToAny
                        break;
                }
            }
        }

    }
}
