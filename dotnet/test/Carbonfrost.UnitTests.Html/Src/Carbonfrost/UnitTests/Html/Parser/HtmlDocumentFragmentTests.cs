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

using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Html {

    public class HtmlDocumentFragmentTests {

        [Fact]
        public void script() {
            HtmlDocument doc2 = new HtmlDocument();
            var e = doc2.CreateElement("div");

            HtmlDocumentFragment doc = HtmlDocumentFragment.Parse(
                "<script>\"hello\"</script>", (HtmlElement) e, null);
            Assert.Equal("script", doc.ChildNode(0).NodeName);

            Assert.Equal(DomNodeType.Text, doc.ChildNode(0).ChildNode(0).NodeType);

            Assert.True(((HtmlText) doc.ChildNode(0).ChildNode(0)).IsData);
        }
    }
}
