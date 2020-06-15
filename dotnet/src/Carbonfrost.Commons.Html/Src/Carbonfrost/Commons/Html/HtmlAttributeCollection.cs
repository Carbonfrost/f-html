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

using System;
using System.Collections;
using System.Collections.Generic;

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    class HtmlAttributeCollection : IEnumerable<HtmlAttribute> {

        // linked hash map to preserve insertion order.
        // null be default as so many elements have no attributes -- saves a good chunk of memory

        private readonly LinkedList<HtmlAttribute> _values = new LinkedList<HtmlAttribute>();
        private readonly Dictionary<string, HtmlAttribute> _map
            = new Dictionary<string, HtmlAttribute>(StringComparer.OrdinalIgnoreCase);

        public string this[string name] {
            get {
                if (name == null) {
                    throw new ArgumentNullException(nameof(name));
                }
                if (name.Length == 0) {
                    throw Failure.EmptyString(nameof(name));
                }

                if (_map.TryGetValue(name, out HtmlAttribute attr)) {
                    return attr.Value;
                }

                return string.Empty;
            }
            set {
                var attr = GetValueOrDefault(name);
                attr.Value = value;
            }
        }

        private HtmlAttribute GetValueOrDefault(string name) {
            if (_map.TryGetValue(name, out var node)) {
                return node;
            }

            HtmlAttribute attr = new HtmlAttribute(name, "");
            _values.AddLast(attr);
            _map.Add(name, attr);
            return attr;
        }

        public bool Contains(string name) {
            return _map.ContainsKey(name);
        }

        public IEnumerator<HtmlAttribute> GetEnumerator() {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(HtmlAttribute item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }
            _map.Add(item.LocalName, item);
            _values.AddLast(item);
        }
    }
}
