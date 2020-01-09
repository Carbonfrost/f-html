//
// - HtmlElementQuery.Helpers.cs -
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

    partial class HtmlElementQuery {

        public HtmlElementQuery ClassName(string className) {
            return this.Call(HtmlElementExtensions.ClassName, className);
        }

        public HtmlElementQuery ClassNames(IEnumerable<string> classNames) {
            return this.Call(HtmlElementExtensions.ClassNames, classNames);
        }

        public bool HasClass(string className) {
            return this.All(t => t.HasClass(className));
        }

        public HtmlElementQuery AddClass(string className) {
            if (className == null)
                return this;

            return this.ForEach(t => t.AddClass(className));
        }

        public HtmlElementQuery RemoveClass(string className) {
            if (className == null)
                return this;

            return this.ForEach(t => t.RemoveClass(className));
        }

        private HtmlElementQuery ForEach(Action<HtmlElement> func) {
            foreach (var e in this)
                func(e);

            return this;
        }

        private HtmlElementQuery Call<T>(Func<HtmlElement, T, HtmlElement> func, T arg) {
            this.Select(e => func(e, arg));
            return this;
        }
    }
}
