//
// - HtmlElement.InnerDataVisitor.cs -
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

namespace Carbonfrost.Commons.Html {

    partial class HtmlElement {

        sealed class InnerDataVisitor : HtmlNodeVisitor {

            private StringBuilder text;

            public InnerDataVisitor(HtmlElement e) {
                this.text = new StringBuilder();

                Visit(e);
            }

            public override void VisitText(HtmlText node) {
                if (node.IsData)
                    text.Append(node.RawText);
            }

            public override string ToString() {
                return text.ToString();
            }

        }
    }

}
