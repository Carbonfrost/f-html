//
// - HtmlElementExtensions.cs -
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

    public static class HtmlElementExtensions {

        public static HtmlElement ClassName(this HtmlElement element, string className) {
            if (element == null)
                return null;

            return (HtmlElement) element.Attribute("class", className);
        }

        public static HtmlElement ClassNames(this HtmlElement element, IEnumerable<string> classNames) {
            if (element == null)
                return null;

            if (classNames == null)
                return element;

            return element.ClassName(string.Join(" ", classNames));
        }

        public static HtmlElement InnerText(this HtmlElement element, string text) {
            if (element == null)
                return null;

            element.InnerText = text;
            return element;

        }
    }
}
