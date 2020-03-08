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
using Carbonfrost.Commons.Web.Dom;

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

        private void InsertNode(DomNode node) {
            this.CurrentElement.Append(node);
        }

        HtmlElement Insert(Token.StartTag startTag) {
            HtmlElementDefinition tag = TagLibrary.GetTag(startTag.Name);

            // TODO: wonder if for xml parsing, should treat all tags as unknown? because it's not html.
            HtmlElement el = new HtmlElement(startTag.Name, baseUri, startTag.Attributes);
            InsertNode(el);

            if (startTag.IsSelfClosing) {
                tokeniser.AcknowledgeSelfClosingFlag();
                // TODO Change to schema is not ideal
                if (!tag.IsReadOnly && tag.IsUnknownTag) { // unknown tag, remember this is self closing for output. see above.
                    tag.IsSelfClosing = true;
                    tag.IsEmpty = true;
                }

            } else {
                stack.AddLast(el);
            }
            return el;
        }

        void Insert(Token.Comment commentToken) {
            if (commentToken.IsBogus) {
                var comment = HtmlProcessingInstruction.Create(commentToken, baseUri);
                InsertNode(comment);

            } else {
                var comment = doc.CreateComment(commentToken.Data);
                InsertNode(comment);
            }
        }

        void Insert(Token.Character characterToken) {
            var node = new HtmlText(characterToken.Data, baseUri);
            InsertNode(node);
        }

        void Insert(Token.Doctype d) {
            var doctypeNode = doc.CreateDocumentType(
                d.Name,
                d.PublicIdentifier,
                d.SystemIdentifier
            );
            doctypeNode.BaseUri = baseUri;
            InsertNode(doctypeNode);
        }

        private void PopStackToClose(Token.EndTag endTag) {
            String elName = endTag.Name;
            DomContainer firstFound = null;

            var it = stack.GetDescendingEnumerator();
            while (it.MoveNext()) {
                DomContainer next = it.Current;
                if (next.NodeName.Equals(elName)) {
                    firstFound = next;
                    break;
                }
            }
            if (firstFound == null)
                return; // not found, skip

            it = stack.GetDescendingEnumerator();
            while (it.MoveNext()) {
                DomContainer next = it.Current;
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
