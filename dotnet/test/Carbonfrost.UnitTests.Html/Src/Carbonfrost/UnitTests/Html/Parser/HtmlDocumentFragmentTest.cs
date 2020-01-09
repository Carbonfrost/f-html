//
// - HtmlDocumentFragmentTest.cs -
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
using System.Linq;
using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.Html {

    public class HtmlDocumentFragmentTest {

        [Fact]
        public void script() {

            // HtmlDocument doc = HtmlDocument.ParseXml("<script>\"hello\"</script>", null);
            HtmlDocument doc2 = new HtmlDocument();
            HtmlElement e = doc2.CreateElement("div");

            HtmlDocumentFragment doc = HtmlDocumentFragment.Parse(
                "<script>\"hello\"</script>", e, null);
            Assert.Equal("script", doc.ChildNode(0).NodeName);

            Assert.Equal(HtmlNodeType.Text, doc.ChildNode(0).ChildNode(0).NodeType);

            Assert.True(((HtmlText) doc.ChildNode(0).ChildNode(0)).IsData);
        }
    }
}
