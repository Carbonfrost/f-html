//
// - StructuralEvaluator.cs -
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

namespace Carbonfrost.Commons.Html.Query {

    abstract class StructuralEvaluator : Evaluator {

        private Evaluator evaluator;

        public class Root : Evaluator {

            public override bool Matches(HtmlElement root, HtmlElement element) {
                return root == element;
            }
        }

        public class Has : StructuralEvaluator {

            public Has(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(HtmlElement root, HtmlElement element) {
                foreach (HtmlElement e in element.GetAllElements()) {
                    if (e != element && evaluator.Matches(root, e))
                        return true;

                }
                return false;
            }

            public override string ToString() {
                return string.Format(":has({0})", evaluator);
            }
        }

        public class Not : StructuralEvaluator {

            public Not(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(HtmlElement root, HtmlElement node) {
                return !evaluator.Matches(root, node);
            }

            public override string ToString() {
                return string.Format(":not{0}", evaluator);
            }
        }

        public class Parent : StructuralEvaluator {

            public Parent(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(HtmlElement root, HtmlElement element) {
                if (root == element)
                    return false;

                HtmlElement parent = element.Parent;
                while (parent != root) {
                    if (evaluator.Matches(root, parent))
                        return true;
                    parent = parent.Parent;
                }
                return false;
            }

            public override string ToString() {
                return string.Format(":parent{0}", evaluator);
            }
        }

        public class ImmediateParent : StructuralEvaluator {

            public ImmediateParent(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(HtmlElement root, HtmlElement element) {
                if (root == element)
                    return false;

                HtmlElement parent = element.Parent;
                return parent != null && evaluator.Matches(root, parent);
            }

            public override string ToString() {
                return string.Format(":ImmediateParent{0}", evaluator);
            }
        }

        public class PreviousSibling : StructuralEvaluator {

            public PreviousSibling(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(HtmlElement root, HtmlElement element) {
                if (root == element)
                    return false;

                HtmlElement prev = element.PreviousElementSibling;

                while (prev != null) {
                    if (evaluator.Matches(root, prev))
                        return true;

                    prev = prev.PreviousElementSibling;
                }
                return false;
            }

            public override string ToString() {
                return string.Format(":prev*{0}", evaluator);
            }
        }

        public class ImmediatePreviousSibling : StructuralEvaluator {

            public ImmediatePreviousSibling(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(HtmlElement root, HtmlElement element) {
                if (root == element)
                    return false;

                HtmlElement prev = element.PreviousElementSibling;
                return prev != null && evaluator.Matches(root, prev);
            }

            public override string ToString() {
                return string.Format(":prev{0}", evaluator);
            }
        }
    }

}
