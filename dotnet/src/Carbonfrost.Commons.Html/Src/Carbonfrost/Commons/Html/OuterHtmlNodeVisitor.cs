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

using System.Text;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    internal class OuterHtmlNodeVisitor : HtmlNodeVisitor {

        private int _depth;
        private readonly StringBuilder _output;
        private readonly HtmlWriterSettings _settings;
        private readonly Tokens _tokens;

        private bool PrettyPrint {
            get {
                return _settings.Indent;
            }
        }

        public OuterHtmlNodeVisitor(StringBuilder accum, bool xml = false) {
            _output = accum;
            _settings = new HtmlWriterSettings {
                IndentWidth = 1,
            };
            _tokens = xml ? Tokens.Xml : Tokens.Html;
        }

        // `HtmlNodeVisitor' implementation
        protected override void VisitEntityReference(DomEntityReference node) {
            _output.Append("&")
                .Append(node.NodeName);
        }

        protected override void VisitCDataSection(DomCDataSection node) {
            _output.Append("<!CDATA[")
                .Append(node.TextContent)
                .Append("]]>");
        }

        protected override void VisitDocumentType(DomDocumentType node) {
            _output.Append("<!DOCTYPE ").Append(node.Name);

            if (!StringUtil.IsBlank(node.PublicId))
                _output.Append(" PUBLIC \"")
                    .Append(node.PublicId)
                    .Append("\"");

            if (!StringUtil.IsBlank(node.SystemId))
                _output.Append(" \"")
                    .Append(node.SystemId)
                    .Append("\"");

            _output.Append('>');
        }

        protected override void VisitElement(HtmlElement node) {
            if (_output.Length > 0 && PrettyPrint
                && (node.Tag.FormatAsBlock || (node.Parent is HtmlElement parent && parent.Tag.FormatAsBlock))) {
                Indent();
            }

            _output.Append("<")
                .Append(node.Tag.Name);

            foreach (HtmlAttribute attribute in node.Attributes) {
                _output.Append(" ");
                attribute.AppendHtml(_output, _settings);
            }

            if (node.ChildNodes.IsEmpty() && node.Tag.IsSelfClosing)
                _output.Append(_tokens.SelfCloseTag);
            else
                _output.Append(">");

            _depth++;

            base.VisitElement(node);

            --_depth;
            if (!(node.ChildNodes.IsEmpty() && node.Tag.IsSelfClosing)) {
                if (PrettyPrint && !node.ChildNodes.IsEmpty() && node.Tag.FormatAsBlock) {
                    Indent();
                }
                _output.Append("</").Append(node.Tag.Name).Append(">");
            }
        }

        protected override void VisitProcessingInstruction(HtmlProcessingInstruction node) {
            if (PrettyPrint) {
                Indent();
            }

            _output
                .Append("<?")
                .Append(node.TextContent)
                .Append("?>");
        }

        protected override void VisitComment(DomComment node) {
            if (PrettyPrint) {
                Indent();
            }
            _output
                .Append("<!--")
                .Append(node.Text)
                .Append("-->");
        }

        protected override void VisitText(DomText text) {
            _output.Append(text.Data);
        }

        protected override void VisitText(HtmlText node) {
            string html;
            if (node.IsData) {
                html = node.Data;

            } else {
                html = HtmlEncoder.Escape(node.Data, _settings.Charset.GetEncoder(), _settings.EscapeMode);

                if (PrettyPrint
                    && node.ParentElement is HtmlElement
                    && !((HtmlElement) node.ParentElement).PreserveWhitespace) {
                    html = StringUtil.NormalizeWhitespace(html);
                }

                if (PrettyPrint && node.NodePosition == 0
                    && node.ParentElement is HtmlElement && ((HtmlElement) node.ParentElement).Tag.FormatAsBlock && !node.IsBlank) {
                    Indent();
                }
            }

            _output.Append(html);
        }

        private void Indent() {
            _output.Append("\n").Append(' ' , _depth * _settings.IndentWidth);
        }

        struct Tokens {
            public static readonly Tokens Xml = new Tokens("/>");
            public static readonly Tokens Html = new Tokens(">");

            public readonly string SelfCloseTag;

            private Tokens(string selfCloseTag) {
                SelfCloseTag = selfCloseTag;
            }
        }
    }

    partial class Extensions {

        internal static string GetOuterHtml(this IHtmlNode node) {
            var accum = new StringBuilder();
            var v = new OuterHtmlNodeVisitor(accum);
            v.Visit((DomNode) node);

            return accum.ToString().Trim();
        }

        internal static string GetInnerHtml(this IHtmlNode self) {
            var accum = new StringBuilder();
            var v = new OuterHtmlNodeVisitor(accum);
            foreach (var node in ((DomNode) self).ChildNodes) {
                v.Visit(node);
            }

            return accum.ToString().Trim();
        }
    }
}
