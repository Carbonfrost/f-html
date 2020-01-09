//
// - HtmlElementQuery.cs -
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
using System.Collections.ObjectModel;
using System.Linq;

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    // TODO This should just be enumerable

    public partial class HtmlElementQuery : Collection<HtmlElement> {

        public HtmlElementQuery(IEnumerable<HtmlElement> items) : base(items.ToArray()) {}

        public HtmlElementQuery() : base() {}

        // `IHtmlElementQuery' implementation
        public HtmlElementQuery Select(string cssQuery) {
            if (cssQuery == null)
                throw new ArgumentNullException("cssQuery");
            cssQuery = cssQuery.Trim();

            if (cssQuery.Length == 0)
                throw Failure.AllWhitespace("cssQuery");

            IEnumerable<HtmlElement> elements =
                this.SelectMany(t => t.Select(cssQuery)).Distinct();

            return new HtmlElementQuery(elements);
        }

        public HtmlElementQuery AppendElement(string tag) {
            return new HtmlElementQuery(this.Select(t => t.AppendElement(tag)));
        }

        public HtmlElementQuery PrependElement(string tag) {
            return new HtmlElementQuery(this.Select(t => t.PrependElement(tag)));
        }
    }
}
