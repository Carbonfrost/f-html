//
// Copyright 2012, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

using System.Linq;
using Carbonfrost.Commons.Html;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Html.Parser {

    public class ParserCommentTest {

        [Fact]
        public void comment_before_html() {
            string h = "<!-- comment --><!-- comment 2 --><p>One</p>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<!-- comment --><!-- comment 2 --><html><head></head><body><p>One</p></body></html>", TextUtil.StripNewLines(doc.InnerHtml));
        }

        [Fact]
        public void parses_comments() {
            string html = "<html><head></head><body><img src=foo><!-- <table><tr><td></table> --><p>Hello</p></body></html>";
            HtmlDocument doc = HtmlDocument.Parse(html);

            HtmlElement body = doc.Body;
            var comment = (DomComment) body.ChildNode(1); // comment should not be sub of img, as it's an empty tag
            Assert.Equal(" <table><tr><td></table> ", comment.Data);
            var p = body.Child(1);
            HtmlText text = (HtmlText) p.ChildNodes[0];
            Assert.Equal("Hello", text.Data);
        }

        [Fact]
        public void parses_unterminated_comments() {
            string html = "<p>Hello<!-- <tr><td>";
            HtmlDocument doc = HtmlDocument.Parse(html);
            var p = doc.GetElementsByTagName("p").ToList()[0];

            // UNDONE Assert.Equal("Hello", p.next());
            HtmlText text = (HtmlText) p.ChildNodes[0];

            Assert.Equal("Hello", text.Data);
            var comment = (DomComment) p.ChildNodes[1];
            Assert.Equal(" <tr><td>", comment.Data);
        }
    }
}
