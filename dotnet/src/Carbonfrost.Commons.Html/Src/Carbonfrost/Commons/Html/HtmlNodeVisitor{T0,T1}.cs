//
// - HtmlNodeVisitor{T0,T1}.cs -
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

    public abstract class HtmlNodeVisitor<TArgument, TResult> {

        public TResult Visit(HtmlNode node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            return node.AcceptVisitor(this, argument);
        }

        public virtual TResult VisitElement(HtmlElement node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            TResult result = DefaultVisit(node, argument);
            result = DefaultVisitAttributes(node, argument, result);
            result = DefaultVisitChildNodes(node, argument, result);

            return result;
        }

        public virtual TResult VisitAttribute(HtmlAttribute node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            TResult result = DefaultVisit(node, argument);
            return default(TResult);
        }

        protected virtual TResult DefaultVisit(HtmlNode node, TArgument argument) {
            return default(TResult);
        }

        public virtual TResult VisitComment(HtmlComment node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            return DefaultVisit(node, argument);
        }

        public virtual TResult VisitText(HtmlText node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            return DefaultVisit(node, argument);
        }

        public virtual TResult VisitDocumentType(HtmlDocumentType node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            return DefaultVisit(node, argument);
        }

        public virtual TResult VisitDocumentFragment(HtmlDocumentFragment node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            TResult result = DefaultVisit(node, argument);
            result = DefaultVisitChildNodes(node, argument, result);
            return result;
        }

        public virtual TResult VisitDocument(HtmlDocument node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            TResult result = DefaultVisit(node, argument);
            result = DefaultVisitChildNodes(node, argument, result);

            return result;
        }

        protected virtual TResult DefaultVisitChildNodes(
            HtmlNode node, TArgument argument, TResult result) {

            foreach (var element in node.ChildNodes) {
                result = element.AcceptVisitor(this, argument);
            }
            return result;
        }

        protected virtual TResult DefaultVisitAttributes(
            HtmlNode node, TArgument argument, TResult result) {

            foreach (var attr in node.Attributes) {
                result = this.VisitAttribute(attr, argument);
            }

            return result;
        }

        public virtual TResult VisitProcessingInstruction(HtmlProcessingInstruction node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            return DefaultVisit(node, argument);
        }

        public virtual TResult VisitEntityReference(HtmlEntityReference node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            return DefaultVisit(node, argument);
        }

        public virtual TResult VisitCDataSection(HtmlCDataSection node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            return DefaultVisit(node, argument);
        }
    }
}
