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

using System;
using System.Collections.Generic;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Web.Dom;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.UnitTests.Html {

    public class HtmlProviderFactoryTests {

        public IEnumerable<Type> ProviderObjectTypes {
            get {
                return new [] {
                    typeof(HtmlElement),
                    typeof(HtmlProcessingInstruction),
                    typeof(HtmlDocument),
                    typeof(HtmlDocumentFragment),
                    typeof(HtmlText),
                    typeof(HtmlAttributeDefinition),
                    typeof(HtmlElementDefinition),
                    typeof(HtmlWriterSettings),
                    typeof(HtmlReaderSettings),
                };
            }
        }

        [Theory]
        [PropertyData(nameof(ProviderObjectTypes))]
        public void IsProviderObject_should_apply_to_known_types(Type type) {
            Assert.True(
                HtmlProviderFactory.Instance.IsProviderObject(type)
            );
        }

        [Theory]
        [PropertyData(nameof(ProviderObjectTypes))]
        public void ForProviderObject_should_lookup_HTML_provider(Type type) {
            Assert.IsInstanceOf<HtmlProviderFactory>(
                DomProviderFactory.ForProviderObject(type)
            );
        }

        [Theory]
        [InlineData(".html")]
        [InlineData(".htm")]
        [InlineData(".xhtml")]
        public void Providers_by_extension_should_find_HTML(string extension) {
            Assert.IsInstanceOf<HtmlProviderFactory>(
                DomProviderFactory.FromCriteria(new { Extension = extension })
            );
        }

        [Fact]
        public void FromName_should_find_provider() {
            Assert.IsInstanceOf<HtmlProviderFactory>(
                App.GetProvider<DomProviderFactory>("html")
            );
            Assert.IsInstanceOf<HtmlProviderFactory>(DomProviderFactory.FromName("html"));
        }

        [Fact]
        public void CreateDocument_creates_HTML_documents() {
            Assert.IsInstanceOf<HtmlDocument>(
                 HtmlProviderFactory.Instance.CreateDocument()
            );
        }

        [Fact]
        public void NodeFactory_for_default_document_creates_HTML_elements() {
            var nodeFactory = HtmlProviderFactory.Instance.CreateDocument().NodeFactory;

            Assert.Equal(
                typeof(HtmlElement),
                nodeFactory.GetElementNodeType("x")
            );

            Assert.IsInstanceOf<HtmlElement>(
                nodeFactory.CreateElement("x")
            );
        }
    }
}
