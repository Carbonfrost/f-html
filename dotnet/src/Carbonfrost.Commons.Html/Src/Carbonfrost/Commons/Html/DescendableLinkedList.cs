//
// - DescendableLinkedList.cs -
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

namespace Carbonfrost.Commons.Html {

    internal class DescendableLinkedList<T> : LinkedList<T> where T : class {

        public DescendableLinkedList() : base() {}

        public DescendingEnumerator GetDescendingEnumerator() {
            return new DescendingEnumerator(this);
        }

        internal class DescendingEnumerator : IEnumerator<T> {

            private LinkedList<T> list;
            private LinkedListNode<T> current = null;
            private bool first = true;

            public DescendingEnumerator(LinkedList<T> list) {
                this.list = list;
            }

            public T Current {
                get {
                    return current == null ? null : current.Value;
                }
            }

            public void Dispose() {}

            object System.Collections.IEnumerator.Current {
                get { return current == null ? null : current.Value; }
            }

            public bool MoveNext() {
                if (first) {
                    first = false;
                    current = list.Last;

                    return current != null;
                }

                if (current.Previous == null) {
                    return false;
                }

                current = current.Previous;
                return true;
            }

            public void Reset() {
                first = true;
            }

            public void Remove() {
                this.list.Remove(current);
            }
        }
    }
}
