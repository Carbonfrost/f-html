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

using System.Text;
using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.Html {

    public class HtmlWriterSettingsTests {

        [Fact]
        public void Clone_should_copy_properties() {
            var s = new HtmlWriterSettings {
                IsXhtml = true,
                Charset = Encoding.UTF8,
                EscapeMode = EscapeMode.Extended,
            };
            var clone = s.Clone();
            Assert.True(clone.IsXhtml);
            Assert.Equal(s.Charset, clone.Charset);
            Assert.Equal(s.EscapeMode, clone.EscapeMode);
        }
    }
}
