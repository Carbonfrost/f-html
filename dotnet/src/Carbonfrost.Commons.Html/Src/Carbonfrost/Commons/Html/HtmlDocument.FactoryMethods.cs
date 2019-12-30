//
// - HtmlDocument.FactoryMethods.cs -
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

namespace Carbonfrost.Commons.Html {

    partial class HtmlDocument {

        public HtmlElement CreateElement(string tagName) {
            return new HtmlElement(Tag.ValueOf(tagName), this.BaseUri);
        }

        public HtmlText CreateText(string text) {
            return new HtmlText(text, null, false);
        }

        public HtmlDocumentType CreateDocumentType(string name,
                                                   string publicId,
                                                   string systemId,
                                                   Uri baseUri) {
            return new HtmlDocumentType(name,
                                        publicId,
                                        systemId,
                                        baseUri);
        }
    }
}
