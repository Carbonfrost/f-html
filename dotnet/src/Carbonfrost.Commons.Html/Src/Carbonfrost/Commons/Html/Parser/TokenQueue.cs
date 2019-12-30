//
// - TokenQueue.cs -
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

namespace Carbonfrost.Commons.Html.Parser {

    class TokenQueue {

        private string queue;
        private int pos = 0;

        private const char ESC = '\\'; // escape char for chomp balanced.

        public TokenQueue(string data) {
            if (data == null)
                throw new ArgumentNullException("data");

            queue = data;
        }

        public bool IsEmpty {
            get {
                return RemainingLength == 0;
            }
        }

        private int RemainingLength {
            get {
                return queue.Length - pos;
            }
        }

        public char Peek() {
            return IsEmpty ? (char) 0 : queue[pos];
        }

        public void AddFirst(Token.Character c) {
            AddFirst(c.ToString());
        }

        public void AddFirst(string seq) {
            // not very performant, but an edge case
            queue = seq + queue.Substring(pos);
            pos = 0;
        }

        public bool Matches(string seq) {
            if (pos + seq.Length > queue.Length)
                return false;

            // Originally: _queue.RegionMatches(true, pos, seq, 0, seq.length());
            return queue.Substring(pos, seq.Length).Equals(seq, StringComparison.OrdinalIgnoreCase);
        }

        public bool MatchesCS(string seq) {
            return queue.Substring(pos).StartsWith(seq);
        }

        public bool MatchesAny(params string[] seq) {
            foreach (string s in seq) {
                if (Matches(s))
                    return true;
            }
            return false;
        }

        public bool MatchesAny(params char[] seq) {
            if (IsEmpty)
                return false;

            foreach (char c in seq) {
                if (queue[pos] == c)
                    return true;
            }
            return false;
        }

        public bool MatchesStartTag() {
            // micro opt for matching "<x"
            return (RemainingLength >= 2 && queue[pos] == '<' && char.IsLetter(queue[pos + 1]));
        }

        public bool MatchChomp(string seq) {
            if (Matches(seq)) {
                pos += seq.Length;
                return true;
            } else {
                return false;
            }
        }

        public bool MatchesWhitespace() {
            return !IsEmpty && StringUtil.IsWhitespace(queue[pos]);
        }

        public bool MatchesWord() {
            return !IsEmpty && char.IsLetterOrDigit(queue[pos]);
        }

        public void Advance() {
            if (!IsEmpty) pos++;
        }

        public char Consume() {
            return queue[pos++];
        }

        public void Consume(string seq) {
            if (!Matches(seq))
                throw HtmlFailure.QueueDidNotMatch();

            int len = seq.Length;
            if (len > RemainingLength)
            throw HtmlFailure.QueueDidNotLongEnough();

            pos += len;
        }

        public string ConsumeTo(string seq) {
            int offset = queue.IndexOf(seq, pos, StringComparison.Ordinal);
            if (offset != -1) {
                string consumed = queue.Substring(pos, offset - pos);
                pos += consumed.Length;
                return consumed;
            } else {
                return Remainder();
            }
        }

        public string ConsumeToIgnoreCase(string seq) {
            int start = pos;
            string first = seq.Substring(0, 1);
            bool canScan = first.ToLower().Equals(first.ToUpper()); // if first is not cased, use index of
            while (!IsEmpty) {
                if (Matches(seq))
                    break;

                if (canScan) {
                    int skip = queue.IndexOf(first, pos) - pos;
                    if (skip == 0) // this char is the skip char, but not match, so force advance of pos
                        pos++;
                    else if (skip < 0) // no chance of finding, grab to end
                        pos = queue.Length;
                    else
                        pos += skip;
                }
                else
                    pos++;
            }

            string data = queue.Substring(start, pos - start);
            return data;
        }

        // TODO: method name. not good that consumeTo cares for case, and consume to any doesn't. And the only use for this
        // is is a case sensitive time...
        public string ConsumeToAny(params string[] seq) {
            int start = pos;
            while (!IsEmpty && !MatchesAny(seq)) {
                pos++;
            }

            string data = queue.Substring(start, pos - start);
            return data;
        }

        public string ChompTo(string seq) {
            string data = ConsumeTo(seq);
            MatchChomp(seq);
            return data;
        }

        public string ChompToIgnoreCase(string seq) {
            string data = ConsumeToIgnoreCase(seq); // case insensitive scan
            MatchChomp(seq);
            return data;
        }

        public string ChompBalanced(char open, char close) {
            StringBuilder accum = new StringBuilder();
            int depth = 0;
            char last = (char) 0;

            do {
                if (IsEmpty) break;
                char c = Consume();
                if (last == 0 || last != ESC) {
                    if (c.Equals(open))
                        depth++;
                    else if (c.Equals(close))
                        depth--;
                }

                if (depth > 0 && last != 0)
                    accum.Append(c); // don't include the outer match pair in the return
                last = c;
            } while (depth > 0);
            return accum.ToString();
        }

        public static string Unescape(string in2) {
            StringBuilder out2 = new StringBuilder();
            char last = (char) 0;
            foreach (char c in in2.ToCharArray()) {
                if (c == ESC) {
                    if (last != 0 && last == ESC)
                        out2.Append(c);
                }
                else
                    out2.Append(c);
                last = c;
            }
            return out2.ToString();
        }

        public bool ConsumeWhitespace() {
            bool seen = false;
            while (MatchesWhitespace()) {
                pos++;
                seen = true;
            }
            return seen;
        }

        public string ConsumeWord() {
            int start = pos;
            while (MatchesWord())
                pos++;
            return queue.Substring(start, pos - start);
        }

        public string ConsumeTagName() {
            int start = pos;
            while (!IsEmpty && (MatchesWord() || MatchesAny(':', '_', '-')))
                pos++;

            return queue.Substring(start, pos - start);
        }

        public string ConsumeElementSelector() {
            int start = pos;
            while (!IsEmpty && (MatchesWord() || MatchesAny('|', '_', '-')))
                pos++;

            return queue.Substring(start, pos - start);
        }

        // http://www.w3.org/TR/CSS2/syndata.html#value-def-identifier
        public string ConsumeCssIdentifier() {
            int start = pos;
            while (!IsEmpty && (MatchesWord() || MatchesAny('-', '_')))
                pos++;

            return queue.Substring(start, pos - start);
        }

        public string ConsumeAttributeKey() {
            int start = pos;
            while (!IsEmpty && (MatchesWord() || MatchesAny('-', '_', ':')))
                pos++;

            return queue.Substring(start, pos - start);
        }

        public string Remainder() {
            StringBuilder accum = new StringBuilder();
            while (!IsEmpty) {
                accum.Append(Consume());
            }
            return accum.ToString();
        }

        public override string ToString() {
            return queue.Substring(pos);
        }
    }

}
