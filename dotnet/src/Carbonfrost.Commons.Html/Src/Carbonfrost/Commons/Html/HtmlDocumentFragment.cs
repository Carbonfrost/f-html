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
using System.Linq;
using System.Text;
using Carbonfrost.Commons.Web.Dom;
using HtmlParser = Carbonfrost.Commons.Html.Parser.Parser;

namespace Carbonfrost.Commons.Html {

    public class HtmlDocumentFragment : DomDocumentFragment, IHtmlNode {

        public string OuterHtml {
            get {
                return InnerHtml;
            }
            set {
                InnerHtml = value;
            }
        }

        public string InnerHtml {
            get {
                StringBuilder accum = new StringBuilder();
                var v = new OuterHtmlNodeVisitor(accum);
                foreach (var child in ChildNodes) {
                    v.Visit(child);
                }

                return accum.ToString().Trim();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override DomNodeType NodeType {
            get {
                return DomNodeType.DocumentFragment;
            }
        }

        // TODO Remove context if possible

        public static HtmlDocumentFragment Parse(string html,
                                                 HtmlElement context,
                                                 Uri baseUri) {

            HtmlDocumentFragment result = new HtmlDocumentFragment {
                BaseUri = baseUri,
            };
            result.Append(
                HtmlParser.ParseFragment(html, context, baseUri).ToList()
            );
            return result;
        }
    }
}
