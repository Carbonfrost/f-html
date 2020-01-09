//
// - LinkedHashMap.cs -
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    class LinkedHashMap<TKey, TValue> : IDictionary<TKey, TValue> {

        private readonly LinkedList<KeyValuePair<TKey, TValue>> values;
        private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> map;

        public LinkedHashMap() {
            this.values = new LinkedList<KeyValuePair<TKey, TValue>>();
            this.map = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
        }

        public LinkedHashMap(int capacity) {
            this.values = new LinkedList<KeyValuePair<TKey, TValue>>();
            this.map= new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity);
        }

        bool TryGetValueExtension(TKey key,
                                  Action<LinkedListNode<KeyValuePair<TKey, TValue>>> action,
                                  out TValue value) {
            LinkedListNode<KeyValuePair<TKey, TValue>> node;
            if (map.TryGetValue(key, out node)) {
                value = node.Value.Value;

                if (action != null) {
                    action(node);
                }
                return true;
            }

            value = default(TValue);
            return true;
        }

        public TValue this[TKey key] {
            get {
                return map[key].Value.Value;
            }
            set {
                var kvp = new KeyValuePair<TKey, TValue>(key, value);
                if (map.ContainsKey(key)) {
                    map[key].Value = kvp;
                } else {
                    map.Add(key, values.AddLast(kvp));
                }
            }
        }

        public ICollection<TKey> Keys {
            get {
                return new KeyCollection(this);
            }
        }

        public ICollection<TValue> Values {
            get {
                return new ValueCollection(this);
            }
        }

        public int Count {
            get {
                return map.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return false;
            }
        }

        public bool ContainsKey(TKey key) {
            return map.ContainsKey(key);
        }

        public bool ContainsValue(TValue item) {
            return this.values.Any(t => object.Equals(item, t.Value));
        }

        public void Add(TKey key, TValue value) {
            map.Add(key, this.values.AddLast(new KeyValuePair<TKey, TValue>(key, value)));
        }

        public bool Remove(TKey key) {
            TValue dummy;
            return TryGetValueExtension(
                key,
                (node) => {
                    map.Remove(key);
                    values.Remove(node);
                },
                out dummy);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return TryGetValueExtension(
                key,
                null,
                out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        public void Clear() {
            this.values.Clear();
            this.map.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            TValue value;
            return this.TryGetValue(item.Key, out value)
                && object.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            this.values.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            TValue dummy;
            bool result = false;

            if (TryGetValueExtension(
                item.Key,
                (node) => {
                    if (object.Equals(item.Value, node.Value.Value)) {
                        map.Remove(item.Key);
                        values.Remove(item);
                        result = true;
                    }
                },
                out dummy)) {
                return result;
            }

            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        sealed class KeyCollection : ICollection<TKey> {

            private readonly LinkedHashMap<TKey, TValue> items;

            public int Count {
                get {
                    return this.items.Count;
                }
            }

            public bool IsReadOnly {
                get { return false; }
            }

            public void Clear() {
                this.items.Clear();
            }

            public KeyCollection(LinkedHashMap<TKey, TValue> items) {
                this.items = items;
            }

            public void Add(TKey item) {
                throw Failure.ReadOnlyCollection();
            }

            public bool Contains(TKey item) {
                return items.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex) {
                this.ToArray().CopyTo(array, arrayIndex);
            }

            public bool Remove(TKey item) {
                return items.Remove(item);
            }

            public IEnumerator<TKey> GetEnumerator() {
                return this.items.values.Select(t => t.Key).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }

        sealed class ValueCollection : ICollection<TValue> {

            private readonly LinkedHashMap<TKey, TValue> items;

            public int Count {
                get {
                    return this.items.Count;
                }
            }

            public bool IsReadOnly {
                get { return false; }
            }

            public void Clear() {
                this.items.Clear();
            }

            public ValueCollection(LinkedHashMap<TKey, TValue> items) {
                this.items = items;
            }

            public void Add(TValue item) {
                throw Failure.ReadOnlyCollection();
            }

            public bool Contains(TValue item) {
                return items.ContainsValue(item);
            }

            public void CopyTo(TValue[] array, int arrayIndex) {
                this.ToArray().CopyTo(array, arrayIndex);
            }

            public bool Remove(TValue item) {
                throw new NotSupportedException();
            }

            public IEnumerator<TValue> GetEnumerator() {
                return this.items.values.Select(t => t.Value).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }

    }
}
