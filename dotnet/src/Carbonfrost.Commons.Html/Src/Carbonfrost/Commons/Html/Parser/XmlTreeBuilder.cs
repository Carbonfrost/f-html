//
// - XmlTreeBuilder.cs -
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

namespace Carbonfrost.Commons.Html.Parser {

    class XmlTreeBuilder : TreeBuilder {

        protected override void InitialiseParse(String input, Uri baseUri, HtmlParseErrorCollection errors) {
            base.InitialiseParse(input, baseUri, errors);
            stack.AddLast(doc); // place the document onto the stack. differs from HtmlTreeBuilder (not on stack)
        }

        public override bool Process(Token token) {
            // start tag, end tag, doctype, comment, character, eof
            switch (token.Type) {
                case TokenType.StartTag:
                    Insert(token.AsStartTag());
                    break;

                case TokenType.EndTag:
                    PopStackToClose(token.AsEndTag());
                    break;

                case TokenType.Comment:
                    Insert(token.AsComment());
                    break;

                case TokenType.Character:
                    Insert(token.AsCharacter());
                    break;

                case TokenType.Doctype:
                    Insert(token.AsDoctype());
                    break;

                case TokenType.EOF: // could put some normalisation here if desired
                    break;

                default:
                    HtmlWarning.UnexpectedTokenType();
                    break;
            }
            return true;
        }

        private void InsertNode(HtmlNode node) {
            this.CurrentElement.AppendChild(node);
        }

        HtmlElement Insert(Token.StartTag startTag) {
            Tag tag = Tag.ValueOf(startTag.Name);

            // TODO: wonder if for xml parsing, should treat all tags as unknown? because it's not html.
            HtmlElement el = new HtmlElement(tag, baseUri, startTag.Attributes);
            InsertNode(el);

            if (startTag.IsSelfClosing) {
                tokeniser.AcknowledgeSelfClosingFlag();
                if (!this.TagLibrary.IsKnownTag(tag)) // unknown tag, remember this is self closing for output. see above.
                    tag.selfClosing = true;

            } else {
                stack.AddLast(el);
            }
            return el;
        }

        void Insert(Token.Comment commentToken) {
            if (commentToken.IsBogus) {
                HtmlProcessingInstruction comment = new HtmlProcessingInstruction(commentToken, baseUri);
                InsertNode(comment);

            } else {
                HtmlComment comment = new HtmlComment(commentToken.Data, baseUri);
                InsertNode(comment);
            }
        }

        void Insert(Token.Character characterToken) {
            HtmlNode node = new HtmlText(characterToken.Data, baseUri);
            InsertNode(node);
        }

        void Insert(Token.Doctype d) {
            HtmlDocumentType doctypeNode = new HtmlDocumentType(
                d.Name,
                d.PublicIdentifier,
                d.SystemIdentifier,
                baseUri);
            InsertNode(doctypeNode);
        }

        private void PopStackToClose(Token.EndTag endTag) {
            String elName = endTag.Name;
            HtmlElement firstFound = null;

            var it = stack.GetDescendingEnumerator();
            while (it.MoveNext()) {
                HtmlElement next = it.Current;
                if (next.NodeName.Equals(elName)) {
                    firstFound = next;
                    break;
                }
            }
            if (firstFound == null)
                return; // not found, skip

            it = stack.GetDescendingEnumerator();
            while (it.MoveNext()) {
                HtmlElement next = it.Current;
                if (next == firstFound) {
                    it.Remove();
                    break;

                } else {
                    it.Remove();
                }
            }
        }

    }
}
