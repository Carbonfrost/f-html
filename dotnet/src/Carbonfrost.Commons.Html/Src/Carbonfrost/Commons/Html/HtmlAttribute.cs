//
// - HtmlAttribute.cs -
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
using System.Text;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    public class HtmlAttribute : HtmlNode, IEquatable<HtmlAttribute> {

        const string dataPrefix = "data-";

        private string name;
        private string value;

        public bool IsDataAttribute {
            get {
                return this.Name.StartsWith(dataPrefix)
                    && this.Name.Length > dataPrefix.Length;
            }
        }

        public override string OuterHtml {
            get {
                return string.Format(
                    "{0}={1}{2}{1}",
                    name,
                    '"',
                    HtmlEncoder.Escape(value));
            }
        }

        public override string TextContent {
            get { return Value; }
            set { Value = value; }
        }

        public string Name {
            get { return name; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (value.Length == 0)
                    throw Failure.EmptyString("value");

                this.name = value.Trim().ToLowerInvariant();
            }
        }

        // TODO VAlue could be null if the property is a boolean
        public string Value {
            get { return value; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.value = value;
            }
        }

        public HtmlAttribute(string name, string value) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw Failure.EmptyString("name");

            if (value == null)
                throw new ArgumentNullException("value");

            this.name = name.Trim().ToLowerInvariant();
            this.value = value;
        }

        internal void AppendHtml(StringBuilder sb, HtmlWriterSettings settings) {
            sb.Append(name)
                .Append("=\"")
                .Append(HtmlEncoder.Escape(value, settings.Charset.GetEncoder(), settings.EscapeMode))
                .Append("\"");
        }

        public override string ToString() {
            return this.OuterHtml;
        }

        public HtmlAttribute Clone() {
            return (HtmlAttribute) MemberwiseClone();
        }

        // 'object' overrides
        public override bool Equals(object obj)  {
            return StaticEquals(this, obj as HtmlAttribute);
        }

        public override int GetHashCode() {
            int hashCode = 0;
            unchecked {
                if (name != null)
                    hashCode += 1000000007 * name.GetHashCode();
                if (value != null)
                    hashCode += 1000000009 * value.GetHashCode();
            }

            return hashCode;
        }

        // `IEquatable' implementation
        public bool Equals(HtmlAttribute other) {
            return StaticEquals(this, other);
        }

        static bool StaticEquals(HtmlAttribute lhs, HtmlAttribute rhs) {
            if (object.ReferenceEquals(lhs, rhs))
                return true;
            else if (object.ReferenceEquals(lhs, null) || object.ReferenceEquals(rhs, null))
                return false;

            return lhs.name == rhs.name
                && lhs.value == rhs.value;
        }

        // `Node' overrides
        public override string NodeName {
            get {
                return NodeNames.Attribute;
            }
        }

        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.Attribute;
            }
        }

        public override TResult AcceptVisitor<TArgument, TResult>(HtmlNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitAttribute(this, argument);
        }

        public override void AcceptVisitor(HtmlNodeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            visitor.VisitAttribute(this);
        }
    }

}
