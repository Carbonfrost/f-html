//
// Copyright 2012, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
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
using System.Text;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    public partial class HtmlDocument : DomDocument, IHtmlNode {

        internal static readonly Uri DEFAULT_URL = new Uri("file:///");

        public QuirksMode QuirksMode {
            get {
                return this.GetQuirksMode();
            }
            set {
                this.SetQuirksMode(value);
            }
        }

        public new HtmlProviderFactory ProviderFactory {
            get {
                return (HtmlProviderFactory) base.ProviderFactory;
            }
        }

        protected override DomProviderFactory DomProviderFactory {
            get {
                return HtmlProviderFactory.Instance;
            }
        }

        public HtmlDocument()
            : this(DEFAULT_URL) {
        }

        public HtmlDocument(Uri baseUri) : base() {
            BaseUri = baseUri;
        }

        public string Title {
            get {
                var titleEl = GetElementsByTagName("title").First();
                if (titleEl == null) {
                    return string.Empty;
                }
                return titleEl.InnerText.Trim();
            }
            set {
                var titleEl = GetElementsByTagName("title").First();
                if (titleEl == null) { // add to head
                    var e = Head.AppendElement("title");
                    e.InnerText = value;

                } else {
                    titleEl.InnerText = value;
                }
            }
        }

        public HtmlElement Head {
            get {
                return FindFirstElementByTagName("head", this);
            }
        }

        public HtmlElement Body {
            get {
                return FindFirstElementByTagName("body", this);
            }
        }

        public HtmlDocument Normalize() {
            HtmlElement htmlEl = FindFirstElementByTagName("html", this);
            if (htmlEl == null)
                htmlEl = (HtmlElement) AppendElement("html");

            if (Head == null) {
                htmlEl.PrependElement("head");
            }

            if (Body == null) {
                htmlEl.AppendElement("body");
            }

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
            return (HtmlDocument) base.Clone();
        }

        internal static HtmlDocument CreateShell(Uri baseUri) {
            HtmlDocument doc = new HtmlDocument(baseUri);
            var html = doc.AppendElement("html");
            html.AppendElement("head");
            html.AppendElement("body");

            return doc;
        }

        // does not recurse.
        private void NormaliseTextNodes(DomContainer element) {
            List<DomNode> toMove = new List<DomNode>();
            foreach (var node in element.ChildNodes) {
                if (node is HtmlText tn) {
                    if (!tn.IsBlank) {
                        toMove.Add(tn);
                    }
                }
            }

            for (int i = toMove.Count-1; i >= 0; i--) {
                var node = toMove[i];
                node.RemoveSelf();
                Body.Prepend(new HtmlText(" ", null, false));
                Body.Prepend(node);
            }
        }

        // merge multiple <head> or <body> contents into one, delete the remainder, and ensure they are owned by <html>
        private void NormaliseStructure(string tag, HtmlElement htmlEl) {
            var elements = GetElementsByTagName(tag).ToList();

            var master = elements.First(); // will always be available as created above if not existent

            if (elements.Count > 1) { // dupes, move contents to master
                var toMove = new List<DomNode>();
                for (int i = 1; i < elements.Count; i++) {
                    var dupe = elements[(i)];
                    foreach (var node in dupe.ChildNodes) {
                        toMove.Add(node);
                    }
                    dupe.Remove();
                }

                foreach (var dupe in toMove) {
                    master.Append(dupe);
                }
            }

            // ensure parented by <html>
            if (!master.Parent.Equals(htmlEl)) {
                htmlEl.Append(master); // includes remove()
            }
        }

        // fast method to get first by tag name, used for html, head, body finders
        private HtmlElement FindFirstElementByTagName(string tag, DomNode node) {
            if (node.NodeName.Equals(tag))
                return (HtmlElement) node;

            else {
                foreach (var child in node.ChildNodes) {
                    HtmlElement found = FindFirstElementByTagName(tag, child);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        public string OuterHtml {
            get {
                return InnerHtml;
            }
            set {
                InnerHtml = value;
            }
        }

        public string InnerHtml {
            get {
                StringBuilder accum = new StringBuilder();
                var v = new OuterHtmlNodeVisitor(accum);
                foreach (var node in ChildNodes) {
                    v.Visit(node);
                }

                return accum.ToString().Trim();
            }
            set {
                Empty();
                Append(value);
            }
        }

        public override string InnerText {
            get {
                return DocumentElement.InnerText;
            }
            set {
                Body.InnerText = value;
            }
        }
    }
}
