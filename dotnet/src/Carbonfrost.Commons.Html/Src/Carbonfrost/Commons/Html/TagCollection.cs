//
// - TagCollection.cs -
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
using System.Collections;
using System.Collections.ObjectModel;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    public class TagCollection : IDictionary<string, Tag>, ICollection<Tag> {

        // }, IMakeReadOnly {

        private bool isReadOnly;
        private readonly Dictionary<string, Tag> _dictionary;

        public TagCollection() {
            _dictionary = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
        }

        public Tag this[string name] {
            get {
                return _dictionary.GetValueOrDefault(name);
            }
        }

        public int Count {
            get {
                return _dictionary.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return isReadOnly;
            }
        }

        private IDictionary<string, Tag> Dictionary {
            get {
                return _dictionary;
            }
        }

        public void Add(Tag tag) {
            ThrowIfReadOnly();
            _dictionary.Add(tag.Name, tag);
        }

        public void Clear() {
            ThrowIfReadOnly();
            _dictionary.Clear();
        }

        public bool Remove(string name) {
            return _dictionary.Remove(name);
        }

        public bool Remove(Tag tag) {
            if (tag == null) {
                throw new ArgumentNullException("tag");
            }
            ThrowIfReadOnly();
            Tag existing;
            return _dictionary.TryGetValue(tag.Name, out existing) && existing == tag
                && _dictionary.Remove(tag.Name);
        }

        public bool Contains(string name) {
            return _dictionary.ContainsKey(name);
        }

        public bool Contains(Tag tag) {
            if (tag == null) {
                throw new ArgumentNullException("tag");
            }
            Tag existing;
            return _dictionary.TryGetValue(tag.Name, out existing) && existing == tag;
        }

        public void CopyTo(Tag[] array, int arrayIndex) {
            _dictionary.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Tag> GetEnumerator() {
            return _dictionary.Values.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        // IDictionary implementation
        Tag IDictionary<string, Tag>.this[string key] {
            get { return this[key]; }
            set { throw new NotSupportedException(); }
        }

        ICollection<string> IDictionary<string, Tag>.Keys {
            get {
                return this.Dictionary.Keys;
            }
        }

        ICollection<Tag> IDictionary<string, Tag>.Values {
            get {
                return this.Dictionary.Values;
            }
        }

        bool ICollection<KeyValuePair<string, Tag>>.IsReadOnly {
            get {
                return isReadOnly;
            }
        }

        bool IDictionary<string, Tag>.ContainsKey(string key) {
            return this.Dictionary.ContainsKey(key);
        }

        void IDictionary<string, Tag>.Add(string key, Tag value) {
            throw new NotSupportedException();
        }

        bool IDictionary<string, Tag>.TryGetValue(string key, out Tag value) {
            return this.Dictionary.TryGetValue(key, out value);
        }

        void ICollection<KeyValuePair<string, Tag>>.Add(KeyValuePair<string, Tag> item) {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<string, Tag>>.Contains(KeyValuePair<string, Tag> item) {
            return Dictionary.Contains(item);
        }

        void ICollection<KeyValuePair<string, Tag>>.CopyTo(KeyValuePair<string, Tag>[] array, int arrayIndex) {
            Dictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, Tag>>.Remove(KeyValuePair<string, Tag> item) {
            throw new NotSupportedException();
        }

        IEnumerator<KeyValuePair<string, Tag>> IEnumerable<KeyValuePair<string, Tag>>.GetEnumerator() {
            return Dictionary.GetEnumerator();
        }

        internal void MakeReadOnly() {
            this.isReadOnly = true;
        }

        // bool IMakeReadOnly.IsReadOnly {
        //     get {
        //         return isReadOnly;
        //     }
        // }

        void ThrowIfReadOnly() {
            if (this.isReadOnly)
                throw Failure.ReadOnlyCollection();
        }
    }
}

