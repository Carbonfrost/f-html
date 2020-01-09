//
// - HtmlAttributeCollection.cs -
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    public class HtmlAttributeCollection : ICollection<HtmlAttribute> {

        // linked hash map to preserve insertion order.
        // null be default as so many elements have no attributes -- saves a good chunk of memory

        // TODO We probably should care about enforcing same document origin on HtmlAttributes that
        // are added

        // TODO This should be using case invariant comparer (and no longer need ToLowerInvariant)

        private LinkedHashMap<string, HtmlAttribute> attributes;

        public string this[string name] {
            get {
                if (name == null)
                    throw new ArgumentNullException("name");
                if (name.Length == 0)
                    throw Failure.EmptyString("name");

                if (attributes == null)
                    return string.Empty;

                HtmlAttribute attr = attributes.GetValueOrDefault(name.ToLowerInvariant());
                return attr != null ? attr.Value : string.Empty;
            }
            set {
                EnsureAttributes();

                HtmlAttribute attr = new HtmlAttribute(name, value);
                attributes[name] = attr;
            }
        }

        public HtmlAttributeCollection() {
        }

        public void Remove(string name) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw Failure.EmptyString("name");

            if (attributes == null)
                return;
            attributes.Remove(name.ToLowerInvariant());
        }

        public bool Contains(string name) {
            return attributes != null
                && attributes.ContainsKey(name.ToLowerInvariant());
        }

        internal void AddAll(HtmlAttributeCollection incoming) {
            if (incoming.Count == 0)
                return;

            EnsureAttributes();
            attributes.AddMany(incoming.attributes);
        }

        public IEnumerator<HtmlAttribute> GetEnumerator() {
            if (this.attributes == null)
                return Empty<HtmlAttribute>.List.GetEnumerator();

            return this.attributes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            if (attributes == null)
                return string.Empty;

            StringBuilder accum = new StringBuilder();
            foreach (var entry in attributes) {
                HtmlAttribute attribute = entry.Value;
                accum.Append(" ");
                attribute.AppendHtml(accum, HtmlWriterSettings.Default);
            }

             // output settings a bit funky, but this html() seldom used
            return accum.ToString();
        }

        public HtmlAttributeCollection Clone() {
            if (attributes == null)
                return new HtmlAttributeCollection();

            HtmlAttributeCollection clone = new HtmlAttributeCollection();
            clone.attributes = new LinkedHashMap<string, HtmlAttribute>(attributes.Count);

            foreach (HtmlAttribute attribute in this)
                clone.attributes[attribute.Name] = attribute.Clone();

            return clone;
        }


        public int Count {
            get {
                if (attributes == null)
                    return 0;

                return attributes.Count;
            }
        }

        bool ICollection<HtmlAttribute>.IsReadOnly {
            get {
                return false;
            }
        }

        public void Add(HtmlAttribute item) {
            if (item == null)
                throw new ArgumentNullException("item");

            EnsureAttributes();
            this.attributes.Add(item.Name, item);
        }

        public void Clear() {
            if (this.attributes != null)
                this.attributes.Clear();
        }

        public bool Contains(HtmlAttribute item) {
            return this.attributes.ContainsValue(item);
        }

        public void CopyTo(HtmlAttribute[] array, int arrayIndex) {
            if (array == null)
                throw new ArgumentNullException("array");

            if (this.attributes == null) {
                Empty<HtmlAttribute>.Array.CopyTo(array, arrayIndex);

            } else {
                this.attributes.Values.ToArray().CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(HtmlAttribute item) {
            if (item == null)
                throw new ArgumentNullException("item");

            return this.attributes.Remove(item.Name);
        }

        private void EnsureAttributes() {
            if (this.attributes == null)
                this.attributes = new LinkedHashMap<string, HtmlAttribute>(2);
        }

    }
}
