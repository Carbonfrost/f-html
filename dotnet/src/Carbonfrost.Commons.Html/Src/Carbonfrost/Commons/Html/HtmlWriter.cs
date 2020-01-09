//
// - HtmlWriter.cs -
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
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    public abstract partial class HtmlWriter : DisposableObject {

        public void WriteAttributeString(
            string prefix, string localName, string ns, string value) {

            // TODO Handling of namespaces - This method is just for
            // compatibility with XmlWriter, but technically, namespaces
            // aren't automatic in HTML DOM
            this.WriteStartAttribute(prefix, localName, ns);
            this.WriteString(value);
            this.WriteEndAttribute();
        }

        public void WriteAttributeString(string localName, string value) {
            this.WriteStartAttribute(localName);
            this.WriteString(value);
            this.WriteEndAttribute();
        }

        public void WriteAttributeString(string localName, string ns, string value) {
            this.WriteStartAttribute(localName);
            this.WriteString(value);
            this.WriteEndAttribute();
        }

        public abstract void WriteString(string value);
        public abstract void WriteEndAttribute();

        public void WriteNode(HtmlNode node) {
            if (node == null)
                throw new ArgumentNullException("node");
            throw new NotImplementedException();
        }

        public void WriteStartElement(string element) {
            throw new NotImplementedException();
        }

        public void WriteElement(string element) {
            WriteStartElement(element);
            WriteEndElement();
        }

        public void WriteElement(string element,
                                 IEnumerable<KeyValuePair<string, string>> attributes) {
            WriteStartElement(element);
            if (attributes != null) {
                foreach (var a in attributes)
                    WriteAttributeString(a.Key, a.Value);
            }

            WriteEndElement();
        }

        public virtual void WriteStartAttribute(string localName) {
            WriteStartAttribute(null, localName, null);
        }

        public virtual void WriteStartAttribute(string localName, string ns) {
            WriteStartAttribute(null, localName, ns);
        }

        public abstract void WriteStartAttribute(string prefix, string localName, string ns);
        public abstract void WriteStartElement(Tag tag);
        public abstract void WriteEndElement();

        public void WriteElement(HtmlElement element) {
            if (element == null)
                throw new ArgumentNullException("element");
            throw new NotImplementedException();
        }

        public void Close() {
            Dispose();
        }

    }

}
