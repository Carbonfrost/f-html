//
// - HtmlTreeBuilderState.InBodyState.cs -
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

namespace Carbonfrost.Commons.Html.Parser {


    abstract partial class HtmlTreeBuilderState {

        class InBodyState : HtmlTreeBuilderState {

            public override bool Process(Token t, HtmlTreeBuilder tb) {

                switch (t.Type) {
                    case TokenType.Character:

                        Token.Character c = t.AsCharacter();
                        if (c.Data.Equals(NullString)) {
                            // TODO: confirm that check
                            tb.Error(this);
                            return false;

                        } else if (IsWhitespace(c)) {
                            tb.ReconstructFormattingElements();
                            tb.Insert(c);

                        } else {
                            tb.ReconstructFormattingElements();
                            tb.Insert(c);
                            tb.FramesetOK = false;
                        }
                        break;

                    case TokenType.Comment:
                        tb.Insert(t.AsComment());
                        break;

                    case TokenType.Doctype:
                        tb.Error(this);
                        return false;

                    case TokenType.StartTag:
                        bool? result = HandleStartTag(t, tb);
                        if (result.HasValue)
                            return result.Value;

                        break;

                    case TokenType.EndTag:
                        result = HandleEndTag(t, tb);
                        if (result.HasValue)
                            return result.Value;

                        break;

                    case TokenType.EOF:
                        // TODO: error if stack contains something not dd, dt, li, p, tbody, td, tfoot, th, thead, tr, body, html
                        // stop parsing
                        break;
                }

                return true;
            }

