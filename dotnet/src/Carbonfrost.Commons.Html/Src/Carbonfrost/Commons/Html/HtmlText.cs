//
// - HtmlText.cs -
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
using System.Text;
using System.Text.RegularExpressions;

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    public class HtmlText : HtmlCharacterData {

        // TODO Improve definition of behavior isData, etc.; remove redundancy

        private bool isData;
        private string text;

        public bool IsBlank {
            get {
                return StringUtil.IsBlank(this.RawText);
            }
        }

        public string Text {
            get {
                return StringUtil.NormalizeWhitespace(RawText);
            }
            set {
                this.text = value;
            }
        }

        public bool IsData {
            get {
                return isData;
            }
        }

        public string RawText {
            get {
                return text;
            }
            set {
                this.text = value;
            }
        }

        internal HtmlText(string text, Uri baseUri, bool isData = false)
            : base(baseUri)
        {
            this.Text = text;
            this.isData = isData;
        }

        public override string NodeName {
            get {
                return NodeNames.Text;
            }
        }

        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.Text;
            }
        }

        public override string TextContent {
            get {
                return this.RawText;
            }
            set {
                this.RawText = value;
            }
        }

        public HtmlText SplitText(int offset) {
            if (offset < 0)
                throw Failure.Negative("offset", offset);

            if (offset >= this.Text.Length)
                throw Failure.IndexOutOfRange("offset", offset);

            string head = this.RawText.Substring(0, offset);
            string tail = this.RawText.Substring(offset);
            this.Text = head;

            HtmlText tailNode = new HtmlText(tail, this.BaseUri);
            if (Parent != null)
                Parent.AddChildren(this.NodePosition + 1, tailNode);

            return tailNode;
        }

        public override string ToString() {
            return OuterHtml;
        }

        internal static string StripLeadingWhitespace(string text) {
            return Regex.Replace(text, "^\\s+", string.Empty);
        }

        internal static bool LastCharIsWhitespace(StringBuilder sb) {
            return sb.Length != 0 && sb[sb.Length - 1] == ' ';
        }

        public override TResult AcceptVisitor<TArgument, TResult>(HtmlNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitText(this, argument);
        }

        public override void AcceptVisitor(HtmlNodeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            visitor.VisitText(this);
        }
    }

}
