//
// Copyright 2012, 2019 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Html {

    public static class HtmlEncoder {

        // Maps entities as UTF-32 codepoints
        private static readonly IDictionary<string, int> full;
        private static readonly IDictionary<int, string> xhtmlByVal;
        private static readonly IDictionary<int, string> baseByVal;
        private static readonly IDictionary<int, string> fullByVal;
        private static readonly Regex unescapePattern = new Regex("&(#(x|X)?([0-9a-fA-F]+)|[a-zA-Z]+\\d*);?");
        private static readonly Regex strictUnescapePattern = new Regex("&(#(x|X)?([0-9a-fA-F]+)|[a-zA-Z]+\\d*);");

        public static bool IsNamedEntity(string name) {
            return full.ContainsKey(name);
        }

        public static string GetCharacterByName(string name) {
            return char.ConvertFromUtf32(GetUtf32CodepointByName(name));
        }

        public static int GetUtf32CodepointByName(string name) {
            return full.GetValueOrDefault(name);
        }

        public static string Escape(string text) {
            return Escape(text, Encoding.UTF8.GetEncoder(), EscapeMode.Extended);
        }

        public static string Escape(string text, Encoder encoder, EscapeMode escapeMode) {
            if (encoder == null) {
                throw new ArgumentNullException("encoder");
            }

            // TODO Might use Encoding instead of encoder for parameter because of the
            // needed API : Encoding.Clone

            StringBuilder accum = new StringBuilder(text.Length * 2);
            IDictionary<int, string> map = escapeMode.GetMap();

            for (int pos = 0; pos < text.Length; pos++) {
                // TODO: char doesnt cover all UTF32 codepoints
                // Need StringInfo.GetTextElementEnumerator(text)
                char c = text[pos];
                if (map.ContainsKey(c))
                    accum.Append('&').Append(map.GetValueOrDefault(c)).Append(';');

                else if (true) { // UNDONE encoder.canEncode(c))
                    accum.Append(c);
                } else {
                    accum.Append("&#").Append((int) c).Append(';');
                }
            }

            return accum.ToString();
        }

        public static string Unescape(string text) {
            return Unescape(text, false);
        }

        public static string Unescape(string text, bool strict) {
            // TODO: change this method to use Tokeniser.consumeCharacterReference
            if (!text.Contains("&")) {
                return text;
            }

            Regex pattern = strict ? strictUnescapePattern : unescapePattern;
            MatchEvaluator evaluator = (m) => {

                // &(#(x|X)?([0-9a-fA-F]+)|[a-zA-Z]\\d*);?
                int charval = -1;
                string num = m.Groups[3].Value;

                if (num.Length > 0) {
                    try {
                        int base2 = m.Groups[2].Length > 0 ? 16 : 10; // 2 is hex indicator
                        charval = Convert.ToInt32(num, base2);

                    } catch (OverflowException) {
                    } catch (FormatException) {
                    } // skip

                } else {
                    string name = m.Groups[1].Value;
                    if (full.ContainsKey(name)) {
                        charval = full[name];
                    }
                }

                if (charval != -1 || charval > 0xFFFF) { // out of range
                    return ((char) charval).ToString();

                } else {
                    return Regex.Escape(m.Groups[0].Value); // replace with original string
                }
            };

            return pattern.Replace(text, evaluator);
        }

        // xhtml has restricted entities
        private static readonly IDictionary<string, int> xhtml = new Dictionary<string, int> {
            { "quot", 0x00022 },
            { "amp", 0x00026 },
            { "apos", 0x00027 },
            { "lt", 0x0003C },
            { "gt", 0x0003E }
        };

        static HtmlEncoder() {
            baseByVal = ToCharacterKey(LoadEntities("entities-base.properties")); // most common / default
            full = LoadEntities("entities-full.properties"); // extended and overblown.
            fullByVal = ToCharacterKey(full);
            xhtmlByVal = new Dictionary<int, string>();

            foreach (var entity in xhtml) {
                xhtmlByVal.Add(entity.Value, entity.Key);
            }
        }

        private static IDictionary<string, int> LoadEntities(string filename) {
            using (Stream source = typeof(HtmlEncoder).GetTypeInfo().Assembly.GetManifestResourceStream(
                    "Carbonfrost.Commons.Html.Resources." + filename
            )) {
                IEnumerable<KeyValuePair<string, object>> properties = Properties.FromStream(source);

                Dictionary<string, int> entities = new Dictionary<string, int>();

                foreach (var entry in properties) {
                    int val = Convert.ToInt32(entry.Value.ToString(), 16);
                    string name = entry.Key;
                    entities[name] = val;
                }

                return entities;
            }
        }

        private static IDictionary<int, string> ToCharacterKey(IDictionary<string, int> inMap) {
            IDictionary<int, string> outMap = new Dictionary<int, string>();

            foreach (var entry in inMap) {
                int char2 = entry.Value;
                string name = entry.Key;

                if (outMap.ContainsKey(char2)) {
                    // dupe, prefer the lower case version
                    if (name.ToLowerInvariant().Equals(name))
                        outMap[char2] = name;

                } else {
                    outMap[char2] = name;
                }
            }
            return outMap;
        }

        private static IDictionary<int, string> GetMap(this EscapeMode mode) {
            switch (mode) {
                case EscapeMode.Xhtml:
                    return xhtmlByVal;

                case EscapeMode.Base:
                    return baseByVal;

                case EscapeMode.Extended:
                default:
                    return fullByVal;
            }
        }

    }
}
