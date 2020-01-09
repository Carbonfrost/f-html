//
// - HtmlDocument.cs -
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
using System.Linq;

namespace Carbonfrost.Commons.Html {

    public partial class HtmlDocument : HtmlElement {

        private QuirksMode _quirksMode = QuirksMode.None;

        public QuirksMode QuirksMode {
            get {
                return _quirksMode;
            }
            set {
                this._quirksMode = value;
            }
        }

        public HtmlElement DocumentElement {
            get {
                return this.Child(0);
            }
        }

        public HtmlDocument()
            : this(DEFAULT_URL) {
        }

        public HtmlDocument(Uri baseUri)
            : base(Tag.ValueOf(NodeNames.Document), baseUri) {
        }

        public string Title {
            get {
                HtmlElement titleEl = GetElementsByTag("title").First();
                if (titleEl == null)
                    return string.Empty;
                else
                    return titleEl.InnerText.Trim();
            }
            set {
                HtmlElement titleEl = GetElementsByTag("title").First();
                if (titleEl == null) { // add to head
                    Head.AppendElement("title").InnerText(value);

                } else {
                    titleEl.InnerText = value;
                }
            }
        }

        public HtmlElement Head {
            get { return FindFirstElementByTagName("head", this); }
        }

        public HtmlElement Body {
            get { return FindFirstElementByTagName("body", this); }
        }

        public HtmlDocument Normalize() {
            HtmlElement htmlEl = FindFirstElementByTagName("html", this);
            if (htmlEl == null)
                htmlEl = AppendElement("html");

            if (Head == null)
                htmlEl.PrependElement("head");

            if (Body == null)
                htmlEl.AppendElement("body");

            // pull text nodes out of root, html, and head els, and push into body. non-text nodes are already taken care
            // of. do in inverse order to maintain text order.
            NormaliseTextNodes(Head);
            NormaliseTextNodes(htmlEl);
            NormaliseTextNodes(this);

            NormaliseStructure("head", htmlEl);
            NormaliseStructure("body", htmlEl);

            return this;
        }

        public new HtmlDocument Clone() {
            HtmlDocument clone = (HtmlDocument) MemberwiseClone();
            return clone;
        }

        static public HtmlDocument CreateShell(Uri baseUri) {
            HtmlDocument doc = new HtmlDocument(baseUri);
            HtmlElement html = doc.AppendElement("html");
            html.AppendElement("head");
            html.AppendElement("body");

            return doc;
        }

        // does not recurse.
        private void NormaliseTextNodes(HtmlElement element) {
            List<HtmlNode> toMove = new List<HtmlNode>();
            foreach (HtmlNode node in element.ChildNodes) {
                if (node is HtmlText) {
                    HtmlText tn = (HtmlText)node;
                    if (!tn.IsBlank)
                        toMove.Add(tn);
                }
            }

            for (int i = toMove.Count-1; i >= 0; i--) {
                HtmlNode node = toMove[i];
                element.RemoveChild(node);
                Body.PrependChild(new HtmlText(" ", null, false));
                Body.PrependChild(node);
            }
        }

        // merge multiple <head> or <body> contents into one, delete the remainder, and ensure they are owned by <html>
        private void NormaliseStructure(string tag, HtmlElement htmlEl) {
            HtmlElementQuery elements = this.GetElementsByTag(tag);

            HtmlElement master = elements.First(); // will always be available as created above if not existent

            if (elements.Count > 1) { // dupes, move contents to master
                List<HtmlNode> toMove = new List<HtmlNode>();
                for (int i = 1; i < elements.Count; i++) {
                    HtmlNode dupe = elements[(i)];
                    foreach (HtmlNode node in dupe.ChildNodes)
                        toMove.Add(node);
                    dupe.Remove();
                }

                foreach (HtmlNode dupe in toMove)
                    master.AppendChild(dupe);
            }

            // ensure parented by <html>
            if (!master.Parent.Equals(htmlEl)) {
                htmlEl.AppendChild(master); // includes remove()
            }
        }

        // fast method to get first by tag name, used for html, head, body finders
        private HtmlElement FindFirstElementByTagName(string tag, HtmlNode node) {
            if (node.NodeName.Equals(tag))
                return (HtmlElement)node;

            else {
                foreach (HtmlNode child in node.ChildNodes) {
                    HtmlElement found = FindFirstElementByTagName(tag, child);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        public override string OuterHtml {
            get {
                // Don't print document node
                return this.InnerHtml;
            }
        }

        public override string InnerText {
            get { return base.InnerText; }
            set {
                this.Body.InnerText = value; // overridden to not nuke doc structure
            }
        }

        public override string NodeName {
            get {
                return NodeNames.Document;
            }
        }

        public override void AcceptVisitor(HtmlNodeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            visitor.VisitDocument(this);
        }

        public override TResult AcceptVisitor<TArgument, TResult>(HtmlNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitDocument(this, argument);
        }

    }

}
