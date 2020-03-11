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

using System.Text;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    partial class HtmlElement {

        sealed class InnerTextVisitor : HtmlNodeVisitor {

            private StringBuilder _text;

            public InnerTextVisitor(HtmlElement e) {
                _text = new StringBuilder();
                Visit(e);
            }

            protected override void VisitText(HtmlText node) {
                if (!node.IsData) {
                    bool preserveWhitespace = ((HtmlElement) node.ParentElement).PreserveWhitespace;
                    StringUtil.AppendNormalisedText(_text, node, preserveWhitespace);
                }
            }

            protected override void VisitText(DomText node) {
                if (node is HtmlText html) {
                    VisitText(html);
                    return;
                }
                _text.Append(node.Data);
            }

            protected override void VisitElement(HtmlElement node) {
                StringUtil.AppendWhitespaceIfBr(node, _text);

                if (_text.Length > 0 && node.IsBlock && !HtmlText.LastCharIsWhitespace(_text))
                    _text.Append(" ");

                base.VisitElement(node);
            }

            public override string ToString() {
                return _text.ToString();
            }

        }
    }

}
