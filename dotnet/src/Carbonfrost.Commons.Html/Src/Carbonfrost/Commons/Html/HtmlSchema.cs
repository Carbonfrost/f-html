//
// Copyright 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    public class HtmlSchema : DomSchema {

        internal static readonly HtmlSchema Html5 = new HtmlSchema();
        private const string HTML5_NAME = "html5";

        public HtmlSchema() : this(HTML5_NAME) {}
        public HtmlSchema(string name) : base(name) {
            NodeTypeProvider = HtmlNodeTypeProvider.Instance;
            if (name == HTML5_NAME) {
                AttributeDefinitions.Add(
                    new HtmlAttributeDefinition("class") {
                        ValueType = typeof(DomStringTokenList)
                    }
                );
                Html5TagLibrary.CopyTo(ElementDefinitions);
            }
        }

        internal HtmlElementDefinition GetTag(string tagName) {
            if (tagName == null)
                throw new ArgumentNullException("tagName");

            tagName = tagName.Trim().ToLowerInvariant();
            if (tagName.Length == 0)
                throw Failure.AllWhitespace("tagName");

            // TODO This behavior causes any undefined tag to be
            // added to HTML 5 (need different behavior)

            var tags = this.ElementDefinitions;
            lock (tags) {
                var tag = (HtmlElementDefinition) tags[tagName];
                if (tag == null) {
                    // not defined: create default; go anywhere, do anything! (incl be inside a <p>)
                    tag = new HtmlElementDefinition(tagName);
                    tags.Add(tag);
                    tag.IsUnknownTag = true;
                    tag.IsBlock = false;
                    tag.CanContainBlock = true;
                }
                return tag;
            }
        }

        internal bool IsKnownTag(string tagName) {
            if (tagName == null)
                throw new ArgumentNullException("tagName");
            if (tagName.Length == 0)
                throw Failure.EmptyString("tagName");

            var tag = ElementDefinitions[tagName] as HtmlElementDefinition;
            return tag != null && !tag.IsUnknownTag;
        }

        public new HtmlAttributeDefinition GetAttributeDefinition(string name) {
            return (HtmlAttributeDefinition) base.GetDomAttributeDefinition(name);
        }

        public new HtmlElementDefinition GetElementDefinition(string name) {
            return (HtmlElementDefinition) base.GetDomElementDefinition(name);
        }
    }

    partial class Extensions {

        internal static HtmlSchema FindSchema(this DomObject node) {
            if (node.OwnerDocument != null
                && node.OwnerDocument.Schema != null
                && node.OwnerDocument.Schema is HtmlSchema hs) {
                return hs;
            }
            return HtmlSchema.Html5;
        }
    }
}
