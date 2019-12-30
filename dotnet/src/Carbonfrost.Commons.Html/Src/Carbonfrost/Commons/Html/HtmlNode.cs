//
// - HtmlNode.cs -
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using Carbonfrost.Commons.Html.Parser;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Web.Dom;
using HtmlParser = Carbonfrost.Commons.Html.Parser.Parser;

namespace Carbonfrost.Commons.Html {

    public abstract partial class HtmlNode {

        // TODO ICloneable
        // TODO Try to push down to Element if properties aren't appropriate to text and attributes

        internal static readonly Uri DEFAULT_URL = new Uri("file:///");
        private List<HtmlNode> _childNodes;
        private HtmlAttributeCollection _attributes;
        private Uri _baseUri;
        private int _siblingIndex;

        public HtmlNode NextSibling {
            get {
                if (Parent == null)
                    return null; // root

                IList<HtmlNode> siblings = Parent.ChildNodes;
                int index = this.NodePosition;
                if (index + 1 < siblings.Count)
                    return siblings[index + 1];
                else
                    return null;
            }
        }

        public Uri BaseUri {
            get {
                return _baseUri;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                Traverse(new UpdateBaseUriVisitor(value));
            }
        }

        public ReadOnlyCollection<HtmlNode> ChildNodes {
            get {
                return _childNodes.AsReadOnly();
            }
        }

        public virtual string OuterHtml {
            get {
                StringBuilder accum = new StringBuilder();
                new OuterHtmlNodeVisitor(accum).Visit(this);
                return accum.ToString();
            }
        }

        public virtual string OuterText {
            get {

                throw new NotImplementedException();
            }
        }

        public virtual string InnerHtml {
            get {
                return null;
            }
            set {
            }
        }

        public virtual string TextContent {
            get {
                return null;
            }
            set {
            }
        }

        public virtual string InnerText {
            get {
                return null;
            }
            set {

            }
        }

        public HtmlNode Parent {
            get; private set;
        }

        public virtual bool HasAttributes {
            get {
                return true;
            }
        }

        public int NodePosition {
            get {
                return _siblingIndex;
            }
        }

        public virtual string NodeValue {
            get { return null; }
            set {}
        }

        public HtmlAttributeCollection Attributes {
            get {
                if (HasAttributes) {
                    if (_attributes == null) {
                        _attributes = new HtmlAttributeCollection();
                    }

                    return _attributes;

                } else {
                    return null;
                }
            }
        }

        protected HtmlNode(Uri baseUri, HtmlAttributeCollection attributes) {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            _childNodes = new List<HtmlNode>(4);
            this._baseUri = baseUri;
            this._attributes = attributes;
        }

        protected HtmlNode(Uri baseUri) : this(baseUri, new HtmlAttributeCollection()) {
        }

        protected HtmlNode() {
            _childNodes = new List<HtmlNode>(0);
            _attributes = null;
        }

        public abstract HtmlNodeType NodeType { get; }
        public abstract string NodeName { get; }

        public HtmlNode ChildNode(int index) {
            return _childNodes[index];
        }

        public HtmlDocument OwnerDocument {
            get {
                if (this.NodeType == HtmlNodeType.Document)
                    return (HtmlDocument) this;
                else if (Parent == null)
                    return null;
                else
                    return Parent.OwnerDocument;
            }
        }

        public void Remove() {
            if (Parent != null)
                Parent.RemoveChild(this);
        }

        public HtmlNode Before(string html) {
            AddSiblingHtml(this.NodePosition, html);
            return this;
        }

        public HtmlNode Before(HtmlNode node) {
            if (node == null)
                throw new ArgumentNullException("node");
            if (Parent == null)
                throw HtmlFailure.ParentNodeRequired();

            Parent.AddChildren(this.NodePosition, node);
            return this;
        }

        public HtmlNode After(string html) {
            AddSiblingHtml(this.NodePosition + 1, html);
            return this;
        }

        public HtmlNode After(HtmlNode node) {
            if (node == null)
                throw new ArgumentNullException("node");
            if (Parent == null)
                throw HtmlFailure.ParentNodeRequired();

            Parent.AddChildren(this.NodePosition + 1, node);
            return this;
        }

        private void AddSiblingHtml(int index, string html) {
            if (string.IsNullOrEmpty(html))
                return;

            if (Parent == null)
                throw HtmlFailure.ParentNodeRequired();

            HtmlElement context = Parent as HtmlElement;
            IList<HtmlNode> nodes = HtmlParser.ParseFragment(html, context, this.BaseUri);
            Parent.AddChildren(index, nodes.ToArray());
        }

        public HtmlNode Wrap(string html) {
            if (html == null)
                throw new ArgumentNullException("html");
            if (html.Length == 0)
                throw Failure.EmptyString("html");

            HtmlElement context = Parent as HtmlElement;
            IList<HtmlNode> wrapChildren = HtmlParser.ParseFragment(html, context, this.BaseUri);
            HtmlNode wrapNode = wrapChildren[0];
            if (wrapNode == null || !(wrapNode is HtmlElement)) // nothing to wrap with; noop
                return null;

            HtmlElement wrap = (HtmlElement) wrapNode;
            HtmlElement deepest = GetDeepChild(wrap);
            Parent.ReplaceChild(this, wrap);
            deepest.AddChildren(this);

            // remainder (unbalanced wrap, like <div></div><p></p> -- The <p> is remainder
            if (wrapChildren.Count > 0) {
                for (int i = 0; i < wrapChildren.Count; i++) {
                    HtmlNode remainder = wrapChildren[i];
                    remainder.Parent.RemoveChild(remainder);
                    wrap.AppendChild(remainder);
                }
            }
            return this;
        }

