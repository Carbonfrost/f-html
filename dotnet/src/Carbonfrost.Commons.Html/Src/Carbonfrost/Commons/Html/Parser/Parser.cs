//
// - Parser.cs -
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
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html.Parser {

    class Parser {

        private const int DEFAULT_MAX_ERRORS = 0; // by default, error tracking is disabled.

        private TreeBuilder treeBuilder;
        private int maxErrors = DEFAULT_MAX_ERRORS;
        private HtmlParseErrorCollection errors;

        public int MaxErrors {
            get { return maxErrors; }
            set { maxErrors = value; }
        }

        public TreeBuilder TreeBuilder {
            get {
                return treeBuilder;
            }
            set {
                this.treeBuilder = value;
            }
        }

        public IList<HtmlParseError> Errors {
            get {
                return errors;
            }
        }

        public Parser(TreeBuilder treeBuilder) {
            this.treeBuilder = treeBuilder;
        }

        public HtmlDocument ParseInput(String html, Uri baseUri) {
            errors = IsTrackErrors() ? HtmlParseErrorCollection.Tracking(maxErrors) : HtmlParseErrorCollection.NoTracking();
            HtmlDocument doc = treeBuilder.Parse(html, baseUri, errors);
            return doc;
        }

        private bool IsTrackErrors() {
            return maxErrors > 0;
        }

        public static HtmlDocument Parse(String html, Uri baseUri) {
            TreeBuilder treeBuilder = new HtmlTreeBuilder();
            return treeBuilder.Parse(html, baseUri, HtmlParseErrorCollection.NoTracking());
        }

        public static IList<DomNode> ParseFragment(String fragmentHtml, HtmlElement context, Uri baseUri) {
            HtmlTreeBuilder treeBuilder = new HtmlTreeBuilder();
            return treeBuilder.ParseFragment(fragmentHtml, context, baseUri, HtmlParseErrorCollection.NoTracking());
        }

        public static HtmlDocument ParseBodyFragment(String bodyHtml, Uri baseUri) {
            HtmlDocument doc = HtmlDocument.CreateShell(baseUri);
            HtmlElement body = doc.Body;
            var nodeList = ParseFragment(bodyHtml, body, baseUri);
            var nodes = nodeList.ToArray(); // the node list gets modified when re-parented
            foreach (var node in nodes) {
                body.Append(node);
            }
            return doc;
        }

        public static HtmlDocument ParseBodyFragmentRelaxed(String bodyHtml, Uri baseUri) {
            return Parse(bodyHtml, baseUri);
        }

        public static Parser HtmlParser() {
            return new Parser(new HtmlTreeBuilder());
        }

        public static Parser XmlParser() {
            return new Parser(new XmlTreeBuilder());
        }
    }

}

