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

using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.Html {

    public class HtmlSchemaTests {

        [Fact]
        public void Constructor_creates_HTML5_by_default() {
            Assert.Equal("html5", new HtmlSchema().Name);
        }

        [Fact]
        public void GetElementDefinition_gets_expected_definition() {
            var schema = new HtmlSchema();
            var definition = schema.GetElementDefinition("br");
            Assert.True(definition.IsSelfClosing);
            Assert.True(definition.IsEmpty);
        }

        [Theory]
        [InlineData("br", Name = "for known elements")]
        [InlineData("xxx", Name = "for unknown elements")]
        public void GetElementNodeType_is_correct_value(string name) {
            var schema = new HtmlSchema();
            Assert.Equal(
                typeof(HtmlElement),
                schema.GetElementNodeType(name)
            );
        }
    }
}
