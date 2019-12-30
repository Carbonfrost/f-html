//
// - HtmlDocument.Static.cs -
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
using System.IO;
using System.Text;

using Carbonfrost.Commons.Html.Parser;
using Carbonfrost.Commons.Core.Runtime;
using HtmlParser = Carbonfrost.Commons.Html.Parser.Parser;

namespace Carbonfrost.Commons.Html {

    partial class HtmlDocument {

        public static HtmlDocument FromFile(string path) {
            return Parse(File.ReadAllText(path));
        }

        public static HtmlDocument FromStream(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string html;
            using (StreamReader sr = new StreamReader(stream)) {
                html = sr.ReadToEnd();
            }
            return Parse(html);
        }

        public static HtmlDocument FromStream(Stream stream, Encoding encoding) {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (encoding == null)
                return FromStream(stream);

            string html;
            using (StreamReader sr = new StreamReader(stream, encoding)) {
                html = sr.ReadToEnd();
            }
            return Parse(html);
        }

        public static HtmlDocument FromStreamContext(StreamContext streamContext) {
            if (streamContext == null)
                throw new ArgumentNullException("streamContext");

            return Parse(streamContext.ReadAllText());
        }

        public static HtmlDocument ParseXml(string html, Uri baseUri) {
            TreeBuilder treeBuilder = new XmlTreeBuilder();
            return treeBuilder.Parse(html, baseUri, HtmlParseErrorCollection.NoTracking());
        }

        public static HtmlDocument Parse(string html) {
            return HtmlParser.Parse(html, null);
        }

        public static HtmlDocument Parse(string html, Uri baseUri) {
            return HtmlParser.Parse(html, baseUri);
        }

    }
}
