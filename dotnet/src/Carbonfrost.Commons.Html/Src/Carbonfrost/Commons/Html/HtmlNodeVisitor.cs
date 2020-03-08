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

using System;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    public abstract class HtmlNodeVisitor : DomNodeVisitor, IHtmlNodeVisitor {

        protected virtual void VisitElement(HtmlElement node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            DefaultVisit(node);
        }

        protected virtual void VisitAttribute(HtmlAttribute node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            DefaultVisit(node);
        }

        protected virtual void VisitText(HtmlText node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            DefaultVisit(node);
        }

        protected virtual void VisitDocument(HtmlDocument node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            DefaultVisit(node);
        }

        protected virtual void VisitDocumentFragment(HtmlDocumentFragment node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            DefaultVisit(node);
        }

        protected virtual void VisitProcessingInstruction(HtmlProcessingInstruction node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            DefaultVisit(node);
        }

        void IHtmlNodeVisitor.Visit(HtmlAttribute attribute) {
            VisitAttribute(attribute);
        }

        void IHtmlNodeVisitor.Visit(HtmlElement element) {
            VisitElement(element);
        }

        void IHtmlNodeVisitor.Visit(HtmlText text) {
            VisitText(text);
        }

        void IHtmlNodeVisitor.Visit(HtmlProcessingInstruction processingInstruction) {
            VisitProcessingInstruction(processingInstruction);
        }

        void IHtmlNodeVisitor.Visit(HtmlDocument document) {
            VisitDocument(document);
        }

        void IHtmlNodeVisitor.Visit(HtmlDocumentFragment documentFragment) {
            VisitDocumentFragment(documentFragment);
        }
    }
}
