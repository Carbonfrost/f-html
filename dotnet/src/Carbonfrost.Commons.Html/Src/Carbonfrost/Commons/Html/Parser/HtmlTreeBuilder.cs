//
// - HtmlTreeBuilder.cs -
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
using System.Diagnostics;
using System.Linq;

namespace Carbonfrost.Commons.Html.Parser {

    class HtmlTreeBuilder : TreeBuilder {

        private HtmlTreeBuilderState _state; // the current state
        private HtmlTreeBuilderState _originalState; // original / marked state

        private bool baseUriSetFromDoc = false;
        private HtmlElement headElement; // the current head element
        private HtmlElement formElement; // the current form element
        private HtmlElement contextElement; // fragment parse context -- could be null even if fragment parsing
        private DescendableLinkedList<HtmlElement> formattingElements = new DescendableLinkedList<HtmlElement>(); // active (open) formatting elements
        private List<Token.Character> pendingTableCharacters = new List<Token.Character>(); // chars in table to be shifted out

        private bool _framesetOk = true; // if ok to go into frameset
        private bool fosterInserts = false; // if next inserts should be fostered
        private bool fragmentParsing = false; // if parsing a fragment of html

        public HtmlTreeBuilderState OriginalState {
            get {
                return _originalState;
            }
        }

        public HtmlElement HeadElement {
            get { return this.headElement; }
            internal set { this.headElement = value; }
        }

        public bool FramesetOK {
            get { return _framesetOk; }
            internal set { _framesetOk = value; }
        }

        public HtmlDocument Document {
            get {
                return doc;
            }
        }

        public Uri BaseUri {
            get {
                return baseUri;
            }
        }

        public HtmlTreeBuilder() {
        }

        public override HtmlDocument Parse(string input, Uri baseUri, HtmlParseErrorCollection errors) {
            _state = HtmlTreeBuilderState.Initial;
            return base.Parse(input, baseUri, errors);
        }

        public IList<HtmlNode> ParseFragment(string inputFragment, HtmlElement context, Uri baseUri, HtmlParseErrorCollection errors) {
            // context may be null
            InitialiseParse(inputFragment, baseUri, errors);
            contextElement = context;
            fragmentParsing = true;
            HtmlElement root = null;

            if (context != null) {
                if (context.OwnerDocument != null) // quirks setup:
                    doc.QuirksMode = context.OwnerDocument.QuirksMode;

                // initialise the tokeniser state:
                string contextTag = context.Tag.Name;

                switch (contextTag) {
                    case "title":
                    case "textarea":
                        tokeniser.Transition(TokeniserState.Rcdata);
                        break;

                    case "script":
                        tokeniser.Transition(TokeniserState.ScriptData);
                        break;

                    case "noscript":
                        tokeniser.Transition(TokeniserState.Data); // if scripting enabled, rawtext
                        break;

                    case "plaintext":
                        tokeniser.Transition(TokeniserState.Data);
                        break;

                    case "iframe":
                    case "noembed":
                    case "noframes":
                    case "style":
                    case "xmp":
                        tokeniser.Transition(TokeniserState.Rawtext);
                        break;

                    default:
                        tokeniser.Transition(TokeniserState.Data); // default
                        break;
                }

                root = new HtmlElement(Tag.ValueOf("html"), baseUri);
                doc.AppendChild(root);
                stack.AddFirst(root);
                ResetInsertionMode();
                // TODO: setup form element to nearest form on context (up ancestor chain)
            }

            RunParser();
            if (context != null)
                return root.ChildNodes;
            else
                return doc.ChildNodes;
        }

        public override bool Process(Token token) {
            currentToken = token;
            return this.State.Process(token, this);
        }

        public bool Process(Token token, HtmlTreeBuilderState state) {
            currentToken = token;
            return state.Process(token, this);
        }

        public void Transition(HtmlTreeBuilderState state) {
            this._state = state;
        }

        public HtmlTreeBuilderState State {
            get {
                return _state;
            }
        }

        public void MarkInsertionMode() {
            _originalState = _state;
        }

