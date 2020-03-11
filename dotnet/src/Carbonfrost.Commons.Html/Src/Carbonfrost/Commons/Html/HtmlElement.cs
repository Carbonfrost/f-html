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
using HtmlParser = Carbonfrost.Commons.Html.Parser.Parser;

namespace Carbonfrost.Commons.Html {

    public partial class HtmlElement : DomElement<HtmlElement>, IHtmlNode {

        public bool IsBlock {
            get {
                return Tag.IsBlock;
            }
        }

        public string Data {
            get {
                return new InnerDataVisitor(this).ToString();
            }
        }

        internal HtmlElementDefinition Tag {
            get {
                return ElementDefinition;
            }
        }

        public new HtmlElementDefinition ElementDefinition {
            get {
                return (HtmlElementDefinition) base.ElementDefinition;
            }
        }

        protected override DomElementDefinition DomElementDefinition {
            get {
                return this.FindSchema().GetTag(Name);
            }
        }

        public virtual string OuterHtml {
            get {
                StringBuilder accum = new StringBuilder();
                var v = new OuterHtmlNodeVisitor(accum);
                v.Visit(this);

                return accum.ToString().Trim();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool HasText {
            get {
                foreach (var child in ChildNodes) {
                    if (child.NodeType == DomNodeType.Text) {
                        HtmlText textNode = (HtmlText) child;
                        if (!textNode.IsBlank) {
                            return true;
                        }

                    } else if (child.NodeType == DomNodeType.Element) {
                        HtmlElement el = (HtmlElement) child;
                        if (el.HasText)
                            return true;
                    }
                }
                return false;
            }
        }

        internal bool PreserveWhitespace {
            get {
                if (Tag.PreserveWhitespace) {
                    return true;
                }
                if (Parent is HtmlElement he) {
                    return he.PreserveWhitespace;
                }
                return false;
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
                return new InnerTextVisitor(this).ToString();
            }
            set {
                Empty();
                HtmlText textNode = new HtmlText(value, this.BaseUri);
                Append(textNode);
            }
        }

        internal HtmlElement(string tagName, Uri baseUri, IEnumerable<HtmlAttribute> attributes) : base(tagName) {
            if (attributes != null) {
                Attributes.AddMany(attributes);
            }
            BaseUri = baseUri;
        }

        internal HtmlElement(string tagName, Uri baseUri) :
            this(tagName, baseUri, null) {
        }

        internal HtmlElement(string tagName) :
            this(tagName, null, null) {
        }

        internal void RemoveChild(DomNode outNode) {
            System.Diagnostics.Debug.Assert(outNode.ParentElement == this);
            if (outNode.ParentElement == this) {
                outNode.RemoveSelf();
            }
        }

        // TODO Requires f-web-dom upgrade to include DomReader/HtmlReader
        public HtmlElement Append(string html) {
            if (html == null)
                throw new ArgumentNullException("html");

            IList<DomNode> nodes = HtmlParser.ParseFragment(html, this, BaseUri);
            Append(nodes.ToArray());
            return this;
        }

        public HtmlElement Prepend(string html) {
            if (html == null)
                throw new ArgumentNullException("html");

            var nodes = HtmlParser.ParseFragment(html, this, this.BaseUri);
            Prepend(nodes.ToArray());
            return this;
        }

        public new HtmlElement Clone() {
            return (HtmlElement) base.Clone();
        }
    }
}
