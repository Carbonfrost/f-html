//
// - HtmlNode.OuterHtmlNodeVisitor.cs -
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
using System.Text;

namespace Carbonfrost.Commons.Html {

    partial class HtmlNode {

        internal class OuterHtmlNodeVisitor : HtmlNodeVisitor {

            private int depth;
            private StringBuilder output;
            private HtmlWriterSettings settings;

            public OuterHtmlNodeVisitor(StringBuilder accum) {
                this.output = accum;
                this.settings = new HtmlWriterSettings();
            }

            // `HtmlNodeVisitor' implementation
            public override void VisitEntityReference(HtmlEntityReference node) {
                output.Append("&")
                    .Append(node.NodeName);
            }

            public override void VisitCDataSection(HtmlCDataSection node) {
                output.Append("<!CDATA[")
                    .Append(node.TextContent)
                    .Append("]]>");
            }

            public override void VisitDocumentType(HtmlDocumentType node) {
                output.Append("<!DOCTYPE ").Append(node.Name);

                if (!StringUtil.IsBlank(node.PublicId))
                    output.Append(" PUBLIC \"")
                        .Append(node.PublicId)
                        .Append("\"");

                if (!StringUtil.IsBlank(node.SystemId))
                    output.Append(" \"")
                        .Append(node.SystemId)
                        .Append("\"");

                output.Append('>');
            }

            public override void VisitElement(HtmlElement node) {
                if (output.Length > 0 && settings.PrettyPrint
                    && (node.Tag.FormatAsBlock || (node.Parent != null && node.Parent.Tag.FormatAsBlock)))
                    Indent();

                output.Append("<")
                    .Append(node.Tag.Name);

                foreach (HtmlAttribute attribute in node.Attributes) {
                    output.Append(" ");
                    attribute.AppendHtml(output, settings);
                }

                if (node.ChildNodes.IsEmpty() && node.Tag.IsSelfClosing)
                    output.Append(" />");
                else
                    output.Append(">");

                depth++;

                base.VisitElement(node);

                --depth;
                if (!(node.ChildNodes.IsEmpty() && node.Tag.IsSelfClosing)) {
                    if (settings.PrettyPrint && !node.ChildNodes.IsEmpty() && node.Tag.FormatAsBlock)
                        Indent();
                    output.Append("</").Append(node.Tag.Name).Append(">");
                }
            }

            public override void VisitDocument(HtmlDocument node) {
                base.VisitDocument(node);
            }

            public override void VisitProcessingInstruction(HtmlProcessingInstruction node) {
                if (settings.PrettyPrint)
                    Indent();

                output
                    .Append("<?")
                    .Append(node.TextContent)
                    .Append("?>");
            }

            public override void VisitComment(HtmlComment node) {
                if (settings.PrettyPrint)
                    Indent();
                output
                    .Append("<!--")
                    .Append(node.Text)
                    .Append("-->");
            }

            public override void VisitText(HtmlText node) {
                string html;
                if (node.IsData) {
                    html = node.RawText;

                } else {
                    html = HtmlEncoder.Escape(node.RawText, settings.Charset.GetEncoder(), settings.EscapeMode);

                    if (settings.PrettyPrint
                        && node.Parent is HtmlElement
                        && !((HtmlElement) node.Parent).PreserveWhitespace) {
                        html = StringUtil.NormalizeWhitespace(html);
                    }

                    if (settings.PrettyPrint && node.NodePosition == 0
                        && node.Parent is HtmlElement && ((HtmlElement) node.Parent).Tag.FormatAsBlock && !node.IsBlank)
                        Indent();
                }

                output.Append(html);
            }

            private void Indent() {
                output.Append("\n").Append(' ' , depth * settings.Indent);
            }
        }
    }

}