        public void MaybeSetBaseUri(HtmlElement base3) {
            if (baseUriSetFromDoc) // only listen to the first <base href> in parse
                return;

            // TODO Should be absolute URL
            string href = base3.Attribute("href");
            if (href.Length != 0) { // ignore <base target> etc
                baseUri = new Uri(href, UriKind.RelativeOrAbsolute);
                baseUriSetFromDoc = true;
                doc.BaseUri = new Uri(href, UriKind.RelativeOrAbsolute); // set on the doc so doc.createElement(Tag) will get updated base, and to update all descendants
            }
        }

        public bool IsFragmentParsing() {
            return fragmentParsing;
        }

        public void Error(HtmlTreeBuilderState state) {
            ParseError.UnexpectedToken(errors, reader.Position, state, currentToken);
        }

        public HtmlElement Insert(Token.StartTag startTag) {
            // handle empty unknown tags
            // when the spec expects an empty tag, will directly hit insertEmpty, so won't generate fake end tag.
            if (startTag.IsSelfClosing && !this.TagLibrary.IsKnownTag(startTag.Name)) {
                HtmlElement el = InsertEmpty(startTag);
                Process(new Token.EndTag(el.Tag.Name)); // ensure we get out of whatever state we are in
                return el;
            }


            HtmlElement elx = new HtmlElement(
                this.TagLibrary.GetTag(startTag.Name), baseUri, startTag.Attributes);
            Insert(elx);
            return elx;
        }

        public HtmlElement Insert(string startTagName) {
            HtmlElement el = new HtmlElement(Tag.ValueOf(startTagName), baseUri);
            Insert(el);
            return el;
        }

        public void Insert(HtmlElement el) {
            InsertNode(el);
            stack.AddLast(el);
        }

        public HtmlElement InsertEmpty(Token.StartTag startTag) {
            // TODO Use the semantic element here (via the factory)
            Tag tag = Tag.ValueOf(startTag.Name);
            HtmlElement el = new HtmlElement(tag, baseUri, startTag.Attributes);

            InsertNode(el);
            if (startTag.IsSelfClosing) {
                tokeniser.AcknowledgeSelfClosingFlag();

                if (!this.TagLibrary.IsKnownTag(tag)) // unknown tag, remember this is self closing for output
                    tag.selfClosing = true;
            }

            return el;
        }

        public void Insert(Token.Comment commentToken) {
            if (commentToken.IsBogus) {
                HtmlProcessingInstruction pi = new HtmlProcessingInstruction(commentToken, baseUri);
                InsertNode(pi);

            } else {
                HtmlComment comment = new HtmlComment(commentToken.Data, baseUri);
                InsertNode(comment);
            }
        }

        public void Insert(Token.Character characterToken) {
            HtmlNode node;
            // characters in script and style go in as datanodes, not text nodes
            if (CurrentElement.Tag.Name.In("script", "style"))
                node = new HtmlText(characterToken.Data, baseUri, true);
            else
                node = new HtmlText(characterToken.Data, baseUri);

            CurrentElement.AppendChild(node); // doesn't use insertNode, because we don't foster these; and will always have a stack.
        }

        private void InsertNode(HtmlNode node) {
            // if the stack hasn't been set up yet, elements (doctype, comments) go into the doc
            if (stack.Count == 0)
                doc.AppendChild(node);
            else if (IsFosterInserts())
                InsertInFosterParent(node);
            else
                CurrentElement.AppendChild(node);
        }

        public HtmlElement Pop() {
            // TODO - dev, remove validation check
            if (stack.Last.Value.NodeName.Equals("td") && !_state.Name.Equals("InCellState")) {
                HtmlWarning.PoppingTDNotInCell();
            }

            if (stack.Last.Value.NodeName.Equals("html")) {
                HtmlWarning.PoppingHtml();
            }

            HtmlElement last = stack.Last.Value;
            stack.RemoveLast();
            return last;
        }

        public void Push(HtmlElement element) {
            stack.AddFirst(element);
        }

        public DescendableLinkedList<HtmlElement> Stack {
            get {
                return stack;
            }
        }

        public bool OnStack(HtmlElement el) {
            return IsElementInQueue(stack, el);
        }

        private bool IsElementInQueue(DescendableLinkedList<HtmlElement> queue, HtmlElement element) {
            return queue.Contains(element);
        }

        public HtmlElement GetFromStack(string elName) {
            var it = stack.GetDescendingEnumerator();
            while (it.MoveNext()) {
                HtmlElement next = it.Current;

                if (next.NodeName.Equals(elName)) {
                    return next;
                }
            }
            return null;
        }

