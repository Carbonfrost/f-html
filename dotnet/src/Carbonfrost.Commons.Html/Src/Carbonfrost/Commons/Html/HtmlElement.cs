//
// - HtmlElement.cs -
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Carbonfrost.Commons.Html.Query;
using HtmlParser = Carbonfrost.Commons.Html.Parser.Parser;

namespace Carbonfrost.Commons.Html {

    public partial class HtmlElement : HtmlNode {

        private Tag _tag;

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

        public new HtmlElement Parent {
            get {
                return (HtmlElement) base.Parent;
            }
        }

        public Tag Tag {
            get { return _tag; }
        }

        public bool HasText {
            get {
                foreach (HtmlNode child in ChildNodes) {
                    if (child.NodeType == HtmlNodeType.Text) {
                        HtmlText textNode = (HtmlText) child;
                        if (!textNode.IsBlank)
                            return true;

                    } else if (child.NodeType == HtmlNodeType.Element) {
                        HtmlElement el = (HtmlElement) child;
                        if (el.HasText)
                            return true;
                    }
                }
                return false;
            }
        }

        public HtmlElement NextElementSibling {
            get {
                if (Parent == null)
                    return null;

                IList<HtmlElement> siblings = Parent.Children;
                int? index = IndexInList(this, siblings);
                if (!index.HasValue) {
                    HtmlWarning.ExpectedChildInParentCollection();
                    return null;
                }

                if (siblings.Count > index + 1)
                    return siblings[index.Value + 1];
                else
                    return null;
            }
        }

        public HtmlElement PreviousElementSibling {
            get {
                if (Parent == null)
                    return null;

                IList<HtmlElement> siblings = Parent.Children;
                int? index = IndexInList(this, siblings);
                if (!index.HasValue) {
                    HtmlWarning.ExpectedChildInParentCollection();
                    return null;
                }

                if (index > 0)
                    return siblings[index.Value - 1];
                else
                    return null;
            }
        }

        internal bool PreserveWhitespace {
            get {
                return this.Tag.PreserveWhitespace
                    || Parent != null
                    && Parent.PreserveWhitespace;
            }
        }

        public override string InnerHtml {
            get {
                StringBuilder accum = new StringBuilder();
                OuterHtmlNodeVisitor v = new OuterHtmlNodeVisitor(accum);
                foreach (HtmlNode node in ChildNodes)
                    v.Visit(node);

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
                AppendChild(textNode);
            }
        }

        public ReadOnlyCollection<HtmlElement> SiblingElements {
            get {
                if (this.Parent == null)
                    return Empty<HtmlElement>.ReadOnly;

                return new ReadOnlyCollection<HtmlElement>(
                    Parent.Children.Except(this).ToArray());
            }
        }

        public string Id {
            get {
                string id = this.Attribute("id");
                return id == null ? string.Empty : id;
            }
            set {
                this.Attribute("id", value);
            }
        }

        internal HtmlElement(Tag tag, Uri baseUri, HtmlAttributeCollection attributes) : base(baseUri, attributes) {
            if (tag == null)
                throw new ArgumentNullException("tag");
            this._tag = tag;
        }

        internal HtmlElement(Tag tag, Uri baseUri) :
            this(tag, baseUri, new HtmlAttributeCollection()) {
        }

        public override string NodeName {
            get {
                return Tag.Name;
            }
        }

        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.Element;
            }
        }

        // TODO These should be "live" collections

        public ReadOnlyCollection<HtmlElement> Parents {
            get {
                return new ReadOnlyCollection<HtmlElement>(ParentsIterator().ToArray());
            }
        }

        public ReadOnlyCollection<HtmlElement> Children {
            get {
                return new ReadOnlyCollection<HtmlElement>(ChildNodes.Where(t => t.NodeType == HtmlNodeType.Element).Cast<HtmlElement>().ToArray());
            }
        }

        public HtmlElement Child(int index) {
            return Children[index];
        }

        public HtmlElementQuery Select(string cssQuery) {
            return new CssSelector(cssQuery, this).Select();
        }

        public HtmlElement AppendText(string text) {
            HtmlText node = new HtmlText(text, BaseUri, false);
            AppendChild(node);
            return this;
        }

        public HtmlElement PrependText(string text) {
            HtmlText node = new HtmlText(text, BaseUri, false);
            PrependChild(node);
            return this;
        }

        public HtmlElement Append(string html) {
            if (html == null)
                throw new ArgumentNullException("html");

            IList<HtmlNode> nodes = HtmlParser.ParseFragment(html, this, BaseUri);
            AddChildren(nodes.ToArray());
            return this;
        }

        public HtmlElement Prepend(string html) {
            if (html == null)
                throw new ArgumentNullException("html");

            IList<HtmlNode> nodes = HtmlParser.ParseFragment(html, this, this.BaseUri);
            AddChildren(0, nodes.ToArray());
            return this;
        }

        public new HtmlElement Before(string html) {
            return (HtmlElement) base.Before(html);
        }

        public new HtmlElement Before(HtmlNode node) {
            return (HtmlElement) base.Before(node);
        }

        public new HtmlElement After(string html) {
            return (HtmlElement) base.After(html);
        }

        public new HtmlElement After(HtmlNode node) {
            return (HtmlElement) base.After(node);
        }

        public new HtmlElement Wrap(string html) {
            return (HtmlElement) base.Wrap(html);
        }

        public HtmlElement FirstElementSibling {
            get {
                // TODO: should firstSibling() exclude this?
                return Parent.Children.FirstOrDefault();
            }
        }

        public int Position {
            get {
                if (Parent == null)
                    return 0;

                return IndexInList(this, Parent.Children).Value;
            }
        }

        public HtmlElement LastElementSibling {
            get {
                return Parent.Children.LastOrDefault();
            }
        }

        private static int? IndexInList<E>(HtmlElement search, IList<E> elements)
            where E : HtmlElement {
            if (search == null)
                throw new ArgumentNullException("search");
            if (elements == null)
                throw new ArgumentNullException("elements");

            for (int i = 0; i < elements.Count; i++) {
                E element = elements[i];
                if (element.Equals(search))
                    return i;
            }
            return null;
        }

        public override string ToString() {
            return OuterHtml;
        }

        public override bool Equals(Object obj) {
            return this == obj;
        }

        public override int GetHashCode() {
            // TODO: fixup, not very useful
            int result = base.GetHashCode();
            result = 31 * result + (Tag != null ? Tag.GetHashCode() : 0);
            return result;
        }

        public new HtmlElement Clone() {
            HtmlElement clone = (HtmlElement) this.MemberwiseClone();
            return clone;
        }

        public override TResult AcceptVisitor<TArgument, TResult>(HtmlNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitElement(this, argument);
        }

        public override void AcceptVisitor(HtmlNodeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            visitor.VisitElement(this);
        }

        private Tag GetTag(string tag) {
            return Tag.ValueOf(tag);
        }

        private IEnumerable<HtmlElement> ParentsIterator() {
            HtmlElement parent = this.Parent;
            while (parent != null && !parent.Tag.Name.Equals(NodeNames.Document) && !parent.Tag.Name.Equals(NodeNames.DocumentFragment)) {
                yield return parent;
                parent = parent.Parent;
            }
        }
    }

}
