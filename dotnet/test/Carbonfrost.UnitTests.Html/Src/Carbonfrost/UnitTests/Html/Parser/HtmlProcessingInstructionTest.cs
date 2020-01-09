//
// - HtmlProcessingInstructionTest.cs -
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
using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.Html {

    public class HtmlProcessingInstructionTest {

        [Fact]
        public void parse_document_into_processing_instruction() {
            string html = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
            HtmlProcessingInstruction pi = HtmlDocument.Parse(html).ChildNode(0)
                as HtmlProcessingInstruction;

            Assert.NotNull(pi);
            Assert.Equal("xml", pi.Target);
            Assert.Equal("version=\"1.0\" encoding=\"UTF-8\"", pi.Data);
            Assert.Equal("xml version=\"1.0\" encoding=\"UTF-8\"", pi.TextContent);
        }

        [Fact]
        public void split_target_and_data_nominal() {
            string h = "xml version=\"1.0\" encoding=\"UTF-8\"";
            HtmlProcessingInstruction pi = new HtmlProcessingInstruction(h, null);
            Assert.Equal("xml", pi.Target);
            Assert.Equal("version=\"1.0\" encoding=\"UTF-8\"", pi.Data);
            Assert.Equal("xml version=\"1.0\" encoding=\"UTF-8\"", pi.TextContent);
        }

        [Fact]
        public void split_target_and_data_no_data() {
            string h = "xmlversion=\"1.0\"encoding=\"UTF-8\"";
            HtmlProcessingInstruction pi = new HtmlProcessingInstruction(h, null);
            Assert.Equal("xmlversion=\"1.0\"encoding=\"UTF-8\"", pi.Target);
            Assert.Equal("", pi.Data);
        }

        [Fact]
        public void should_get_node_name() {
            string h = "xml version=\"1.0\" encoding=\"UTF-8\"";
            HtmlProcessingInstruction pi = new HtmlProcessingInstruction(h, null);
            Assert.Equal("xml", pi.NodeName);
        }

    }
}
