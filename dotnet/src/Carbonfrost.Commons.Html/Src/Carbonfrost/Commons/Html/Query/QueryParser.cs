//
// - QueryParser.cs -
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
using System.Text;
using System.Text.RegularExpressions;

using Carbonfrost.Commons.Html.Parser;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html.Query {

    class QueryParser {

        private readonly static string[] combinators = { ",", ">", "+", "~", " " };

        private readonly TokenQueue tq;
        private readonly string query;
        private readonly List<Evaluator> evals = new List<Evaluator>();

        private QueryParser(string query) {
            this.query = query;
            this.tq = new TokenQueue(query);
        }

        public static Evaluator Parse(string query) {
            QueryParser p = new QueryParser(query);
            return p.Parse();
        }

        private Evaluator Parse() {
            tq.ConsumeWhitespace();

            if (tq.MatchesAny(combinators)) { // if starts with a combinator, use root as elements
                evals.Add(new StructuralEvaluator.Root());
                Combinator(tq.Consume());

            } else {
                FindElements();
            }

            while (!tq.IsEmpty) {
                // hierarchy and extras
                bool seenWhite = tq.ConsumeWhitespace();

                if (tq.MatchesAny(combinators)) {
                    Combinator(tq.Consume());
                } else if (seenWhite) {
                    Combinator(' ');
                } else { // E.class, E#id, E[attr] etc. AND
                    FindElements(); // take next el, #. etc off queue
                }
            }

            if (evals.Count == 1)
                return evals[0];

            return new CombiningEvaluator.And(evals);
        }

        private void Combinator(char combinator) {
            tq.ConsumeWhitespace();
            string subQuery = ConsumeSubQuery(); // support multi > childs

            Evaluator rootEval; // the new topmost evaluator
            Evaluator currentEval; // the evaluator the new eval will be combined to. could be root, or rightmost or.
            Evaluator newEval = Parse(subQuery); // the evaluator to add into target evaluator
            bool replaceRightMost = false;

            if (evals.Count == 1) {
                rootEval = currentEval = evals[0];
                // make sure OR (,) has precedence:
                if (rootEval is CombiningEvaluator.Or && combinator != ',') {
                    currentEval = ((CombiningEvaluator.Or) currentEval).RightMostEvaluator();
                    replaceRightMost = true;
                }
            }

            else {
                rootEval = currentEval = new CombiningEvaluator.And(evals);
            }
            evals.Clear();

            // for most combinators: change the current eval into an AND of the current eval and the new eval
            switch (combinator) {

                case '>':
                    currentEval = new CombiningEvaluator.And(newEval, new StructuralEvaluator.ImmediateParent(currentEval));
                    break;

                case ' ':
                    currentEval = new CombiningEvaluator.And(newEval, new StructuralEvaluator.Parent(currentEval));
                    break;

                case '+':
                    currentEval = new CombiningEvaluator.And(newEval, new StructuralEvaluator.ImmediatePreviousSibling(currentEval));
                    break;

                case '~':
                    currentEval = new CombiningEvaluator.And(newEval, new StructuralEvaluator.PreviousSibling(currentEval));
                    break;

                case ',': // group or.
                    CombiningEvaluator.Or or;
                    if (currentEval is CombiningEvaluator.Or) {
                        or = (CombiningEvaluator.Or) currentEval;
                        or.Add(newEval);

                    } else {
                        or = new CombiningEvaluator.Or();
                        or.Add(currentEval);
                        or.Add(newEval);
                    }
                    currentEval = or;
                    break;

                default:
                    throw HtmlFailure.UnknownCombinator(combinator);
            }

            if (replaceRightMost)
                ((CombiningEvaluator.Or) rootEval).ReplaceRightMostEvaluator(currentEval);
            else rootEval = currentEval;
            evals.Add(rootEval);
        }

        private string ConsumeSubQuery() {
            StringBuilder sq = new StringBuilder();
            while (!tq.IsEmpty) {
                if (tq.Matches("("))
                    sq.Append("(").Append(tq.ChompBalanced('(', ')')).Append(")");

                else if (tq.Matches("["))
                    sq.Append("[").Append(tq.ChompBalanced('[', ']')).Append("]");

                else if (tq.MatchesAny(combinators))
                    break;
                else
                    sq.Append(tq.Consume());
            }

            return sq.ToString();
        }

        private void FindElements() {
            if (tq.MatchChomp("#"))
                ById();

            else if (tq.MatchChomp("."))
                ByClass();

            else if (tq.MatchesWord())
                ByTag();

            else if (tq.Matches("["))
                byAttribute();

            else if (tq.MatchChomp("*"))
                AllElements();

            else if (tq.MatchChomp(":lt("))
                IndexLessThan();

            else if (tq.MatchChomp(":gt("))
                IndexGreaterThan();

            else if (tq.MatchChomp(":eq("))
                IndexEquals();

            else if (tq.Matches(":has("))
                Has();

            else if (tq.Matches(":contains("))
                Contains(false);

            else if (tq.Matches(":containsOwn("))
                Contains(true);

            else if (tq.Matches(":matches("))
                Matches(false);

            else if (tq.Matches(":matchesOwn("))
                Matches(true);

            else if (tq.Matches(":not("))
                Not();

            else // unhandled
                throw HtmlFailure.CouldNotParseQuery(query, tq.Remainder());

        }

        private void ById() {
            string id = tq.ConsumeCssIdentifier();
            if (id.Length == 0)
                throw Failure.EmptyString("id");

            evals.Add(new Evaluator.Id(id));
        }

        private void ByClass() {
            string className = tq.ConsumeCssIdentifier();
            if (className.Length == 0)
                throw Failure.EmptyString("className");

            evals.Add(new Evaluator.Class(className.Trim().ToLower()));
        }

        private void ByTag() {
            string tagName = tq.ConsumeElementSelector();
            if (tagName.Length == 0)
                throw Failure.EmptyString("tagName");

            // namespaces: if element name is "abc:def", selector must be "abc|def", so flip:
            if (tagName.Contains("|"))
                tagName = tagName.Replace("|", ":");

            evals.Add(new Evaluator.Tag(tagName.Trim().ToLower()));
        }

        private void byAttribute() {
            TokenQueue cq = new TokenQueue(tq.ChompBalanced('[', ']')); // content queue
            string key = cq.ConsumeToAny("=", "!=", "^=", "$=", "*=", "~="); // eq, not, start, end, contain, match, (no val)

            if (key.Length == 0)
                throw Failure.EmptyString("key");

            cq.ConsumeWhitespace();

            if (cq.IsEmpty) {
                if (key.StartsWith("^"))
                    evals.Add(new Evaluator.AttributeStarting(key.Substring(1)));
                else
                    evals.Add(new Evaluator.Attribute(key));
            } else {
                if (cq.MatchChomp("="))
                    evals.Add(new Evaluator.AttributeWithValue(key, cq.Remainder()));

                else if (cq.MatchChomp("!="))
                    evals.Add(new Evaluator.AttributeWithValueNot(key, cq.Remainder()));

                else if (cq.MatchChomp("^="))
                    evals.Add(new Evaluator.AttributeWithValueStarting(key, cq.Remainder()));

                else if (cq.MatchChomp("$="))
                    evals.Add(new Evaluator.AttributeWithValueEnding(key, cq.Remainder()));

                else if (cq.MatchChomp("*="))
                    evals.Add(new Evaluator.AttributeWithValueContaining(key, cq.Remainder()));

                else if (cq.MatchChomp("~="))
                    evals.Add(new Evaluator.AttributeWithValueMatching(key, new Regex(cq.Remainder())));
                else
                    throw HtmlFailure.CannotParseAttributeQuery(query, cq.Remainder());
            }
        }

        private void AllElements() {
            evals.Add(new Evaluator.AllElements());
        }

        // pseudo selectors :lt, :gt, :eq
        private void IndexLessThan() {
            evals.Add(new Evaluator.IndexLessThan(ConsumeIndex()));
        }

        private void IndexGreaterThan() {
            evals.Add(new Evaluator.IndexGreaterThan(ConsumeIndex()));
        }

        private void IndexEquals() {
            evals.Add(new Evaluator.IndexEquals(ConsumeIndex()));
        }

        private int ConsumeIndex() {
            string indexS = tq.ChompTo(")").Trim();
            return int.Parse(indexS);
        }

        // pseudo selector :has(el)
        private void Has() {
            tq.Consume(":has");
            string subQuery = tq.ChompBalanced('(', ')');

            // TODO Validation errors in a structured way
            if (subQuery.Length == 0)
                throw HtmlFailure.HasSelectorNotEmpty();

            evals.Add(new StructuralEvaluator.Has(Parse(subQuery)));
        }

        // TODO Consider these non-standard selectors

        // pseudo selector :contains(text), containsOwn(text)
        private void Contains(bool own) {
            tq.Consume(own ? ":containsOwn" : ":contains");
            string searchText = TokenQueue.Unescape(tq.ChompBalanced('(', ')'));

            if (searchText.Length == 0)
                throw HtmlFailure.ContainsSelectorNotEmpty();

            if (own)
                evals.Add(new Evaluator.ContainsOwnText(searchText));
            else
                evals.Add(new Evaluator.ContainsText(searchText));
        }

        // :matches(regex), matchesOwn(regex)
        private void Matches(bool own) {
            tq.Consume(own ? ":matchesOwn" : ":matches");
            string regex = tq.ChompBalanced('(', ')'); // don't unescape, as regex bits will be escaped

            if  (string.IsNullOrEmpty(regex))
                throw HtmlFailure.MatchesSelectorNotEmpty();

            if (own)
                evals.Add(new Evaluator.MatchesOwn(new Regex(regex)));
            else
                evals.Add(new Evaluator.MatchesImpl(new Regex(regex)));
        }

        // :not(selector)
        private void Not() {
            tq.Consume(":not");
            string subQuery = tq.ChompBalanced('(', ')');

            if (subQuery.Length == 0)
                throw HtmlFailure.NotSelectorSelectionCannotBeEmpty();

            evals.Add(new StructuralEvaluator.Not(Parse(subQuery)));
        }
    }

}
