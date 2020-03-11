//
// - ParserTest.cs -
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
using Carbonfrost.Commons.Web.Dom;
using HtmlParser = Carbonfrost.Commons.Html.Parser.Parser;

namespace Carbonfrost.UnitTests.Html.Parser {

    public class ParserTest {

        [Fact]
        public void drops_unterminated_tag() {
            // jsoup used to parse this to <p>, but whatwg, webkit will drop.
            string h1 = "<p";
            HtmlDocument doc = HtmlDocument.Parse(h1);
            Assert.Equal(0, doc.GetElementsByTagName("p").Count());
            Assert.Equal("", doc.InnerText);

            string h2 = "<div id=1<p id='2'";
            doc = HtmlDocument.Parse(h2);
            Assert.Equal("", doc.InnerText);
        }

        [Fact]
        public void parses_unterminated_textarea() {
            // don't parse right to end, but break on <p>
            HtmlDocument doc = HtmlDocument.Parse("<body><p><textarea>one<p>two");
            var t = doc.Select("textarea").First();
            Assert.Equal("one", t.InnerText);
            Assert.Equal("two", doc.Select("p")[1].InnerText);
        }

        [Fact]
        public void parses_unterminated_option() {
            // bit weird this -- browsers and spec get stuck in select until there's a </select>
            HtmlDocument doc = HtmlDocument.Parse("<body><p><select><option>One<option>Two</p><p>Three</p>");
            var options = doc.Select("option");
            Assert.Equal(2, options.Count());
            Assert.Equal("One", options.First().InnerText);
            Assert.Equal("TwoThree", options.Last().InnerText);
        }