        public HtmlNode Unwrap() {
            if (Parent == null)
                throw HtmlFailure.ParentNodeRequired();

            int index = this.NodePosition;
            HtmlNode firstChild = ChildNodes.Count > 0 ? ChildNodes[0] : null;
            Parent.AddChildren(index, this._childNodes.ToArray());
            this.Remove();

            return firstChild;
        }

        private HtmlElement GetDeepChild(HtmlElement el) {
            IList<HtmlElement> children = el.Children;
            if (children.Count > 0)
                return GetDeepChild(children[0]);
            else
                return el;
        }

        public void ReplaceWith(HtmlNode other) {
            if (other == null)
                throw new ArgumentNullException("other");
            if (Parent == null)
                throw HtmlFailure.ParentNodeRequired();

            Parent.ReplaceChild(this, other);
        }

        internal void SetParentNode(HtmlNode parentNode) {
            if (this.Parent != null)
                this.Parent.RemoveChild(this);
            this.Parent = parentNode;
        }

        internal void ReplaceChild(HtmlNode outNode, HtmlNode inNode) {
            if (inNode == null)
                throw new ArgumentNullException("inNode");

            Debug.Assert(outNode.Parent == this);
            if (inNode.Parent != null)
                inNode.Parent.RemoveChild(inNode);

            int index = outNode.NodePosition;
            _childNodes[index] = inNode;
            inNode.Parent = this;
            inNode.SetSiblingIndex(index);
            outNode.Parent = null;
        }

        internal void RemoveChild(HtmlNode outNode) {
            Debug.Assert(outNode.Parent == this);
            int index = outNode.NodePosition;
            _childNodes.RemoveAt(index);
            ReindexChildren();
            outNode.Parent = null;
        }

        internal void AddChildren(params HtmlNode[] children) {
            AddChildren((IEnumerable<HtmlNode>) children);
        }

        internal void AddChildren(IEnumerable<HtmlNode> children) {
            //most used. short circuit addChildren(int), which hits reindex children and array copy
            foreach (HtmlNode child in children) {
                ReparentChild(child);
                _childNodes.Add(child);
                child.SetSiblingIndex(ChildNodes.Count - 1);
            }
        }

        internal void AddChildren(int index, params HtmlNode[] children) {
            Debug.Assert(children.All(t => t != null));
            for (int i = children.Length - 1; i >= 0; i--) {
                HtmlNode in2 = children[i];
                ReparentChild(in2);
                _childNodes.Insert(index, in2);
            }

            ReindexChildren();
        }

        private void ReparentChild(HtmlNode child) {
            if (child.Parent != null)
                child.Parent.RemoveChild(child);
            child.SetParentNode(this);
        }

        private void ReindexChildren() {
            for (int i = 0; i < ChildNodes.Count; i++) {
                ChildNodes[i].SetSiblingIndex(i);
            }
        }

        public ReadOnlyCollection<HtmlNode> SiblingNodes {
            get {
                if (Parent == null)
                    return Empty<HtmlNode>.ReadOnly;

                return new ReadOnlyCollection<HtmlNode>(Parent.ChildNodes.Except(this).ToArray());
            }
        }

        public HtmlNode PreviousSibling {
            get {
                if (Parent == null)
                    return null; // root

                IList<HtmlNode> siblings = Parent.ChildNodes;
                int index = this.NodePosition;
                if (index > 0)
                    return siblings[index - 1];
                else
                    return null;
            }
        }

        protected void SetSiblingIndex(int siblingIndex) {
            this._siblingIndex = siblingIndex;
        }

        internal HtmlNode Traverse(INodeVisitor nodeVisitor) {
            if (nodeVisitor == null)
                throw new ArgumentNullException("nodeVisitor");

            NodeTraversor traversor = new NodeTraversor(nodeVisitor);
            traversor.Traverse(this);
            return this;
        }

        public abstract void AcceptVisitor(HtmlNodeVisitor visitor);
        public abstract TResult AcceptVisitor<TArgument, TResult>(HtmlNodeVisitor<TArgument, TResult> visitor, TArgument argument);

        public override bool Equals(Object o) {
            if (this == o)
                return true;

            // TODO: have nodes hold a child index, compare against that and parent (not children)
            return false;
        }

        public HtmlNode Clone() {
            return DoClone(null); // splits for orphan
        }

        protected HtmlNode DoClone(HtmlNode parent) {
            HtmlNode clone = (HtmlNode) MemberwiseClone();
            clone.Parent = parent; // can be null, to create an orphan split
            clone._siblingIndex = parent == null ? 0 : _siblingIndex;
            clone._attributes = _attributes != null ? _attributes.Clone() : null;
            clone._baseUri = _baseUri;
            clone._childNodes = new List<HtmlNode>(_childNodes.Count);
            foreach (HtmlNode child in _childNodes)
                clone._childNodes.Add(child.DoClone(clone)); // clone() creates orphans, doClone() keeps parent

            return clone;
        }

        // `object' overrides
        public override string ToString() {
            return OuterHtml;
        }

        sealed class UpdateBaseUriVisitor : INodeVisitor {

            private Uri baseUri;

            public UpdateBaseUriVisitor(Uri baseUri) {
                this.baseUri = baseUri;
            }

            public void Head(HtmlNode node, int depth) {
                node._baseUri = baseUri;
            }

            public void Tail(HtmlNode node, int depth) {
            }
        }

    }

}
