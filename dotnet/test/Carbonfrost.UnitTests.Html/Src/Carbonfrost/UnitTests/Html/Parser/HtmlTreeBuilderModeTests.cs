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

using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Html;

namespace Carbonfrost.UnitTests.Html {

    public class HtmlTreeBuilderModeTests {

        public IEnumerable<string> ExpectedOperators {
            get {
                return new [] {
                    "op_Equality",
                    "op_Inequality",
                };
            }
        }

        public IEnumerable<string> Names {
            get {
                return new [] {
                    "Xml",
                    "Html5"
                };
            }
        }

        public HtmlTreeBuilderMode[] Values {
            get {
                return new [] {
                    HtmlTreeBuilderMode.Html5,
                    HtmlTreeBuilderMode.Xml,
                };
            }
        }

        [Theory]
        [PropertyData(nameof(ExpectedOperators))]
        public void Equals_operators_should_be_present(string name) {
            Assert.NotNull(
                typeof(HtmlTreeBuilderMode).GetMethod(name)
            );
        }

        [Fact]
        public void Values_each_are_unique() {
            Assert.HasCount(
                Values.Length,
                HtmlTreeBuilderMode.GetValues().Select(t => t.ToInt32()).Distinct()
            );
        }

        [Fact]
        public void GetNames_should_have_correct_values() {
            Assert.SetEqual(
                Names,
                HtmlTreeBuilderMode.GetNames()
            );
        }

        [Fact]
        public void GetValues_should_have_correct_values() {
            Assert.SetEqual(
                Values,
                HtmlTreeBuilderMode.GetValues()
            );
        }

        [Theory]
        [PropertyData(nameof(Names))]
        public void GetName_should_produce_the_right_names(string name) {
            var value = (HtmlTreeBuilderMode) typeof(HtmlTreeBuilderMode).GetField(name).GetValue(null);
            Assert.Equal(
                name,
                HtmlTreeBuilderMode.GetName(value)
            );
        }

        [Theory]
        [PropertyData(nameof(Names))]
        public void Parse_should_roundtrip_on_values(string name) {
            var expected = typeof(HtmlTreeBuilderMode).GetField(name).GetValue(null);
            Assert.Equal(
                expected,
                HtmlTreeBuilderMode.Parse(name)
            );
        }

        [Fact]
        public void TryParse_should_return_false_on_unknown_value() {
            Assert.False(
                HtmlTreeBuilderMode.TryParse("Unknown", out _)
            );
        }

        [Fact]
        public void Html5_is_the_default_value() {
            Assert.Equal(
                HtmlTreeBuilderMode.Html5,
                default(HtmlTreeBuilderMode)
            );
        }
    }

}
