//
// Copyright 2012, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

// The MIT License
//
// Copyright (c) 2009, 2010, 2011, 2012 Jonathan Hedley <jonathan@hedley.net>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace Carbonfrost.Commons.Html.Parser {

    partial class Token {

        internal abstract class Tag : Token {

            private string tagName;
            private readonly HtmlAttributeCollection _attributes = new HtmlAttributeCollection(); // TODO: allow nodes to not have attributes
            internal bool selfClosing = false;

            private string pendingAttributeName;
            private string pendingAttributeValue;

            public bool IsSelfClosing {
                get {
                    return selfClosing;
                }
            }

            public HtmlAttributeCollection Attributes {
                get {
                    return _attributes;
                }
            }

            public string Name {
                get {
                    return tagName;
                }
                set {
                    this.tagName = value;
                }
            }

            public void NewAttribute() {
                if (pendingAttributeName != null) {
                    if (pendingAttributeValue == null)
                        pendingAttributeValue = string.Empty;

                    Attributes[pendingAttributeName] = pendingAttributeValue;
                }

                pendingAttributeName = null;
                pendingAttributeValue = null;
            }

            public void FinaliseTag() {
                // readonlyises for emit
                if (pendingAttributeName != null) {
                    // TODO: check if attribute name exists; if so, drop and error
                    NewAttribute();
                }
            }

            // these appenders are rarely hit in not null state-- caused by null chars.
            public void AppendTagName(string append) {
                Name = Name == null ? append : (Name + append);
            }

            public void AppendTagName(char append) {
                AppendTagName(append.ToString());
            }

            public void AppendAttributeName(string append) {
                if (pendingAttributeName == null) {
                    pendingAttributeName = append;

                } else {
                    pendingAttributeName = pendingAttributeName + append;
                }
            }

            public void AppendAttributeName(char append) {
                AppendAttributeName(append.ToString());
            }

            public void AppendAttributeValue(string append) {
                if (pendingAttributeValue == null) {
                    pendingAttributeValue = append;
                } else {
                    pendingAttributeValue = pendingAttributeValue + append;
                }
            }

            public void AppendAttributeValue(char append) {
                AppendAttributeValue(append.ToString());
            }

        }

    }

}