        public bool RemoveFromStack(HtmlElement el) {
            return stack.Remove(el);
        }

        public void PopStackToClose(string elName) {
            while (stack.Last != null) {
                if (stack.Last.Value.NodeName.Equals(elName)) {
                    stack.RemoveLast();
                    break;

                } else {
                    stack.RemoveLast();
                }
            }
        }

        public void PopStackToClose(params string[] elNames) {
            while (stack.Last != null) {
                if (elNames.Contains(stack.Last.Value.NodeName)) {
                    stack.RemoveLast();
                    break;

                } else {
                    stack.RemoveLast();
                }
            }
        }

        public void PopStackToBefore(string elName) {
            while (stack.Last != null) {
                if (stack.Last.Value.NodeName.Equals(elName)) {
                    break;

                } else {
                    stack.RemoveLast();
                }
            }
        }

        public void ClearStackToTableContext() {
            ClearStackToContext("table");
        }

        public void ClearStackToTableBodyContext() {
            ClearStackToContext("tbody", "tfoot", "thead");
        }

        public void ClearStackToTableRowContext() {
            ClearStackToContext("tr");
        }

        private void ClearStackToContext(params string[] nodeNames) {
            LinkedListNode<HtmlElement> node = stack.Last;
            while (node != null) {
                HtmlElement next = node.Value;
                if (nodeNames.Contains(next.NodeName) || next.NodeName.Equals("html")) {
                    break;

                } else {
                    stack.Remove(node);
                    node = node.Previous;
                }
            }
        }

        public HtmlElement AboveOnStack(HtmlElement el) {
            if (!OnStack(el))
                HtmlWarning.ElementShouldBeOnStack();
            var it = stack.GetDescendingEnumerator();
            while (it.MoveNext()) {
                HtmlElement next = it.Current;

                if (next == el) {
                    it.MoveNext();
                    return it.Current;
                }
            }
            return null;
        }

        public void InsertOnStackAfter(HtmlElement after, HtmlElement in2) {
            stack.AddAfter(stack.Find(after), in2);
        }

        public void ReplaceOnStack(HtmlElement out2, HtmlElement in2) {
            ReplaceInQueue(stack, out2, in2);
        }

        private void ReplaceInQueue(DescendableLinkedList<HtmlElement> queue, HtmlElement out2, HtmlElement in2) {
            queue.AddAfter(queue.Find(out2), in2);
            queue.Remove(out2);
        }