            private bool? HandleEndTag(Token t, HtmlTreeBuilder tb) {
                Token.EndTag endTag = t.AsEndTag();
                string name = endTag.Name;

                if (name.Equals("body")) {
                    if (!tb.InScope("body")) {
                        tb.Error(this);
                        return false;

                    } else {
                        // TODO: error if stack contains something not dd, dt, li, optgroup, option, p, rp, rt, tbody, td, tfoot, th, thead, tr, body, html
                        tb.Transition(AfterBody);
                    }

                } else if (name.Equals("html")) {
                    bool notIgnored = tb.Process(new Token.EndTag("body"));
                    if (notIgnored)
                        return tb.Process(endTag);

                } else if (StringUtil.In(name,
                                         "address", "article", "aside", "blockquote", "button", "center", "details", "dir", "div",
                                         "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup", "listing", "menu",
                                         "nav", "ol", "pre", "section", "summary", "ul")) {
                    // TODO: refactor these lookups
                    if (!tb.InScope(name)) {
                        // nothing to close
                        tb.Error(this);
                        return false;
                    } else {
                        tb.GenerateImpliedEndTags();
                        if (!tb.CurrentElement.NodeName.Equals(name))
                            tb.Error(this);
                        tb.PopStackToClose(name);
                    }

                } else if (name.Equals("form")) {
                    HtmlElement currentForm = tb.FormElement;
                    tb.FormElement = null;

                    if (currentForm == null || !tb.InScope(name)) {
                        tb.Error(this);
                        return false;
                    } else {
                        tb.GenerateImpliedEndTags();
                        if (!tb.CurrentElement.NodeName.Equals(name))
                            tb.Error(this);
                        // remove currentForm from stack. will shift anything under up.
                        tb.RemoveFromStack(currentForm);
                    }

                } else if (name.Equals("p")) {
                    if (!tb.InButtonScope(name)) {
                        tb.Error(this);
                        tb.Process(new Token.StartTag(name)); // if no p to close, creates an empty <p></p>
                        return tb.Process(endTag);
                    } else {
                        tb.GenerateImpliedEndTags(name);
                        if (!tb.CurrentElement.NodeName.Equals(name))
                            tb.Error(this);
                        tb.PopStackToClose(name);
                    }

                } else if (name.Equals("li")) {
                    if (!tb.InListItemScope(name)) {
                        tb.Error(this);
                        return false;
                    } else {
                        tb.GenerateImpliedEndTags(name);
                        if (!tb.CurrentElement.NodeName.Equals(name))
                            tb.Error(this);
                        tb.PopStackToClose(name);
                    }

                } else if (StringUtil.In(name, "dd", "dt")) {
                    if (!tb.InScope(name)) {
                        tb.Error(this);
                        return false;

                    } else {
                        tb.GenerateImpliedEndTags(name);
                        if (!tb.CurrentElement.NodeName.Equals(name))
                            tb.Error(this);
                        tb.PopStackToClose(name);
                    }

                } else if (StringUtil.In(name, "h1", "h2", "h3", "h4", "h5", "h6")) {
                    if (!tb.InScope(new string[]{"h1", "h2", "h3", "h4", "h5", "h6"})) {
                        tb.Error(this);
                        return false;

                    } else {
                        tb.GenerateImpliedEndTags(name);
                        if (!tb.CurrentElement.NodeName.Equals(name))
                            tb.Error(this);
                        tb.PopStackToClose("h1", "h2", "h3", "h4", "h5", "h6");
                    }

                } else if (name.Equals("sarcasm")) {
                    // *sigh*
                    return AnyOtherEndTag(t, tb);

                } else if (StringUtil.In(name,
                                         "a", "b", "big", "code", "em", "font", "i", "nobr", "s", "small", "strike", "strong", "tt", "u")) {

                    // Adoption Agency Algorithm.
                OUTER:
                    for (int i = 0; i < 8; i++) {
                        HtmlElement formatEl = tb.GetActiveFormattingElement(name);
                        if (formatEl == null)
                            return AnyOtherEndTag(t, tb);

                        else if (!tb.OnStack(formatEl)) {
                            tb.Error(this);
                            tb.RemoveFromActiveFormattingElements(formatEl);
                            return true;

                        } else if (!tb.InScope(formatEl.NodeName)) {
                            tb.Error(this);
                            return false;

                        } else if (tb.CurrentElement != formatEl)
                            tb.Error(this);

                        HtmlElement furthestBlock = null;
                        HtmlElement commonAncestor = null;
                        bool seenFormattingElement = false;
                        DescendableLinkedList<HtmlElement> stack = tb.Stack;

                        for (int si = 0; si < stack.Count; si++) {
                            HtmlElement el = stack.ElementAt(si);
                            if (el == formatEl) {
                                commonAncestor = stack.ElementAt(si - 1);
                                seenFormattingElement = true;

                            } else if (seenFormattingElement && tb.IsSpecial(el)) {
                                furthestBlock = el;
                                break;
                            }
                        }

                        if (furthestBlock == null) {
                            tb.PopStackToClose(formatEl.NodeName);
                            tb.RemoveFromActiveFormattingElements(formatEl);
                            return true;
                        }

                        // TODO: Let a bookmark note the position of the formatting element in the list of active formatting elements relative to the elements on either side of it in the list.
                        // does that mean: int pos of format el in list?
                        HtmlElement node = furthestBlock;
                        HtmlElement lastNode = furthestBlock;

                    INNER:
                        for (int j = 0; j < 3; j++) {
                        continueINNER:
                            if (tb.OnStack(node))
                                node = tb.AboveOnStack(node);

                            if (!tb.IsInActiveFormattingElements(node)) { // note no bookmark check
                                tb.RemoveFromStack(node);
                                goto continueINNER;
                            } else if (node == formatEl)
                                goto breakINNER;

                            HtmlElement replacement = new HtmlElement(Tag.ValueOf(node.NodeName), tb.BaseUri);
                            tb.ReplaceActiveFormattingElement(node, replacement);
                            tb.ReplaceOnStack(node, replacement);
                            node = replacement;

                            if (lastNode == furthestBlock) {
                                // TODO: move the aforementioned bookmark to be immediately after the new node in the list of active formatting elements.
                                // not getting how this bookmark both straddles the element above, but is inbetween here...
                            }
                            if (lastNode.Parent != null)
                                lastNode.Remove();
                            node.AppendChild(lastNode);

                            lastNode = node;
                        }
                    breakINNER:

                        if (StringUtil.In(commonAncestor.NodeName, "table", "tbody", "tfoot", "thead", "tr")) {
                            if (lastNode.Parent != null)
                                lastNode.Remove();
                            tb.InsertInFosterParent(lastNode);

                        } else {
                            if (lastNode.Parent != null)
                                lastNode.Remove();
                            commonAncestor.AppendChild(lastNode);
                        }

                        HtmlElement adopter = new HtmlElement(Tag.ValueOf(name), tb.BaseUri);
                        HtmlNode[] childNodes = furthestBlock.ChildNodes.ToArray();
                        foreach (HtmlNode childNode in childNodes) {
                            adopter.AppendChild(childNode); // append will reparent. thus the clone to avvoid concurrent mod.
                        }

                        furthestBlock.AppendChild(adopter);
                        tb.RemoveFromActiveFormattingElements(formatEl);
                        // TODO: insert the new element into the list of active formatting elements at the position of the aforementioned bookmark.
                        tb.RemoveFromStack(formatEl);
                        tb.InsertOnStackAfter(furthestBlock, adopter);
                    }

                } else if (StringUtil.In(name, "applet", "marquee", "object")) {
                    if (!tb.InScope("name")) {

                        if (!tb.InScope(name)) {
                            tb.Error(this);
                            return false;
                        }

                        tb.GenerateImpliedEndTags();
                        if (!tb.CurrentElement.NodeName.Equals(name))
                            tb.Error(this);
                        tb.PopStackToClose(name);
                        tb.ClearFormattingElementsToLastMarker();
                    }

                } else if (name.Equals("br")) {
                    tb.Error(this);
                    tb.Process(new Token.StartTag("br"));
                    return false;

                } else {
                    return AnyOtherEndTag(t, tb);
                }

                return null;
            }

