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

using System.Linq;
using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Html {

    public class HtmlDocumentFragmentTests {

        [Fact]
        public void AppendElement_defaults_to_HTML_element() {
            Assert.IsInstanceOf<HtmlElement>(
                new HtmlDocument().AppendElement("x")
            );
        }

        [Fact]
        public void Parse_should_handle_script_as_data() {
            HtmlDocument doc2 = new HtmlDocument();
            HtmlDocumentFragment doc = HtmlDocumentFragment.Parse(
                "<script>\"hello\"</script>", null
            );
            Assert.Equal("script", doc.ChildNode(0).NodeName);

            Assert.Equal(DomNodeType.Text, doc.ChildNode(0).ChildNode(0).NodeType);

            Assert.True(((HtmlText) doc.ChildNode(0).ChildNode(0)).IsData);
        }

        [Fact]
        public void Parse_should_support_several_nodes() {
            var settings = new HtmlReaderSettings {
                Mode = HtmlTreeBuilderMode.Xml,
            };
            var xml = "<a/><b/><c hello=\"world\"/>";
            var frag = HtmlDocumentFragment.Parse(xml, settings);
            Assert.HasCount(3, frag.ChildNodes);
            Assert.Equal(new [] { "a", "b", "c" }, frag.ChildNodes.Select(n => n.NodeName));
        }

        [Fact]
        public void InnerHtml_should_replace_contents() {
            var frag = new HtmlDocumentFragment();
            frag.InnerHtml = "<div><br><br></div>";
            Assert.HasCount(1, frag.ChildNodes);
            Assert.Equal("<div>\n <br>\n <br>\n</div>", frag.InnerHtml);
        }

        [Fact]
        public void OwnerDocument_initializes_with_Html5_by_default() {
            var frag = new HtmlDocumentFragment();
            Assert.NotNull(frag.OwnerDocument.Schema);
            Assert.Equal("html5", frag.OwnerDocument.Schema.LocalName);
        }
    }
}
