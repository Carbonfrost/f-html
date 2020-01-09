//
// - HtmlClassList.cs -
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
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Core;
// using Carbonfrost.Commons.ComponentModel.ValueSerializers;

namespace Carbonfrost.Commons.Html {

    // [ValueSerializer(typeof(HtmlClassListConverter))]
    public class HtmlClassList : IList<string> {

        // TODO Consider implementing ISet<string>

        private HtmlElement element;
        private List<string> items = new List<string>();

        internal HtmlClassList(HtmlElement element) {
            this.element = element;
        }

        public bool Add(string item) {
            if (this.GetClassNameIndex(item) < 0) {
                items.Add(item);
                this.UpdateClassName();
                return true;
            }

            return false;
        }

        public void Toggle(string className) {
            if (this.GetClassNameIndex(className) < 0 ) {
                this.Add(className);

            } else {
                this.Remove(className);
            }
        }

        // `IList' implementation
        public string this[int index] {
            get {
                return items[index];
            }
            set {
                items[index] = value;
                UpdateClassName();
            }
        }

        public int Count {
            get { return items.Count; } }

        bool ICollection<string>.IsReadOnly {
            get { return false; } }

        public int IndexOf(string item) {
            return GetClassNameIndex(item);
        }

        public void Insert(int index, string item) {
            this.items.Insert(index, item);
            UpdateClassName();
        }

        public void RemoveAt(int index) {
            this.items.RemoveAt(index);
            UpdateClassName();
        }

        void ICollection<string>.Add(string item) {
            Add(item);
        }

        public void Clear() {
            this.items.Clear();
        }

        public bool Contains(string item) {
            return items.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex) {
            this.items.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item) {
            var index = GetClassNameIndex(item);

            if (index >= 0) {
                this.items.RemoveAt(index);
                this.UpdateClassName();
                return true;
            }

            return false;
        }

        public IEnumerator<string> GetEnumerator() {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            return string.Join(" ", this.items);
        }

        private void UpdateClassName() {
            this.element.ClassName = this.ToString();
        }

        private int GetClassNameIndex(string className) {
            if (className == null)
                throw new ArgumentNullException("className");
            if (className.Length == 0)
                throw Failure.EmptyString("className");

            if (Regex.IsMatch(className, @"\s"))
                throw HtmlFailure.CannotContainWhitespace("className");

            return this.items.IndexOf(className);
        }

        public static bool TryParse(string text, out HtmlClassList result) {
            return _TryParse(text, out result) == null;
        }

        public static HtmlClassList Parse(string text) {
            HtmlClassList result;
            Exception ex = _TryParse(text, out result);
            if (ex == null)
                return result;
            else
                throw ex;
        }

        static Exception _TryParse(string text, out HtmlClassList result) {
            throw new NotImplementedException();
        }
    }
}

