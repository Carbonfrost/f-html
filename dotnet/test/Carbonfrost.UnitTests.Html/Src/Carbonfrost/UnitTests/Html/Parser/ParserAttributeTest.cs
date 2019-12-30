//
// - ParserAttributeTest.cs -
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

// The MIT License
//
// Copyright (c) 2009, 2010, 2011, 2012 Jonathan Hedley <jonathan@hedley.net>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.Html.Parser {

    public class ParserAttributeTest {

        [Fact]
        public void handles_solidus_in_a() {
            // test for jsoup bug #66
            string h = "<a class=lp href=/lib/14160711/>link text</a>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            HtmlElement a = doc.Select("a").First();
            Assert.Equal("link text", a.InnerText);
            Assert.Equal("/lib/14160711/", a.Attribute("href"));
        }

        [Fact]
        public void parses_rough_attributes() {
            string html = "<html><head><title>First!</title></head><body><p class=\"foo > bar\">First post! <img src=\"foo.png\" /></p></body></html>";
            HtmlDocument doc = HtmlDocument.Parse(html);

            // need a better way to verify these:
            HtmlElement p = doc.Body.Child(0);
            Assert.Equal("p", p.Tag.Name);
            Assert.Equal("foo > bar", p.Attribute("class"));
        }

        [Fact]
        public void parses_quite_rough_attributes() {
            string html = "<p =a>One<a <p>Something</p>Else";
            // this gets a <p> with attr '=a' and an <a tag with an attribue named '<p'; and then auto-recreated
            HtmlDocument doc = HtmlDocument.Parse(html);
            Assert.Equal("<p =a=\"\">One<a <p=\"\">Something</a></p>\n" +
                            "<a <p=\"\">Else</a>", doc.Body.InnerHtml);

            doc = HtmlDocument.Parse("<p .....>");
            Assert.Equal("<p .....=\"\"></p>", doc.Body.InnerHtml);
        }

        [Fact]
        public void drops_unterminated_attribute() {
            // jsoup used to parse this to <p id="foo">, but whatwg, webkit will drop.
            string h1 = "<p id=\"foo";
            HtmlDocument doc = HtmlDocument.Parse(h1);
            Assert.Equal("", doc.InnerText);
        }

    }
}
