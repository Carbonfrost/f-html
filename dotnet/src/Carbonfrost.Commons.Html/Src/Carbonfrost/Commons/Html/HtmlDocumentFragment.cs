//
// - HtmlDocumentFragment.cs -
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
using HtmlParser = Carbonfrost.Commons.Html.Parser.Parser;

namespace Carbonfrost.Commons.Html {

    public class HtmlDocumentFragment : HtmlElement {

        public HtmlDocumentFragment(Uri baseUri)
            : base(Tag.ValueOf(NodeNames.Document), baseUri) {
        }

        public override string NodeName {
            get {
                return NodeNames.DocumentFragment;
            }
        }

        public override string OuterHtml {
            get {
                return this.InnerHtml; // no outer wrapper tag
            }
        }

        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.DocumentFragment;
            }
        }

        // TODO Remove context if possible

        public static HtmlDocumentFragment Parse(string html,
                                                 HtmlElement context,
                                                 Uri baseUri) {

            HtmlDocumentFragment result = new HtmlDocumentFragment(baseUri);
            result.AddChildren(HtmlParser.ParseFragment(html, context, baseUri));
            return result;
        }

        public override TResult AcceptVisitor<TArgument, TResult>(HtmlNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitDocumentFragment(this, argument);
        }

        public override void AcceptVisitor(HtmlNodeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            visitor.VisitDocumentFragment(this);
        }
    }
}
