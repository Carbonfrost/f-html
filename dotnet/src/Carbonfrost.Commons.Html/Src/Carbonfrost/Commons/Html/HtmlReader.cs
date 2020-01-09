//
// - HtmlReader.cs -
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
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    public abstract class HtmlReader : DisposableObject {

        // TODO Support HtmlReader implementation

        public abstract HtmlParseErrorCollection ParseErrors { get; }

        public HtmlDocument ReadDocument() {
            throw new NotImplementedException();
        }

        public void Close() {
            Dispose(true);
        }

        public void CopyTo(HtmlWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");
        }

        // TODO Consider support for TextReader, XmlReader overloads
        // (possibly prohibited due to encoding behavior)

        public static HtmlReader Create(StreamContext input) {
            return Create(input, null);
        }

        public static HtmlReader Create(Stream input) {
            return Create(input, null);
        }

        public static HtmlReader Create(string inputUri) {
            return Create(inputUri, null);
        }

        public static HtmlReader Create(Stream input, HtmlReaderSettings settings) {
            return Create(input, settings, null);
        }

        public static HtmlReader Create(StreamContext input, HtmlReaderSettings settings) {
            if (input == null)
                throw new ArgumentNullException("input");

            return Create(input.OpenRead(), settings, input.Uri);
        }

        public static HtmlReader Create(string inputUri, HtmlReaderSettings settings) {
            throw new NotImplementedException();
        }

        public static HtmlReader Create(Stream input, HtmlReaderSettings settings, Uri baseUri) {
            throw new NotImplementedException();
        }

    }
}
