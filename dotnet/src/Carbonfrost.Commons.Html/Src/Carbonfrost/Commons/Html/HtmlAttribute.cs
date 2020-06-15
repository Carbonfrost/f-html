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
using System.Text;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    public class HtmlAttribute : DomAttribute<HtmlAttribute>, IHtmlObject {

        const string DATA_PREFIX = "data-";

        public bool IsDataAttribute {
            get {
                string name = LocalName;
                return name.StartsWith(DATA_PREFIX) && name.Length > DATA_PREFIX.Length;
            }
        }

        public new HtmlAttributeDefinition AttributeDefinition {
            get {
                return (HtmlAttributeDefinition) base.AttributeDefinition;
            }
        }

        protected override DomAttributeDefinition DomAttributeDefinition {
            get {
                return this.FindSchema().GetAttributeDefinition(Name);
            }
        }

        internal HtmlAttribute(string name) : base(name) {
        }

        internal HtmlAttribute(string name, string value) : base(name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (name.Length == 0) {
                throw Failure.EmptyString(nameof(name));
            }
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        internal void AppendHtml(StringBuilder sb, HtmlWriterSettings settings) {
            sb.Append(Name)
                .Append("=\"")
                .Append(HtmlEncoder.Escape(Value, settings.Charset.GetEncoder(), settings.EscapeMode))
                .Append("\"");
        }

        public new HtmlAttribute Clone() {
            return (HtmlAttribute) CloneCore();
        }

        protected override DomAttribute CloneCore() {
            return new HtmlAttribute(LocalName, Value);
        }
    }
}
