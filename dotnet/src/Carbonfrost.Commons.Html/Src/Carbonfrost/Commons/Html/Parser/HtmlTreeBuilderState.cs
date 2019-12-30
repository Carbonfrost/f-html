//
// - HtmlTreeBuilderState.cs -
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

        private static string NullString = "\u0000";

        public abstract bool Process(Token t, HtmlTreeBuilder tb);

        public string Name {
            get {
                return GetType().Name;
            }
        }

        private static bool IsWhitespace(Token t) {
            if (t.IsCharacter) {
                string data = t.AsCharacter().Data;

                // TODO: this checks more than spec - "\t", "\n", "\f", "\r", " "
                foreach (char c in data) {
                    if (!StringUtil.IsWhitespace(c))
                        return false;
                }
                return true;
            }

            return false;
        }

        private static void HandleRcData(Token.StartTag startTag, HtmlTreeBuilder tb) {
            tb.Insert(startTag);
            tb.tokeniser.Transition(TokeniserState.Rcdata);
            tb.MarkInsertionMode();
            tb.Transition(Text);
        }

        private static void HandleRawtext(Token.StartTag startTag, HtmlTreeBuilder tb) {
            tb.Insert(startTag);
            tb.tokeniser.Transition(TokeniserState.Rawtext);
            tb.MarkInsertionMode();
            tb.Transition(Text);
        }

        public static readonly HtmlTreeBuilderState AfterAfterBody = new AfterAfterBodyState();
        public static readonly HtmlTreeBuilderState AfterAfterFrameset = new AfterAfterFramesetState();
        public static readonly HtmlTreeBuilderState AfterBody = new AfterBodyState();
        public static readonly HtmlTreeBuilderState AfterFrameset = new AfterFramesetState();
        public static readonly HtmlTreeBuilderState AfterHead = new AfterHeadState();
        public static readonly HtmlTreeBuilderState BeforeHead = new BeforeHeadState();
        public static readonly HtmlTreeBuilderState BeforeHtml = new BeforeHtmlState();
        public static readonly HtmlTreeBuilderState ForeignContent = new ForeignContentState();
        public static readonly HtmlTreeBuilderState InBody = new InBodyState();
        public static readonly HtmlTreeBuilderState InCaption = new InCaptionState();
        public static readonly HtmlTreeBuilderState InCell = new InCellState();
        public static readonly HtmlTreeBuilderState InColumnGroup = new InColumnGroupState();
        public static readonly HtmlTreeBuilderState InFrameset = new InFramesetState();
        public static readonly HtmlTreeBuilderState InHeadNoscript = new InHeadNoscriptState();
        public static readonly HtmlTreeBuilderState InHead = new InHeadState();
        public static readonly HtmlTreeBuilderState Initial= new InitialState();
        public static readonly HtmlTreeBuilderState InRow = new InRowState();
        public static readonly HtmlTreeBuilderState InSelect = new InSelectState();
        public static readonly HtmlTreeBuilderState InSelectInTable = new InSelectInTableState();
        public static readonly HtmlTreeBuilderState InTable = new InTableState();
        public static readonly HtmlTreeBuilderState InTableBody = new InTableBodyState();
        public static readonly HtmlTreeBuilderState InTableText = new InTableTextState();
        public static readonly HtmlTreeBuilderState Text = new TextState();

    }

}
