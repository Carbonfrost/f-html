//
// - HtmlWriter.Static.cs -
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
using System.Text;
using System.Xml;

namespace Carbonfrost.Commons.Html {

	partial class HtmlWriter {

        public static HtmlWriter FromXmlWriter(XmlWriter xmlWriter) {
            if (xmlWriter == null)
                throw new ArgumentNullException("xmlWriter");

            return new XmlWriterAdapter(xmlWriter);
        }

        public static HtmlWriter Create(string fileName) {
            return Create(fileName, null);
        }

        public static HtmlWriter Create(TextWriter output) {
            return Create(output, null);
        }

        public static HtmlWriter Create(Stream output) {
            return Create(output, null);
        }

        // TODO Support HtmlWriter static methods

        public static HtmlWriter Create(string fileName, HtmlWriterSettings settings) {
            throw new NotImplementedException();
        }

        public static HtmlWriter Create(TextWriter output, HtmlWriterSettings settings) {
            throw new NotImplementedException();
        }

        public static HtmlWriter Create(Stream output, HtmlWriterSettings settings) {
            throw new NotImplementedException();
        }

	}
}
