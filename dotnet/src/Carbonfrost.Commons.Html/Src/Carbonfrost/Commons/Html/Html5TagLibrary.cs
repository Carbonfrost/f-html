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

using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    sealed class Html5TagLibrary {

        public static void CopyTo(DomElementDefinitionCollection tags) {
            foreach (string tagName in blockTags) {
                HtmlElementDefinition tag = new HtmlElementDefinition(tagName);
                tags.Add(tag);
            }

            foreach (string tagName in inlineTags) {
                HtmlElementDefinition tag = new HtmlElementDefinition(tagName);
                tag.IsBlock = false;
                tag.CanContainBlock = false;
                tag.FormatAsBlock = false;
                tags.Add(tag);
            }

            // mods:
            foreach (string tagName in emptyTags) {
                HtmlElementDefinition tag = (HtmlElementDefinition) tags[tagName];

                tag.CanContainBlock = false;
                tag.CanContainInline = false;

                // can self close (<foo />). used for unknown tags that self close, without forcing them as empty.
                tag.IsEmpty = true; // can hold nothing; e.g. img
                tag.IsSelfClosing = true;
            }

            foreach (string tagName in formatAsInlineTags) {
                HtmlElementDefinition tag = (HtmlElementDefinition) tags[tagName];

                tag.FormatAsBlock = false;
            }

            // for pre, textarea, script etc
            foreach (string tagName in preserveWhitespaceTags) {
                HtmlElementDefinition tag = (HtmlElementDefinition) tags[tagName];

                tag.WhitespaceMode = DomWhitespaceMode.Preserve;
            }

        }

        // internal static initialisers:
        // prepped from http://www.w3.org/TR/REC-html40/sgml/dtd.html and other sources
        private static readonly string[] blockTags = {
            "html", "head", "body", "frameset", "script", "noscript", "style", "meta", "link", "title", "frame",
            "noframes", "section", "nav", "aside", "hgroup", "header", "footer", "p", "h1", "h2", "h3", "h4", "h5", "h6",
            "ul", "ol", "pre", "div", "blockquote", "hr", "address", "figure", "figcaption", "form", "fieldset", "ins",
            "del", "dl", "dt", "dd", "li", "table", "caption", "thead", "tfoot", "tbody", "colgroup", "col", "tr", "th",
            "td", "video", "audio", "canvas", "details", "menu", "plaintext"
        };

        private static readonly string[] inlineTags = {
            "object", "base", "font", "tt", "i", "b", "u", "big", "small", "em", "strong", "dfn", "code", "samp", "kbd",
            "var", "cite", "abbr", "time", "acronym", "mark", "ruby", "rt", "rp", "a", "img", "br", "wbr", "map", "q",
            "sub", "sup", "bdo", "iframe", "embed", "span", "input", "select", "textarea", "label", "button", "optgroup",
            "option", "legend", "datalist", "keygen", "output", "progress", "meter", "area", "param", "source", "track",
            "summary", "command", "device"
        };

        private static readonly string[] emptyTags = {
            "meta", "link", "base", "frame", "img", "br", "wbr", "embed", "hr", "input", "keygen", "col", "command",
            "device"
        };

        private static readonly string[] formatAsInlineTags = {
            "title", "a", "p", "h1", "h2", "h3", "h4", "h5", "h6", "pre", "address", "li", "th", "td", "script", "style"
        };

        private static readonly string[] preserveWhitespaceTags = {"pre", "plaintext", "title"};

    }
}