        [Fact]
        public void test_space_after_tag() {
            HtmlDocument doc = HtmlDocument.Parse("<div > <a name=\"top\"></a ><p id=1 >Hello</p></div>");
            Assert.Equal("<div> <a name=\"top\"></a><p id=\"1\">Hello</p></div>",
                            TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void creates_document_structure() {
            string html = "<meta name=keywords /><link rel=stylesheet /><title>jsoup</title><p>Hello world</p>";
            HtmlDocument doc = HtmlDocument.Parse(html);
            HtmlElement head = doc.Head;
            HtmlElement body = doc.Body;

            Assert.Equal(1, doc.Children.Count); // root node: contains html node
            Assert.Equal(2, doc.Child(0).Elements.Count); // html node: head and body
            Assert.Equal(3, head.Children.Count);
            Assert.Equal(1, body.Children.Count);

            Assert.Equal("keywords", head.GetElementsByTagName("meta").ToList()[0].Attribute("name"));
            Assert.Equal(0, body.GetElementsByTagName("meta").Count());
            Assert.Equal("jsoup", doc.Title);
            Assert.Equal("Hello world", body.InnerText);
            Assert.Equal("Hello world", body.Children[0].InnerText);
        }

        [Fact]
        public void creates_structure_from_body_snippet() {
            // the bar baz stuff naturally goes into the body, but the 'foo' goes into root, and the normalisation routine
            // needs to move into the start of the body
            string html = "foo <b>bar</b> baz";
            HtmlDocument doc = HtmlDocument.Parse(html);
            Assert.Equal("foo bar baz", doc.InnerText);
        }

        [XFact(Reason = "escaping rules")]
        public void handles_escaped_data() {
            string html = "<div title='Surf &amp; Turf'>Reef &amp; Beef</div>";
            HtmlDocument doc = HtmlDocument.Parse(html);
            var div = doc.GetElementsByTagName("div")[0];

            Assert.Equal("Surf & Turf", div.Attribute("title"));
            Assert.Equal("Reef & Beef", div.InnerText);
        }

        [Fact]
        public void handles_data_only_tags() {
            string t = "<style>font-family: bold</style>";
            var tels = HtmlDocument.Parse(t).GetElementsByTagName("style");
            Assert.Equal("font-family: bold", ((HtmlElement) tels[0]).Data);
            Assert.Equal("", tels[0].InnerText);

            string s = "<p>Hello</p><script>Nope</script><p>There</p>";
            HtmlDocument doc = HtmlDocument.Parse(s);
            Assert.Equal("Hello There", doc.InnerText);
            // UNDONE Possibly stale API: Assert.Equal("Nope", doc.Data);
        }

        [Fact]
        public void handles_text_after_data() {
            string h = "<html><body>pre <script>inner</script> aft</body></html>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<html><head></head><body>pre <script>inner</script> aft</body></html>", TextUtil.StripNewLines(doc.InnerHtml));
        }

        [Fact]
        public void handles_text_area() {
            HtmlDocument doc = HtmlDocument.Parse("<textarea>Hello</textarea>");
            var els = (HtmlElement) doc.Select("textarea").First();

            Assert.Equal("Hello", els.InnerText);
            Assert.Equal("Hello", els.Value());
        }

        [Fact]
        public void does_not_create_implicit_lists() {
            string h = "<li>Point one<li>Point two";
            HtmlDocument doc = HtmlDocument.Parse(h);
            var ol = doc.Select("ul"); // should NOT have created a default ul.
            Assert.Equal(0, ol.Count());
            var lis = doc.Select("li");
            Assert.Equal(2, lis.Count());
            Assert.Equal("body", lis.First().ParentElement.NodeName);

            // no fiddling with non-implicit lists
            string h2 = "<ol><li><p>Point the first<li><p>Point the second";
            HtmlDocument doc2 = HtmlDocument.Parse(h2);

            Assert.Equal(0, doc2.Select("ul").Count());
            Assert.Equal(1, doc2.Select("ol").Count());
            Assert.Equal(2, doc2.Select("ol li").Count());
            Assert.Equal(2, doc2.Select("ol li p").Count());
            Assert.Equal(1, ((DomElement) doc2.Select("ol li").First()).Children.Count); // one p in first li
        }

        [Fact]
        public void discards_naked_tds() {
            // jsoup used to make this into an implicit table; but browsers make it into a text run
            string h = "<td>Hello<td><p>There<p>now";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("Hello<p>There</p><p>now</p>", TextUtil.StripNewLines(doc.Body.InnerHtml));
            // <tbody> is introduced if no implicitly creating table, but allows tr to be directly under table
        }

        [Fact]
        public void handles_nested_implicit_gable() {
            HtmlDocument doc = HtmlDocument.Parse("<table><td>1</td></tr> <td>2</td></tr> <td> <table><td>3</td> <td>4</td></table> <tr><td>5</table>");
            Assert.Equal("<table><tbody><tr><td>1</td></tr> <tr><td>2</td></tr> <tr><td> <table><tbody><tr><td>3</td> <td>4</td></tr></tbody></table> </td></tr><tr><td>5</td></tr></tbody></table>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void handles_what_wg_expenses_table_example() {
            // http://www.whatwg.org/specs/web-apps/current-work/multipage/tabular-data.html#examples-0
            HtmlDocument doc = HtmlDocument.Parse("<table> <colgroup> <col> <colgroup> <col> <col> <col> <thead> <tr> <th> <th>2008 <th>2007 <th>2006 <tbody> <tr> <th scope=rowgroup> Research and development <td> $ 1,109 <td> $ 782 <td> $ 712 <tr> <th scope=row> Percentage of net sales <td> 3.4% <td> 3.3% <td> 3.7% <tbody> <tr> <th scope=rowgroup> Selling, general, and administrative <td> $ 3,761 <td> $ 2,963 <td> $ 2,433 <tr> <th scope=row> Percentage of net sales <td> 11.6% <td> 12.3% <td> 12.6% </table>");
            Assert.Equal("<table> <colgroup> <col /> </colgroup><colgroup> <col /> <col /> <col /> </colgroup><thead> <tr> <th> </th><th>2008 </th><th>2007 </th><th>2006 </th></tr></thead><tbody> <tr> <th scope=\"rowgroup\"> Research and development </th><td> $ 1,109 </td><td> $ 782 </td><td> $ 712 </td></tr><tr> <th scope=\"row\"> Percentage of net sales </th><td> 3.4% </td><td> 3.3% </td><td> 3.7% </td></tr></tbody><tbody> <tr> <th scope=\"rowgroup\"> Selling, general, and administrative </th><td> $ 3,761 </td><td> $ 2,963 </td><td> $ 2,433 </td></tr><tr> <th scope=\"row\"> Percentage of net sales </th><td> 11.6% </td><td> 12.3% </td><td> 12.6% </td></tr></tbody></table>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void handles_tbody_table() {
            HtmlDocument doc = HtmlDocument.Parse("<html><head></head><body><table><tbody><tr><td>aaa</td><td>bbb</td></tr></tbody></table></body></html>");
            Assert.Equal("<table><tbody><tr><td>aaa</td><td>bbb</td></tr></tbody></table>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void handles_implicit_caption_close() {
            HtmlDocument doc = HtmlDocument.Parse("<table><caption>A caption<td>One<td>Two");
            Assert.Equal("<table><caption>A caption</caption><tbody><tr><td>One</td><td>Two</td></tr></tbody></table>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void no_table_direct_in_table() {
            HtmlDocument doc = HtmlDocument.Parse("<table> <td>One <td><table><td>Two</table> <table><td>Three");
            Assert.Equal("<table> <tbody><tr><td>One </td><td><table><tbody><tr><td>Two</td></tr></tbody></table> <table><tbody><tr><td>Three</td></tr></tbody></table></td></tr></tbody></table>",
                            TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void ignores_dupe_end_tr_tag() {
            HtmlDocument doc = HtmlDocument.Parse("<table><tr><td>One</td><td><table><tr><td>Two</td></tr></tr></table></td><td>Three</td></tr></table>"); // two </tr></tr>, must ignore or will close table
            Assert.Equal("<table><tbody><tr><td>One</td><td><table><tbody><tr><td>Two</td></tr></tbody></table></td><td>Three</td></tr></tbody></table>",
                            TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        [Skip("Revisit handling of BaseUri.")]
        public void handles_base_tags() {
            // TODO -- don't handle base tags like this -- spec and browsers don't (any more -- v. old ones do).
            // instead, just maintain one baseUri in the doc
            string h = "<a href=1>#</a><base href='/2/'><a href='3'>#</a><base href='http://bar'><a href=4>#</a>";
            HtmlDocument doc = HtmlDocument.Parse(h, new Uri("http://foo/"));
            //Assert.Equal("http://bar", doc.BaseUri); // gets updated as base changes, so doc.createElement has latest.
            Assert.Equal("http://foo/2/", doc.BaseUri.ToString()); // Slight limitation in .NET, System.Uri class adds slash after string.

            var anchors = doc.GetElementsByTagName("a").ToList();
            Assert.Equal(3, anchors.Count);

            Assert.Equal("http://foo/", anchors[0].BaseUri.ToString());
            Assert.Equal("http://foo/2/", anchors[1].BaseUri.ToString());
            Assert.Equal("http://bar", anchors[2].BaseUri.ToString());
            Assert.Equal("http://bar/", anchors[2].BaseUri.ToString()); // Again, same limitation as above.
        }

        [Fact]
        public void handles_cdata() {
            // TODO as this is html namespace, should actually treat as bogus comment, not cdata. keep as cdata for now
            string h = "<div id=1><![CDATA[<html>\n<foo><&amp;]]></div>"; // the &amp; in there should remain literal
            HtmlDocument doc = HtmlDocument.Parse(h);
            var div = doc.GetElementById("1");
            Assert.Equal(0, div.Children.Count);
            Assert.Equal(1, div.ChildNodes.Count); // no elements, one text node
            Assert.Equal("<html> <foo><&amp;", div.InnerText);
        }

        [XFact(Reason = "Escaping rules")]
        public void handles_invalid_start_tags() {
            string h = "<div>Hello < There <&amp;></div>"; // parse to <div {#text=Hello < There <&>}>
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("Hello < There <&>", doc.Select("div").First().InnerText);
        }

        [Fact]
        public void handles_unknown_tags() {
            string h = "<div><foo title=bar>Hello<foo title=qux>there</foo></div>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            var foos = doc.Select("foo");
            Assert.Equal(2, foos.Count());
            Assert.Equal("bar", foos.First().Attribute("title"));
            Assert.Equal("qux", foos.Last().Attribute("title"));
            Assert.Equal("there", foos.Last().InnerText);
        }

        [Fact]
        public void handles_unknown_inline_tags() {
            string h = "<p><cust>Test</cust></p><p><cust><cust>Test</cust></cust></p>";
            HtmlDocument doc = HtmlParser.ParseBodyFragment(h, null);
            string output = doc.Body.InnerHtml;
            Assert.Equal(h, TextUtil.StripNewLines(output));
        }

        [Fact]
        public void parses_body_fragment() {
            string h = "<!-- comment --><p><a href='foo'>One</a></p>";
            HtmlDocument doc = HtmlParser.ParseBodyFragment(h, new Uri("http://example.com"));
            Assert.Equal("<body><!-- comment --><p><a href=\"foo\">One</a></p></body>", TextUtil.StripNewLines(doc.Body.OuterHtml));
            var a = doc.Select("a").First();
            Assert.Equal(new Uri("http://example.com/foo"), new Uri(a.BaseUri, a.Attribute("href")));
        }

        [Fact]
        public void handles_unknown_namespace_tags() {
            // note that the first foo:bar should not really be allowed to be self closing, if parsed in html mode.
            string h = "<foo:bar id='1' /><abc:def id=2>Foo<p>Hello</p></abc:def><foo:bar>There</foo:bar>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<foo:bar id=\"1\" /><abc:def id=\"2\">Foo<p>Hello</p></abc:def><foo:bar>There</foo:bar>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void handles_known_empty_blocks() {
            // if known tag, must be defined as self closing to allow as self closing. unkown tags can be self closing.
            string h = "<div id='1' /><div id=2><img /><img></div> <hr /> hr text <hr> hr text two";
            HtmlDocument doc = HtmlDocument.Parse(h);
            var div1 = doc.GetElementById("1");

            Assert.NotEmpty(div1.Children); // <div /> is treated as <div>...
            Assert.Empty(((DomElement) doc.Select("hr").First()).Children);
            Assert.Empty(((DomElement) doc.Select("hr").Last()).Children);
            Assert.Empty(((DomElement) doc.Select("img").First()).Children);
            Assert.Empty(((DomElement) doc.Select("img").Last()).Children);
        }

        [Fact]
        public void handles_solidus_at_attribute_end() {
            // this test makes sure [<a href=/>link</a>] is parsed as [<a href="/">link</a>], not [<a href="" /><a>link</a>]
            string h = "<a href=/>link</a>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<a href=\"/\">link</a>", doc.Body.InnerHtml);
        }

        [Fact]
        public void handles_multi_closing_body() {
            string h = "<body><p>Hello</body><p>there</p></body></body></html><p>now";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal(3, doc.Select("p").Count());
            Assert.Equal(3, doc.Body.Children.Count);
        }

        [Fact]
        public void handles_unclosed_definition_lists() {
            // jsoup used to create a <dl>, but that's not to spec
            string h = "<dt>Foo<dd>Bar<dt>Qux<dd>Zug";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal(0, doc.Select("dl").Count()); // no auto dl
            Assert.Equal(4, doc.Select("dt, dd").Count());
            var dts = doc.Select("dt");
            Assert.Equal(2, dts.Count());
            Assert.Equal("Zug", ((DomElement) dts[1]).NextSibling.InnerText);
        }

        [Fact]
        public void handles_blocks_in_definitions() {
            // per the spec, dt and dd are inline, but in practise are block
            string h = "<dl><dt><div id=1>Term</div></dt><dd><div id=2>Def</div></dd></dl>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("dt", doc.Select("#1").First().ParentNode.NodeName);
            Assert.Equal("dd", doc.Select("#2").First().ParentNode.NodeName);
            Assert.Equal("<dl><dt><div id=\"1\">Term</div></dt><dd><div id=\"2\">Def</div></dd></dl>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void handles_frames() {
            string h = "<html><head><script></script><noscript></noscript></head><frameset><frame src=foo></frame><frame src=foo></frameset></html>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<html><head><script></script><noscript></noscript></head><frameset><frame src=\"foo\" /><frame src=\"foo\" /></frameset></html>",
                            TextUtil.StripNewLines(doc.OuterHtml));
            // no body auto vivification
        }

        [Fact]
        public void handlesJ_javadoc_font() {
            string h = "<TD BGCOLOR=\"#EEEEFF\" CLASS=\"NavBarCell1\">    <A HREF=\"deprecated-list.html\"><FONT CLASS=\"NavBarFont1\"><B>Deprecated</B></FONT></A>&nbsp;</TD>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            var a = (DomElement) doc.Select("a").First();
            Assert.Equal("Deprecated", a.InnerText);
            Assert.Equal("font", a.Child(0).NodeName);
            Assert.Equal("b", a.Child(0).Child(0).NodeName);
        }

        [Fact]
        public void handles_base_without_href() {
            string h = "<head><base target='_blank'></head><body><a href=/foo>Test</a></body>";
            HtmlDocument doc = HtmlDocument.Parse(h, new Uri("http://example.com/"));
            var a = doc.Select("a").First();
            Assert.Equal(new Uri("http://example.com"), a.BaseUri);
            Assert.Equal("/foo", a.Attribute("href"));
            Assert.Equal(new Uri("http://example.com/foo"), new Uri(a.BaseUri, a.Attribute("href")));
        }

        [Fact]
        public void normalises_document() {
            string h = "<!doctype html>One<html>Two<head>Three<link></head>Four<body>Five </body>Six </html>Seven ";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<!DOCTYPE html><html><head></head><body>OneTwoThree<link />FourFive Six Seven </body></html>",
                            TextUtil.StripNewLines(doc.InnerHtml));
        }

        [Fact]
        public void normalises_empty_document() {
            HtmlDocument doc = HtmlDocument.Parse("");
            Assert.Equal("<html><head></head><body></body></html>", TextUtil.StripNewLines(doc.InnerHtml));
        }

        [Fact]
        public void normalises_headless_body() {
            HtmlDocument doc = HtmlDocument.Parse("<html><body><span class=\"foo\">bar</span>");
            Assert.Equal("<html><head></head><body><span class=\"foo\">bar</span></body></html>",
                            TextUtil.StripNewLines(doc.InnerHtml));
        }

        // UNDONE body attributes get copied to html incorrectly
        [Fact]
        [Skip]
        public void normalised_body_after_content() {
            HtmlDocument doc = HtmlDocument.Parse("<font face=Arial><body class=name><div>One</div></body></font>");
            Assert.Equal("<html><head></head><body class=\"name\"><font face=\"Arial\"><div>One</div></font></body></html>",
                            TextUtil.StripNewLines(doc.InnerHtml));
        }

        [Fact]
        public void finds_charset_in_malformed_meta() {
            string h = "<meta http-equiv=Content-Type content=text/html; charset=gb2312>";
            // example cited for reason of html5's <meta charset> element
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("gb2312", doc.Select("meta").First().Attribute("charset"));
        }

        [Fact]
        public void test_hgroup() {
            // jsoup used to not allow hroup in h{n}, but that's not in spec, and browsers are OK
            HtmlDocument doc = HtmlDocument.Parse("<h1>Hello <h2>There <hgroup><h1>Another<h2>headline</hgroup> <hgroup><h1>More</h1><p>stuff</p></hgroup>");
            Assert.Equal("<h1>Hello </h1><h2>There <hgroup><h1>Another</h1><h2>headline</h2></hgroup> <hgroup><h1>More</h1><p>stuff</p></hgroup></h2>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void test_relaxed_tags() {
            HtmlDocument doc = HtmlDocument.Parse("<abc_def id=1>Hello</abc_def> <abc-def>There</abc-def>");
            Assert.Equal("<abc_def id=\"1\">Hello</abc_def> <abc-def>There</abc-def>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void test_header_contents() {
            // h* tags (h1 .. h9) in browsers can handle any internal content other than other h*. which is not per any
            // spec, which defines them as containing phrasing content only. so, reality over theory.
            HtmlDocument doc = HtmlDocument.Parse("<h1>Hello <div>There</div> now</h1> <h2>More <h3>Content</h3></h2>");
            Assert.Equal("<h1>Hello <div>There</div> now</h1> <h2>More </h2><h3>Content</h3>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void test_span_contents() {
            // like h1 tags, the spec says SPAN is phrasing only, but browsers and publisher treat span as a block tag
            HtmlDocument doc = HtmlDocument.Parse("<span>Hello <div>there</div> <span>now</span></span>");
            Assert.Equal("<span>Hello <div>there</div> <span>now</span></span>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void test_no_images_in_no_script_in_head() {
            // jsoup used to allow, but against spec if parsing with noscript
            HtmlDocument doc = HtmlDocument.Parse("<html><head><noscript><img src='foo'></noscript></head><body><p>Hello</p></body></html>");
            Assert.Equal("<html><head><noscript></noscript></head><body><img src=\"foo\" /><p>Hello</p></body></html>", TextUtil.StripNewLines(doc.InnerHtml));
        }

        [Fact]
        public void test_a_flow_contents() {
            // html5 has <a> as either phrasing or block
            HtmlDocument doc = HtmlDocument.Parse("<a>Hello <div>there</div> <span>now</span></a>");
            Assert.Equal("<a>Hello <div>there</div> <span>now</span></a>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void test_font_flow_contents() {
            // html5 has no definition of <font>; often used as flow
            HtmlDocument doc = HtmlDocument.Parse("<font>Hello <div>there</div> <span>now</span></font>");
            Assert.Equal("<font>Hello <div>there</div> <span>now</span></font>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        [Fact]
        public void handles_misnested_tags_b_i() {
            // whatwg: <b><i></b></i>
            string h = "<p>1<b>2<i>3</b>4</i>5</p>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<p>1<b>2<i>3</i></b><i>4</i>5</p>", doc.Body.InnerHtml);
            // adoption agency on </b>, reconstruction of formatters on 4.
        }

        [Fact]
        public void handles_misnested_tags_b_p() {
            //  whatwg: <b><p></b></p>
            string h = "<b>1<p>2</b>3</p>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<b>1</b>\n<p><b>2</b>3</p>", doc.Body.InnerHtml);
        }

        [Fact]
        public void handles_unexpected_markup_in_tables() {
            // whatwg - tests markers in active formatting (if they didn't work, would get in in table)
            // also tests foster parenting
            string h = "<table><b><tr><td>aaa</td></tr>bbb</table>ccc";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<b></b><b>bbb</b><table><tbody><tr><td>aaa</td></tr></tbody></table><b>ccc</b>", TextUtil.StripNewLines(doc.Body.InnerHtml));
        }

        // UNDONE An excess b doesn't get thrown away ?

        [Fact]
        [Skip]
        public void handles_unclosed_formatting_elements() {
            // whatwg: formatting elements get collected and applied, but excess elements are thrown away
            string h = "<!DOCTYPE html>\n" +
                "<p><b class=x><b class=x><b><b class=x><b class=x><b>X\n" +
                "<p>X\n" +
                "<p><b><b class=x><b>X\n" +
                "<p></b></b></b></b></b></b>X";
            HtmlDocument doc = HtmlDocument.Parse(h);
            string want = "<!DOCTYPE html>\n" +
                "<html>\n" +
                "<head></head>\n" +
                "<body>\n" +
                "<p><b class=\"x\"><b class=\"x\"><b><b class=\"x\"><b class=\"x\"><b>X </b></b></b></b></b></b></p>\n" +
                "<p><b class=\"x\"><b><b class=\"x\"><b class=\"x\"><b>X </b></b></b></b></b></p>\n" +
                "<p><b class=\"x\"><b><b class=\"x\"><b class=\"x\"><b><b><b class=\"x\"><b>X </b></b></b></b></b></b></b></b></p>\n" +
                "<p>X</p>\n" +
                "</body>\n" +
                "</html>";
            Assert.Equal(TextUtil.CompressWhitespace(want), TextUtil.CompressWhitespace(doc.InnerHtml));
        }

        // UNDONE Looks like outer <b class> becomes inner ?

        [Fact, Skip]
        public void reconstruct_formatting_elements() {
            // tests attributes and multi b
            string h = "<p><b class=one>One <i>Two <b>Three</p><p>Hello</p>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal(TextUtil.CompressWhitespace("<p><b class=\"one\">One <i>Two <b>Three</b></i></b></p>\n<p><b class=\"one\"><i><b>Hello</b></i></b></p>"),
                            TextUtil.CompressWhitespace(doc.Body.InnerHtml));
        }

        // UNDONE Reconstructing formatting elements is problematic inside tables

        [Fact, Skip]
        public void reconstruct_formatting_elements_in_table() {
            // tests that tables get formatting markers -- the <b> applies outside the table and does not leak in,
            // and the <i> inside the table and does not leak out.
            string h = "<p><b>One</p> <table><tr><td><p><i>Three<p>Four</i></td></tr></table> <p>Five</p>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            string want = "<p><b>One</b></p>\n" +
                "<b> \n" +
                " <table>\n" +
                "  <tbody>\n" +
                "   <tr>\n" +
                "    <td><p><i>Three</i></p><p><i>Four</i></p></td>\n" +
                "   </tr>\n" +
                "  </tbody>\n" +
                " </table> <p>Five</p></b>";
            Assert.Equal(TextUtil.CompressWhitespace(want), TextUtil.CompressWhitespace(doc.Body.InnerHtml));
        }


        [Fact]
        public void empty_td_tag() {
            string h = "<table><tr><td>One</td><td id='2' /></tr></table>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal("<td>One</td>\n<td id=\"2\"></td>", ((HtmlElement) doc.Select("tr").First()).InnerHtml);
        }


        [Fact]
        public void handles_span_in_tbody() {
            // test for jsoup bug 64
            string h = "<table><tbody><span class='1'><tr><td>One</td></tr><tr><td>Two</td></tr></span></tbody></table>";
            HtmlDocument doc = HtmlDocument.Parse(h);
            Assert.Equal(((DomElement) doc.Select("span").First()).Children.Count, 0); // the span gets closed
            Assert.Equal(doc.Select("table").Count(), 1); // only one table
        }

        [Fact]
        public void handles_unclosed_title_at_eof() {
            Assert.Equal("Data", HtmlDocument.Parse("<title>Data").Title);
            Assert.Equal("Data<", HtmlDocument.Parse("<title>Data<").Title);
            Assert.Equal("Data</", HtmlDocument.Parse("<title>Data</").Title);
            Assert.Equal("Data</t", HtmlDocument.Parse("<title>Data</t").Title);
            Assert.Equal("Data</ti", HtmlDocument.Parse("<title>Data</ti").Title);
            Assert.Equal("Data", HtmlDocument.Parse("<title>Data</title>").Title);
            Assert.Equal("Data", HtmlDocument.Parse("<title>Data</title >").Title);
        }

        [Fact]
        public void handles_unclosed_title() {
            HtmlDocument one = HtmlDocument.Parse("<title>One <b>Two <b>Three</TITLE><p>Test</p>"); // has title, so <b> is plain text
            Assert.Equal("One <b>Two <b>Three", one.Title);
            Assert.Equal("Test", one.Select("p").First().InnerText);

            HtmlDocument two = HtmlDocument.Parse("<title>One<b>Two <p>Test</p>"); // no title, so <b> causes </title> breakout
            Assert.Equal("One", two.Title);
            Assert.Equal("<b>Two <p>Test</p></b>", two.Body.InnerHtml);
        }

        [Fact]
        public void no_implicit_form_for_text_areas() {
            // old jsoup parser would create implicit forms for form children like <textarea>, but no more
            HtmlDocument doc = HtmlDocument.Parse("<textarea>One</textarea>");
            Assert.Equal("<textarea>One</textarea>", doc.Body.InnerHtml);
        }
    }
}