            private Nullable<bool> HandleStartTag(Token t, HtmlTreeBuilder tb) {
                Token.StartTag startTag = t.AsStartTag();
                string name = startTag.Name;

                if (name.Equals("html")) {
                    tb.Error(this);
                    // merge attributes onto real html
                    HtmlElement html = tb.Stack.First();
                    foreach (HtmlAttribute attribute in startTag.Attributes) {
                        if (!html.HasAttribute(attribute.Name))
                            html.Attributes.Add(attribute);
                    }

                } else if (StringUtil.In(name, "base", "basefont", "bgsound", "command", "link", "meta", "noframes", "script", "style", "title")) {
                    return tb.Process(t, InHead);

                } else if (name.Equals("body")) {
                    tb.Error(this);
                    DescendableLinkedList<HtmlElement> stack = tb.Stack;
                    if (stack.Count == 1 || (stack.Count > 2 && !stack.ElementAt(1).NodeName.Equals("body"))) {
                        // only in fragment case
                        return false; // ignore

                    } else {
                        tb.FramesetOK = false;
                        HtmlElement body = stack.First();
                        foreach (HtmlAttribute attribute in startTag.Attributes) {
                            if (!body.HasAttribute(attribute.Name))
                                body.Attributes.Add(attribute);
                        }
                    }

                } else if (name.Equals("frameset")) {
                    tb.Error(this);
                    DescendableLinkedList<HtmlElement> stack = tb.Stack;
                    if (stack.Count == 1 || (stack.Count > 2 && !stack.ElementAt(1).NodeName.Equals("body"))) {
                        // only in fragment case
                        return false; // ignore

                    } else if (!tb.FramesetOK) {
                        return false; // ignore frameset

                    } else {
                        HtmlElement second = stack.ElementAt(1);
                        if (second.Parent != null)
                            second.Remove();

                        // pop up to html element
                        while (stack.Count > 1)
                            stack.RemoveLast();

                        tb.Insert(startTag);
                        tb.Transition(InFrameset);
                    }

                } else if (StringUtil.Hash(
                    @"address article aside blockquote center details dir div dl
                                           fieldset figcaption figure footer header hgroup menu nav ol
                                           p section summary ul").Contains(name)) {
                    if (tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }
                    tb.Insert(startTag);

                } else if (StringUtil.Hash("h1 h2 h3 h4 h5 h6").Contains(name)) {
                    if (tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }

                    if (StringUtil.Hash("h1 h2 h3 h4 h5 h6").Contains(tb.CurrentElement.NodeName)) {
                        tb.Error(this);
                        tb.Pop();
                    }
                    tb.Insert(startTag);

                } else if (StringUtil.In(name, "pre", "listing")) {
                    if (tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }

                    tb.Insert(startTag);
                    // TODO: ignore LF if next token
                    tb.FramesetOK = false;

                } else if (name.Equals("form")) {
                    if (tb.FormElement != null) {
                        tb.Error(this);
                        return false;
                    }
                    if (tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }

                    HtmlElement form = tb.Insert(startTag);
                    tb.FormElement = form;

                } else if (name.Equals("li")) {
                    tb.FramesetOK = false;

                    DescendableLinkedList<HtmlElement> stack = tb.Stack;
                    for (int i = stack.Count - 1; i > 0; i--) {
                        HtmlElement el = stack.ElementAt(i); // TODO Performance of this?
                        if (el.NodeName.Equals("li")) {
                            tb.Process(new Token.EndTag("li"));
                            break;
                        }

                        if (tb.IsSpecial(el) && !StringUtil.In(el.NodeName, "address", "div", "p"))
                            break;
                    }
                    if (tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }
                    tb.Insert(startTag);

                } else if (StringUtil.In(name, "dd", "dt")) {
                    tb.FramesetOK = false;
                    DescendableLinkedList<HtmlElement> stack = tb.Stack;
                    for (int i = stack.Count - 1; i > 0; i--) {
                        HtmlElement el = stack.ElementAt(i);
                        if (StringUtil.In(el.NodeName, "dd", "dt")) {
                            tb.Process(new Token.EndTag(el.NodeName));
                            break;
                        }
                        if (tb.IsSpecial(el) && !StringUtil.In(el.NodeName, "address", "div", "p"))
                            break;
                    }

                    if (tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }
                    tb.Insert(startTag);

                } else if (name.Equals("plaintext")) {
                    if (tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }
                    tb.Insert(startTag);
                    tb.tokeniser.Transition(TokeniserState.PLAINTEXT); // once in, never gets out

                } else if (name.Equals("button")) {
                    if (tb.InButtonScope("button")) {
                        // close and reprocess
                        tb.Error(this);
                        tb.Process(new Token.EndTag("button"));
                        tb.Process(startTag);
                    } else {
                        tb.ReconstructFormattingElements();
                        tb.Insert(startTag);
                        tb.FramesetOK = false;
                    }

                } else if (name.Equals("a")) {
                    if (tb.GetActiveFormattingElement("a") != null) {
                        tb.Error(this);
                        tb.Process(new Token.EndTag("a"));

                        // still on stack?
                        HtmlElement remainingA = tb.GetFromStack("a");
                        if (remainingA != null) {
                            tb.RemoveFromActiveFormattingElements(remainingA);
                            tb.RemoveFromStack(remainingA);
                        }
                    }
                    tb.ReconstructFormattingElements();
                    HtmlElement a = tb.Insert(startTag);
                    tb.PushActiveFormattingElements(a);

                } else if (StringUtil.In(name,
                                         "b", "big", "code", "em", "font", "i", "s", "small", "strike", "strong", "tt", "u")) {
                    tb.ReconstructFormattingElements();
                    HtmlElement el = tb.Insert(startTag);
                    tb.PushActiveFormattingElements(el);

                } else if (name.Equals("nobr")) {
                    tb.ReconstructFormattingElements();
                    if (tb.InScope("nobr")) {
                        tb.Error(this);
                        tb.Process(new Token.EndTag("nobr"));
                        tb.ReconstructFormattingElements();
                    }
                    HtmlElement el = tb.Insert(startTag);
                    tb.PushActiveFormattingElements(el);

                } else if (StringUtil.In(name, "applet", "marquee", "object")) {
                    tb.ReconstructFormattingElements();
                    tb.Insert(startTag);
                    tb.InsertMarkerToFormattingElements();
                    tb.FramesetOK = false;

                } else if (name.Equals("table")) {
                    if (tb.Document.QuirksMode != QuirksMode.Quirks && tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }
                    tb.Insert(startTag);
                    tb.FramesetOK = false;
                    tb.Transition(InTable);

                } else if (StringUtil.In(name, "area", "br", "embed", "img", "keygen", "wbr")) {
                    tb.ReconstructFormattingElements();
                    tb.InsertEmpty(startTag);
                    tb.FramesetOK = false;

                } else if (name.Equals("input")) {
                    tb.ReconstructFormattingElements();
                    HtmlElement el = tb.InsertEmpty(startTag);

                    if (!el.Attribute("type").Equals("hidden", StringComparison.OrdinalIgnoreCase))
                        tb.FramesetOK = false;

                } else if (StringUtil.In(name, "param", "source", "track")) {
                    tb.InsertEmpty(startTag);

                } else if (name.Equals("hr")) {
                    if (tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }
                    tb.InsertEmpty(startTag);
                    tb.FramesetOK = false;

                } else if (name.Equals("image")) {
                    // we're not supposed to ask.
                    startTag.Name = "img";
                    return tb.Process(startTag);

                } else if (name.Equals("isindex")) {
                    // how much do we care about the early 90s?
                    tb.Error(this);
                    if (tb.FormElement != null)
                        return false;

                    tb.tokeniser.AcknowledgeSelfClosingFlag();
                    tb.Process(new Token.StartTag("form"));

                    if (startTag.Attributes.Contains("action")) {
                        HtmlElement form = tb.FormElement;
                        form.Attribute("action", startTag.Attributes["action"]);
                    }

                    tb.Process(new Token.StartTag("hr"));
                    tb.Process(new Token.StartTag("label"));

                    // hope you like english.
                    string prompt = startTag.Attributes.Contains("prompt") ?
                        startTag.Attributes["prompt"] :
                        "This is a searchable index. Enter search keywords: ";

                    tb.Process(new Token.Character(prompt));

                    // input
                    var inputStToken = new Token.StartTag("input");
                    HtmlAttributeCollection inputAttribs = inputStToken.Attributes;
                    inputAttribs["name"] = "isindex";

                    foreach (HtmlAttribute attr in startTag.Attributes) {
                        if (!StringUtil.In(attr.Name, "name", "action", "prompt"))
                            inputAttribs.Add(attr);
                    }

                    tb.Process(inputStToken);
                    tb.Process(new Token.EndTag("label"));
                    tb.Process(new Token.StartTag("hr"));

                    tb.Process(new Token.EndTag("form"));

                } else if (name.Equals("textarea")) {
                    tb.Insert(startTag);
                    // TODO: If the next token is a U+000A LINE FEED (LF) char token, then ignore that token and move on to the next one. (Newlines at the start of textarea elements are ignored as an authoring convenience.)
                    tb.tokeniser.Transition(TokeniserState.Rcdata);
                    tb.MarkInsertionMode();
                    tb.FramesetOK = false;
                    tb.Transition(Text);

                } else if (name.Equals("xmp")) {
                    if (tb.InButtonScope("p")) {
                        tb.Process(new Token.EndTag("p"));
                    }
                    tb.ReconstructFormattingElements();
                    tb.FramesetOK = false;
                    HandleRawtext(startTag, tb);

                } else if (name.Equals("iframe")) {
                    tb.FramesetOK = false;
                    HandleRawtext(startTag, tb);

                } else if (name.Equals("noembed")) {
                    // also handle noscript if script enabled
                    HandleRawtext(startTag, tb);

                } else if (name.Equals("select")) {

                    tb.ReconstructFormattingElements();
                    tb.Insert(startTag);
                    tb.FramesetOK = false;

                    HtmlTreeBuilderState state = tb.State;
                    if (state.Equals(InTable) || state.Equals(InCaption) || state.Equals(InTableBody) || state.Equals(InRow) || state.Equals(InCell))
                        tb.Transition(InSelectInTable);
                    else
                        tb.Transition(InSelect);

                } else if (StringUtil.In(name, "optgroup", "option")) {
                    if (tb.CurrentElement.NodeName.Equals("option"))
                        tb.Process(new Token.EndTag("option"));
                    tb.ReconstructFormattingElements();
                    tb.Insert(startTag);

                } else if (StringUtil.In(name, "rp", "rt")) {
                    if (tb.InScope("ruby")) {
                        tb.GenerateImpliedEndTags();
                        if (!tb.CurrentElement.NodeName.Equals("ruby")) {
                            tb.Error(this);
                            tb.PopStackToBefore("ruby"); // i.e. close up to but not include name
                        }
                        tb.Insert(startTag);
                    }

                } else if (name.Equals("math")) {
                    tb.ReconstructFormattingElements();
                    // TODO: handle A start tag whose tag name is "math" (i.e. foreign, mathml)
                    tb.Insert(startTag);
                    tb.tokeniser.AcknowledgeSelfClosingFlag();

                } else if (name.Equals("svg")) {
                    tb.ReconstructFormattingElements();
                    // TODO: handle A start tag whose tag name is "svg" (xlink, svg)
                    tb.Insert(startTag);
                    tb.tokeniser.AcknowledgeSelfClosingFlag();

                } else if (StringUtil.Hash("caption col colgroup frame head tbody td tfoot th thead tr").Contains(name)) {
                    tb.Error(this);
                    return false;
                } else {
                    tb.ReconstructFormattingElements();
                    tb.Insert(startTag);
                }

                return null;
            }

            bool AnyOtherEndTag(Token t, HtmlTreeBuilder tb) {
                string name = t.AsEndTag().Name;
                DescendableLinkedList<HtmlElement> stack = tb.Stack;
                var it = stack.GetDescendingEnumerator();

                while (it.MoveNext()) {
                    HtmlElement node = it.Current;
                    if (node.NodeName.Equals(name)) {
                        tb.GenerateImpliedEndTags(name);
                        if (!name.Equals(tb.CurrentElement.NodeName))
                            tb.Error(this);
                        tb.PopStackToClose(name);
                        break;

                    } else {
                        if (tb.IsSpecial(node)) {
                            tb.Error(this);
                            return false;
                        }
                    }
                }

                return true;
            }
        }

    }
}
