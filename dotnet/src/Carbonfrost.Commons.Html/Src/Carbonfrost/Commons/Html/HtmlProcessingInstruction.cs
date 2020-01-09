//
// - HtmlProcessingInstruction.cs -
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
using System.Text;
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Html.Parser;

namespace Carbonfrost.Commons.Html {

    public class HtmlProcessingInstruction : HtmlNode {

        static readonly Regex split = new Regex(@"\s+");
        private string textContent;
        private string[] parts;

        internal HtmlProcessingInstruction(Token.Comment commentToken, Uri baseUri)
            : this(commentToken.Data.Substring(1, commentToken.Data.Length - 2), baseUri) {
        }

         internal HtmlProcessingInstruction(string textContent, Uri baseUri) : base(baseUri) {
            this.TextContent = textContent ?? string.Empty;
        }

        public string Target {
            get {
                return parts[0];
            }
            set {
                parts[0] = (value ?? string.Empty).Trim();
                UpdateTextContent();
            }
        }

        public string Data {
            get {
                return parts[1];
            }
            set {
                parts[1] = (value ?? string.Empty).Trim();
                UpdateTextContent();
            }
        }

        public override string TextContent {
            get { return this.textContent; }
            set {
                this.textContent = (value ?? string.Empty).Trim();
                string[] results = split.Split(textContent, 2);

                if (results.Length < 2) {
                    Array.Resize(ref results, 2);
                    results[1] = string.Empty;
                }

                results[0] = results[0].Trim();
                results[1] = results[1].Trim();

                this.parts = results;
            }
        }

        public override string NodeName {
            get {
                return Target;
            }
        }

        public override string NodeValue {
            get {
                return Data;
            }
            set {
                Data = value;
            }
        }

        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.ProcessingInstruction;
            }
        }

        public override string ToString() {
            return OuterHtml;
        }

        public override TResult AcceptVisitor<TArgument, TResult>(HtmlNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitProcessingInstruction(this, argument);
        }

        public override void AcceptVisitor(HtmlNodeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            visitor.VisitProcessingInstruction(this);
        }

        private void UpdateTextContent() {
            this.TextContent = string.Concat(parts[0], ' ', parts[1]);
        }
    }

}
