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

namespace Carbonfrost.Commons.Html {

    static class StringUtil {

        public static bool In(this string name, string item0, string item1) {
            return name == item0 || name == item1;
        }

        public static bool In(this string name, string item0, string item1, string item2) {
            return name == item0 || name == item1 || name == item2;
        }

        public static bool In(this string name, string item0, string item1, string item2, string item3) {
            return name == item0 || name == item1 || name == item2 || name == item3;
        }

        public static bool In(this string name, string item0, string item1, string item2, string item3, string item4) {
            return name == item0 || name == item1 || name == item2 || name == item3 || name == item4;
        }

        public static bool IsWhitespace(char c) {
            return c == ' ' || c == '\t' || c == '\n' || c == '\f' || c == '\r';
        }

        public static bool IsBlank(string text) {
            if (string.IsNullOrEmpty(text))
                return true;

            int l = text.Length;
            foreach (char c in text) {
                if (!StringUtil.IsWhitespace(c))
                    return false;
            }

            return true;
        }

        internal static void AppendNormalisedText(StringBuilder accum, HtmlText textNode, bool preserveWhitespace) {
            string text = textNode.Data;

            if (!preserveWhitespace) {
                text = StringUtil.NormalizeWhitespace(text);

                if (HtmlText.LastCharIsWhitespace(accum))
                    text = HtmlText.StripLeadingWhitespace(text);
            }
            accum.Append(text);
        }

        internal static void AppendWhitespaceIfBr(HtmlElement element, StringBuilder accum) {
            if (element.NodeName == "br" && !HtmlText.LastCharIsWhitespace(accum))
                accum.Append(" ");
        }

        // Removes non-significant whitespace
        public static string NormalizeWhitespace(string text) {
            StringBuilder sb = new StringBuilder(text.Length);

            bool lastWasWhitespace = false;
            bool modified = false;

            int l = text.Length;
            for (int i = 0; i < l; i++) {
                char c = text[i];

                if (IsWhitespace((char) c)) {
                    if (lastWasWhitespace) {
                        modified = true;
                        continue;
                    }

                    if (c != ' ')
                        modified = true;
                    sb.Append(' ');
                    lastWasWhitespace = true;
                }

                else {
                    sb.Append(c);
                    lastWasWhitespace = false;
                }
            }

            return modified ? sb.ToString() : text;
        }
    }
}