        public void ResetInsertionMode() {
            bool last = false;
            var it = stack.GetDescendingEnumerator();

            while (it.MoveNext()) {
                HtmlElement node = it.Current;
                if (stack.FindLast(node).Previous == null) {
                    last = true;
                    node = contextElement;
                }

                string name = node.NodeName;
                if ("select".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InSelect);
                    break; // frag

                } else if (("td".Equals(name)
                            || "td".Equals(name) && !last)) {
                    Transition(HtmlTreeBuilderState.InCell);
                    break;

                } else if ("tr".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InRow);
                    break;

                } else if ("tbody".Equals(name)
                           || "thead".Equals(name)
                           || "tfoot".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InTableBody);
                    break;

                } else if ("caption".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InCaption);
                    break;

                } else if ("colgroup".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InColumnGroup);
                    break; // frag

                } else if ("table".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InTable);
                    break;

                } else if ("head".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InBody);
                    break; // frag

                } else if ("body".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InBody);
                    break;

                } else if ("frameset".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InFrameset);
                    break; // frag

                } else if ("html".Equals(name)) {
                    Transition(HtmlTreeBuilderState.BeforeHead);
                    break; // frag

                } else if (last) {
                    Transition(HtmlTreeBuilderState.InBody);
                    break; // frag
                }
            }
        }

        // TODO: tidy up in specific scope methods
        private bool InSpecificScope(string targetName, string[] baseTypes, string[] extraTypes) {
            return InSpecificScope(new string[]{targetName}, baseTypes, extraTypes);
        }

        private bool InSpecificScope(string[] targetNames, string[] baseTypes, string[] extraTypes) {
            IEnumerator<HtmlElement> it = stack.GetDescendingEnumerator();
            while (it.MoveNext()) {
                HtmlElement el = it.Current;
                string elName = el.NodeName;
                if (targetNames.Contains(elName))
                    return true;

                if (baseTypes.Contains(elName))
                    return false;

                if (extraTypes != null && extraTypes.Contains(elName))
                    return false;
            }

            HtmlWarning.ShouldNotBeReachable();
            return false;
        }

        public bool InScope(string[] targetNames) {
            return InSpecificScope(targetNames, new string[]{"applet", "caption", "html", "table", "td", "th", "marquee", "object"}, null);
        }

        public bool InScope(string targetName) {
            return InScope(targetName, null);
        }

        public bool InScope(string targetName, string[] extras) {
            return InSpecificScope(targetName, new string[]{"applet", "caption", "html", "table", "td", "th", "marquee", "object"}, extras);
            // TODO: in mathml namespace: mi, mo, mn, ms, mtext annotation-xml
            // TODO: in svg namespace: forignOjbect, desc, title
        }

        public bool InListItemScope(string targetName) {
            return InScope(targetName, new string[]{"ol", "ul"});
        }

        public bool InButtonScope(string targetName) {
            return InScope(targetName, new string[]{"button"});
        }

        public bool InTableScope(string targetName) {
            return InSpecificScope(targetName, new string[]{"html", "table"}, null);
        }

        public bool InSelectScope(string targetName) {
            IEnumerator<HtmlElement> it = stack.GetDescendingEnumerator();

            while (it.MoveNext()) {
                HtmlElement el = it.Current;
                string elName = el.NodeName;
                if (elName.Equals(targetName)) {
                    return true;
                }

                if (!StringUtil.In(elName, "optgroup", "option")) // all elements except
                {
                    return false;
                }
            }

            HtmlWarning.ShouldNotBeReachable();
            return false;
        }

        public bool IsFosterInserts() {
            return fosterInserts;
        }

        public void SetFosterInserts(bool fosterInserts) {
            this.fosterInserts = fosterInserts;
        }

        public HtmlElement FormElement {
            get {
                return formElement;
            }
            set {
                this.formElement = value;
            }
        }

        public void NewPendingTableCharacters() {
            pendingTableCharacters = new List<Token.Character>();
        }

        public List<Token.Character> GetPendingTableCharacters() {
            return pendingTableCharacters;
        }

        public void SetPendingTableCharacters(List<Token.Character> pendingTableCharacters) {
            this.pendingTableCharacters = pendingTableCharacters;
        }

        /**
             11.2.5.2 Closing elements that have implied end tags<p/>
             When the steps below require the UA to generate implied end tags, then, while the current node is a dd element, a
             dt element, an li element, an option element, an optgroup element, a p element, an rp element, or an rt element,
             the UA must pop the current node off the stack of open elements.

             @param excludeTag If a step requires the UA to generate implied end tags but lists an element to exclude from the
             process, then the UA must perform the above steps as if that element was not in the above list.
         */
        public void GenerateImpliedEndTags(string excludeTag) {
            while ((excludeTag != null && !CurrentElement.NodeName.Equals(excludeTag)) &&
                   StringUtil.In(CurrentElement.NodeName, "dd", "dt", "li", "option", "optgroup", "p", "rp", "rt"))
                Pop();
        }

        public void GenerateImpliedEndTags() {
            GenerateImpliedEndTags(null);
        }

        public bool IsSpecial(HtmlElement el) {
            // TODO: mathml's mi, mo, mn
            // TODO: svg's foreigObject, desc, title
            string name = el.NodeName;
            return StringUtil.Hash(@"address applet area article aside base basefont bgsound
                                   blockquote body br button caption center col colgroup command dd
                                   details dir div dl dt embed fieldset figcaption figure footer for
                                   frame frameset h1 h2 h3 h4 h5 h6 head header hgroup hr html
                                   iframe img input isindex li link listing marquee menu meta nav
                                   noembed noframes noscript object ol p param plaintext pre script
                                   section select style summary table tbody td textarea tfoot th thead
                                   title tr ul wbr xmp").Contains(name);
        }

        // active formatting elements
        public void PushActiveFormattingElements(HtmlElement in2) {
            int numSeen = 0;
            var iter = formattingElements.GetDescendingEnumerator();
            while (iter.MoveNext()) {
                HtmlElement el =  iter.Current;
                if (el == null) // marker
                    break;

                if (IsSameFormattingElement(in2, el))
                    numSeen++;

                if (numSeen == 3) {
                    iter.Remove();
                    break;
                }
            }

            formattingElements.AddFirst(in2);
        }

        private bool IsSameFormattingElement(HtmlElement a, HtmlElement b) {
            // same if: same namespace, tag, and attributes. Element.Equals only checks tag, might in future check children

            return a.NodeName.Equals(b.NodeName) &&
                // a.namespace().equals(b.namespace()) &&
                a.Attributes.Equals(b.Attributes);
            // TODO: namespaces
        }

        public void ReconstructFormattingElements() {
            int size = formattingElements.Count;

            if (size == 0 || formattingElements.Last.Value == null || OnStack(formattingElements.Last.Value)) {
                return;
            }

            HtmlElement entry = formattingElements.Last.Value;
            int pos = size - 1;
            bool skip = false;
            while (true) {
                if (pos == 0) // step 4. if none before, skip to 8
                {
                    skip = true;
                    break;
                }
                entry = formattingElements.ElementAt(--pos); // step 5. one earlier than entry
                if (entry == null || OnStack(entry)) // step 6 - neither marker nor on stack
                {
                    break; // jump to 8, else continue back to 4
                }
            }

            while (true) {
                if (!skip) // step 7: on later than entry
                {
                    entry = formattingElements.ElementAt(++pos);
                }

                if (entry == null) {
                    HtmlWarning.ReconstructUnexpectedlyEmpty();
                    // should not occur, as we break at last element
                }

                // 8. create new element from element, 9 insert into current node, onto stack
                skip = false; // can only skip increment from 4.
                HtmlElement newEl = Insert(entry.NodeName); // todo: avoid fostering here?
                // newEl.namespace(entry.namespace()); // todo: namespaces
                newEl.Attributes.AddAll(entry.Attributes);

                // 10. replace entry with new entry
                formattingElements.AddBefore(formattingElements.Find(entry), newEl);
                formattingElements.Remove(entry);

                // 11
                if (pos == size - 1) // if not last entry in list, jump to 7
                {
                    break;
                }
            }
        }

        public void ClearFormattingElementsToLastMarker() {
            while (!formattingElements.IsEmpty()) {
                HtmlElement el = formattingElements.Last.Value;
                formattingElements.RemoveLast();
                if (el == null)
                    break;
            }
        }

        public void RemoveFromActiveFormattingElements(HtmlElement el) {
            var it = formattingElements.GetDescendingEnumerator();
            while (it.MoveNext()) {
                HtmlElement next = it.Current;

                if (next == el) {
                    it.Remove();
                    break;
                }
            }
        }

        public bool IsInActiveFormattingElements(HtmlElement el) {
            return IsElementInQueue(formattingElements, el);
        }

        public HtmlElement GetActiveFormattingElement(string nodeName) {
            var it = formattingElements.GetDescendingEnumerator();
            while (it.MoveNext()) {
                HtmlElement next = it.Current;

                if (next == null) // scope marker
                    break;
                else if (next.NodeName.Equals(nodeName))
                    return next;
            }
            return null;
        }

        public void ReplaceActiveFormattingElement(HtmlElement out2, HtmlElement in2) {
            ReplaceInQueue(formattingElements, out2, in2);
        }

        public void InsertMarkerToFormattingElements() {
            formattingElements.AddLast(new LinkedListNode<HtmlElement>(null));
        }

        public void InsertInFosterParent(HtmlNode input) {
            HtmlElement fosterParent = null;
            HtmlElement lastTable = GetFromStack("table");
            bool isLastTableParent = false;

            if (lastTable != null) {
                if (lastTable.Parent != null) {
                    fosterParent = lastTable.Parent;
                    isLastTableParent = true;

                } else
                    fosterParent = AboveOnStack(lastTable);

            } else { // no table == frag
                fosterParent = stack.ElementAt(0);
            }

            if (isLastTableParent) {
                // last table cannot be null by this point.
                if (lastTable == null)
                    HtmlWarning.FosterParentTableUnexpectedlyNull();

                lastTable.Before(input);
            }
            else
                fosterParent.AppendChild(input);
        }

        public override string ToString() {
            return string.Concat(
                "TreeBuilder {",
                "currentToken=", currentToken,
                ", state=", State,
                ", currentElement=", CurrentElement,
                '}');
        }
    }

}
