//
// - Mixin.cs -
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
using System.Linq;

namespace Carbonfrost.Commons.Html {

    static class Mixin {

        public static bool IsEmpty<T>(this ICollection<T> items) {
            return items.Count == 0;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T item) {
            return items.Where(t => !object.Equals(t, item));
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source,
                                                             TKey key) {
            TValue value;
            if (source.TryGetValue(key, out value))
                return value;
            else
                return default(TValue);
        }

        public static TValue GetValueOrCache<TKey, TValue>(this IDictionary<TKey, TValue> source,
                                                           TKey key,
                                                           Func<TKey, TValue> func) {
            TValue value;
            if (source.TryGetValue(key, out value))
                return value;
            else
                return source[key] = func(key);
        }

        public static void AddMany<T>(this ICollection<T> items, IEnumerable<T> others) {
            foreach (var element in others) {
                items.Add(element);
            }
        }

    }
}
