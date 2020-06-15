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

using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Html {

    public class HtmlDocumentTests {

        [Fact]
        public void AppendElement_defaults_to_HTML_element() {
            Assert.IsInstanceOf<HtmlElement>(
                new HtmlDocument().AppendElement("x")
            );

            Assert.IsInstanceOf<HtmlElement>(
                new HtmlDocument().WithSchema(new HtmlSchema()).AppendElement("x")
            );
        }

        [Fact]
        public void NodeFactory_creates_HTML_elements() {
            Assert.IsInstanceOf<HtmlElement>(
                new HtmlDocument().NodeFactory.CreateElement(DomName.Create("any"))
            );
        }

        [Fact]
        public void NodeFactory_creates_HTML_text() {
            Assert.IsInstanceOf<HtmlText>(
                new HtmlDocument().NodeFactory.CreateText()
            );
        }

        [Fact]
        public void LoadHtml_should_parse_document() {
            var document = new HtmlDocument();
            document.LoadHtml("<p> </p>");
            Assert.Equal(
                "<html> <head></head> <body> <p> </p> </body> </html>",
                TextUtil.CompressWhitespace(document.OuterHtml)
            );
        }

        [Fact]
        public void Load_StreamContext_should_parse_document() {
            var document = new HtmlDocument();
            document.Load(StreamContext.FromText("<p> </p>"));
            Assert.Equal(
                "<html> <head></head> <body> <p> </p> </body> </html>",
                TextUtil.CompressWhitespace(document.OuterHtml)
            );
        }
    }
}
