//
// - HtmlNodeVisitor.cs -
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

using System;

namespace Carbonfrost.Commons.Html {

    public abstract class HtmlNodeVisitor {

        public void Visit(HtmlNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            node.AcceptVisitor(this);
        }

        public virtual void VisitElement(HtmlElement node) {
            if (node == null)
                throw new ArgumentNullException("node");

            DefaultVisit(node);
            foreach (var attr in node.Attributes) {
                VisitAttribute(attr);
            }
            foreach (var child in node.ChildNodes) {
                Visit(child);
            }
        }

        public virtual void VisitAttribute(HtmlAttribute node) {
            if (node == null)
                throw new ArgumentNullException("node");

            DefaultVisit(node);
        }

        public virtual void VisitText(HtmlText node) {
            if (node == null)
                throw new ArgumentNullException("node");

            DefaultVisit(node);
        }

        protected virtual void DefaultVisit(HtmlNode node) {}

        public virtual void VisitComment(HtmlComment node) {
            if (node == null)
                throw new ArgumentNullException("node");

            DefaultVisit(node);
        }

        public virtual void VisitDocumentType(HtmlDocumentType node) {
            if (node == null)
                throw new ArgumentNullException("node");

            DefaultVisit(node);
        }

        public virtual void VisitDocument(HtmlDocument node) {
            if (node == null)
                throw new ArgumentNullException("node");

            DefaultVisit(node);
            DefaultVisitChildNodes(node);
        }

        public virtual void VisitDocumentFragment(HtmlDocumentFragment node) {
            if (node == null)
                throw new ArgumentNullException("node");

            DefaultVisit(node);
            DefaultVisitChildNodes(node);
        }

        protected virtual void DefaultVisitChildNodes(HtmlNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            foreach (var child in node.ChildNodes) {
                Visit(child);
            }
        }

        public virtual void VisitProcessingInstruction(HtmlProcessingInstruction node) {
            if (node == null)
                throw new ArgumentNullException("node");
            DefaultVisit(node);
        }

        public virtual void VisitEntityReference(HtmlEntityReference node) {
            if (node == null)
                throw new ArgumentNullException("node");
            DefaultVisit(node);
        }

        public virtual void VisitCDataSection(HtmlCDataSection node) {
            if (node == null)
                throw new ArgumentNullException("node");
            DefaultVisit(node);
        }
    }
}
