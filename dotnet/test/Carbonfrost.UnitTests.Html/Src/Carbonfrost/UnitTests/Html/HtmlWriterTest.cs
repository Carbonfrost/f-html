//
// - HtmlWriterTest.cs -
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
using System.IO;
using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.Html {

    [Skip]
	public class HtmlWriterTest {

        [Fact]
        public void writeEmptySelfClosingElement() {
            StringWriter sw = new StringWriter();
            HtmlWriter writer = HtmlWriter.Create(sw, null);
            writer.WriteStartElement("br");
            writer.WriteEndElement();

            // TODO Use fixtures to assert results:
            // <br>
        }


        [Fact]
        public void writeSelfClosingElement() {
            StringWriter sw = new StringWriter();
            HtmlWriter writer = HtmlWriter.Create(sw, null);
            writer.WriteElement("br");

            // <br>
        }

	}
}
