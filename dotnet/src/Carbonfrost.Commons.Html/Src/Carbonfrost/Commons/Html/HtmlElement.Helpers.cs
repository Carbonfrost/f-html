//
// - HtmlElement.Helpers.cs -
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Carbonfrost.Commons.Html.Query;

namespace Carbonfrost.Commons.Html {

    partial class HtmlElement {

        public string ClassName {
            get {
                return this.Attribute("class");
            }
            set {
                this.Attribute("class", value);
            }
        }

        public HtmlClassList ClassNames {
            get {
                return new HtmlClassList(this);
            }
        }

        public HtmlElementQuery Query {
            get {
                return new HtmlElementQuery(TrivialEnumerator());
            }
        }

        // TODO Generalize to any element (ie, if element allows value attribute,
        // otherwise use inner text)

        public string Value() {
            if (this.Tag.Name.Equals("textarea"))
                return this.InnerText;
            else
                return this.Attribute("value");
        }

        public HtmlElement Value(string value) {
            if (this.Tag.Name.Equals("textarea"))
                this.InnerText = value;
            else
                this.Attribute("value", value);

            return this;
        }

        public new HtmlElement Attribute(string name, string value) {
            return (HtmlElement) base.Attribute(name, value);
        }

        public bool HasClass(string className) {
            return this.ClassNames.Contains(className);
        }

        public HtmlElement AddClass(string className) {
            if (className == null)
                return this;

            ClassNames.Add(className);
            return this;
        }

        public HtmlElement RemoveClass(string className) {
            if (className == null)
                return this;

            ClassNames.Remove(className);
            return this;
        }

        public HtmlElement ToggleClass(string className) {
            if (className == null)
                return this;

            ClassNames.Toggle(className);
            return this;
        }

        public HtmlElement AppendChild(HtmlNode child) {
            if (child == null)
                throw new ArgumentNullException("child");

            AddChildren(child);
            return this;
        }

        public HtmlElement PrependChild(HtmlNode child) {
            if (child == null)
                throw new ArgumentNullException("child");

            AddChildren(0, child);
            return this;
        }

        public HtmlElement AppendElement(string tag) {
            HtmlElement child = new HtmlElement(GetTag(tag), BaseUri);
            AppendChild(child);
            return child;
        }

        public HtmlElement PrependElement(string tag) {
            HtmlElement child = new HtmlElement(GetTag(tag), BaseUri);
            PrependChild(child);
            return child;
        }

        public new HtmlElement Empty() {
            base.Empty();
            return this;
        }

        private IEnumerable<HtmlElement> TrivialEnumerator() {
            yield return this;
        }

        // TODO Consider resurrecting additional dom methods: ByClass, etc.
        public virtual HtmlElementQuery GetElementsByTag(string tag) {
            if (tag == null)
                throw new ArgumentNullException("tag");

            string tagName = tag.ToLower();
            return Collector.Collect(new Evaluator.Tag(tagName), this);
        }

        public HtmlElementQuery GetAllElements() {
            return Collector.Collect(new Evaluator.AllElements(), this);
        }

        public virtual HtmlElement GetElementById(string id) {
            if (id == null)
                throw new ArgumentNullException("id");

            return Collector.Collect(new Evaluator.Id(id), this).FirstOrDefault();
        }
    }

}
