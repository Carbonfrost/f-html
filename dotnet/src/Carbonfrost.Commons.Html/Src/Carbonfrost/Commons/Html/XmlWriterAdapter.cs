//
// - XmlWriterAdapter.cs -
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
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Carbonfrost.Commons.Html {

    sealed class XmlWriterAdapter : HtmlWriter {

        // TODO Implement XML writer adapter

        private XmlWriter writer;

        public XmlWriterAdapter(XmlWriter writer) {
            this.writer = writer;
        }

        // `HtmlWriter' overrides
        public override void WriteString(string value) {
            throw new NotImplementedException();
        }

        public override void WriteStartElement(Tag tag) {
            throw new NotImplementedException();
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns) {
            throw new NotImplementedException();
        }

        public override void WriteEndElement() {
            throw new NotImplementedException();
        }

        public override void WriteEndAttribute() {
            throw new NotImplementedException();
        }

    }
}
