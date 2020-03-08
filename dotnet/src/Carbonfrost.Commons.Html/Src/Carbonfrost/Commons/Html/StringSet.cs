//
// Copyright 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
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

namespace Carbonfrost.Commons.Html {

    sealed class StringSet : IReadOnlyCollection<string> {

        static readonly Dictionary<string, StringSet> _hashes = new Dictionary<string, StringSet>();
        private readonly HashSet<string> _items;

        public int Count {
            get {
                return _items.Count;
            }
        }

        private StringSet(IEnumerable<string> items) {
            _items = new HashSet<string>(items);
        }

        public bool Contains(string item) {
            return _items.Contains(item);
        }

        // As long as clients use interned strings for the argument, dictionary lookup should be very fast
        // Like qw// (or %w in Ruby) then making a set
        public static StringSet Create(string text) {
            return _hashes.GetValueOrCache(text, _ => new StringSet(SplitHashText(text)));
        }

        static IEnumerable<string> SplitHashText(string text) {
            return text.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerator<string> GetEnumerator() {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
