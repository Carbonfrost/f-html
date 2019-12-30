//
// - CharacterReader.cs -
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
using System.Text.RegularExpressions;

namespace Carbonfrost.Commons.Html.Parser {

    class CharacterReader {

        // TODO Convert to use TextReader or possibly Stream with buffer type compatible with String methods
        // TODO Index line breaks so that we can generate FileLocations instead of positions in error messages

        public const char EOF = unchecked ((char) -1);
        private readonly string input;
        private readonly int length;
        private int _pos = 0;
        private int _mark = 0;

        public int Position {
            get {
                return _pos;
            }
        }

        public bool IsEmpty {
            get {
                return _pos >= length;
            }
        }

        public char Current {
            get {
                return IsEmpty ? EOF : input[_pos];
            }
        }

        public CharacterReader(string input) {
            if (input == null)
                throw new ArgumentNullException("input");

            input = Regex.Replace(input, "\r\n?", "\n"); // normalise carriage returns to newlines

            this.input = input;
            this.length = input.Length;
        }

        public char Consume() {
            char val = IsEmpty ? EOF : input[_pos];
            _pos++;
            return val;
        }

        public void Unconsume() {
            _pos--;
        }

        public void Advance() {
            _pos++;
        }

        public void Mark() {
            _mark = _pos;
        }

        public void RewindToMark() {
            _pos = _mark;
        }

        public string ConsumeAsString() {
            return input.Substring(_pos++, 1);
        }

        public string ConsumeTo(char c) {
            int offset = input.IndexOf(c, _pos);
            if (offset != -1) {
                string consumed = input.Substring(_pos, offset - _pos);
                _pos += consumed.Length;
                return consumed;
            } else {
                return ConsumeToEnd();
            }
        }

        public string ConsumeTo(string seq) {
            int offset = input.IndexOf(seq, this.Position);
            if (offset != -1) {
                string consumed = input.Substring(this.Position, offset - this.Position);
                _pos += consumed.Length;
                return consumed;

            } else {
                return ConsumeToEnd();
            }
        }

        public string ConsumeToAny(params char[] seq) {
            int start = _pos;

            while (!IsEmpty) {
                char c = input[_pos];
                foreach (char seek in seq) {
                    if (seek == c)
                        goto exit;
                }
                _pos++;
            }

        exit:
            return _pos > start ? input.Substring(start, _pos - start) : string.Empty;
        }

        public string ConsumeToEnd() {
            string data = input.Substring(this.Position);
            _pos = input.Length;
            return data;
        }

        public string ConsumeLetterSequence() {
            int start = this.Position;
            while (!IsEmpty) {
                char c = input[_pos];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    _pos++;
                else
                    break;
            }

            return input.Substring(start, _pos - start);
        }

        public string ConsumeLetterThenDigitSequence() {
            int start = _pos;
            while (!IsEmpty) {
                char c = input[_pos];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    _pos++;
                else
                    break;
            }

            while (!IsEmpty) {
                char c = input[_pos];
                if (c >= '0' && c <= '9')
                    _pos++;
                else
                    break;
            }

            return input.Substring(start, _pos - start);
        }

        public string ConsumeHexSequence() {
            int start = this.Position;
            while (!IsEmpty) {
                char c = input[_pos];
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'))
                    _pos++;
                else
                    break;
            }
            return input.Substring(start, _pos - start);
        }

        public string ConsumeDigitSequence() {
            int start = _pos;
            while (!IsEmpty) {
                char c = input[_pos];
                if (c >= '0' && c <= '9')
                    _pos++;
                else
                    break;
            }
            return input.Substring(start, _pos - start);
        }

        public bool Matches(char c) {
            return !IsEmpty && input[_pos] == c;
        }

        public bool Matches(string seq) {
            return input.Substring(_pos).StartsWith(seq);
        }

        public bool MatchesIgnoreCase(string seq) {
            return input.Substring(this.Position, seq.Length).Equals(seq, StringComparison.OrdinalIgnoreCase);
        }

        public bool MatchesAny(params char[] seq) {
            if (IsEmpty)
                return false;

            char c = input[_pos];
            foreach (char seek in seq) {
                if (seek == c)
                    return true;
            }
            return false;
        }

        public bool MatchesLetter() {
            if (IsEmpty)
                return false;

            char c = input[_pos];
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        public bool MatchesDigit() {
            if (IsEmpty)
                return false;

            char c = input[_pos];
            return (c >= '0' && c <= '9');
        }

        public bool MatchConsume(string seq) {
            if (Matches(seq)) {
                _pos += seq.Length;
                return true;

            } else {
                return false;
            }
        }

        public bool MatchConsumeIgnoreCase(string seq) {
            if (MatchesIgnoreCase(seq)) {
                _pos += seq.Length;
                return true;

            } else {
                return false;
            }
        }

        public bool ContainsIgnoreCase(string seq) {
            // used to check presence of </title>, </style>. only finds consistent case.
            return input.IndexOf(seq, _pos, StringComparison.OrdinalIgnoreCase) > -1;
        }

        public override string ToString() {
            return input.Substring(_pos);
        }
    }
}
