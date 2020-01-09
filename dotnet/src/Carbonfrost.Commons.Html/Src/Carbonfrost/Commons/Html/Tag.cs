//
// - Tag.cs -
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
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Html {

    [Builder(typeof(TagBuilder))]
    public class Tag : IEquatable<Tag> {

        private string tagName;
        internal bool _isBlock = true; // block or inline
        internal bool _formatAsBlock = true; // should be formatted as a block
        internal bool _canContainBlock = true; // Can this tag hold block level tags?
        internal bool _canContainInline = true; // only pcdata if not
        internal bool empty = false; // can hold nothing; e.g. img
        internal bool selfClosing = false; // can self close (<foo />). used for unknown tags that self close, without forcing them as empty.
        internal bool _preserveWhitespace = false; // for pre, textarea, script etc

        public bool CanContainBlock {
            get { return _canContainBlock; }
        }

        public string Name {
            get { return tagName; }
        }

        public bool IsInline {
            get {
                return !IsBlock;
            }
        }

        public bool IsData {
            get {
                return !_canContainInline && !IsEmpty;
            }
        }

        public bool IsEmpty {
            get {
                return empty;
            }
        }

        public bool IsBlock {
            get {
                return _isBlock;
            }
        }

        public bool FormatAsBlock {
            get {
                return _formatAsBlock;
            }
        }

        public bool IsSelfClosing {
            get {
                return empty || selfClosing;
            }
        }

        public bool PreserveWhitespace {
            get { return _preserveWhitespace; }
        }

        internal Tag(string tagName) {
            this.tagName = tagName.ToLowerInvariant();
        }

        [Obsolete("Use TagLibrary instead.")]
        public static Tag ValueOf(string tagName) {
            return TagLibrary.Html5.GetTag(tagName);
        }

        public override bool Equals(object obj) {
            Tag other = obj as Tag;
            return Equals(other);
        }

        public override int GetHashCode() {
            int hashCode = 0;
            unchecked {
                hashCode += 1000000007 * tagName.GetHashCode();
                hashCode += 1000000009 * _isBlock.GetHashCode();
                hashCode += 1000000021 * _formatAsBlock.GetHashCode();
                hashCode += 1000000033 * _canContainBlock.GetHashCode();
                hashCode += 1000000087 * _canContainInline.GetHashCode();
                hashCode += 1000000093 * empty.GetHashCode();
                hashCode += 1000000097 * selfClosing.GetHashCode();
                hashCode += 1000000103 * _preserveWhitespace.GetHashCode();
            }
            return hashCode;
        }

        public override string ToString() {
            return tagName;
        }

        public bool Equals(Tag other) {
            if (object.ReferenceEquals(this, other))
                return true;
            if (other == null)
                return false;

            return this.tagName == other.tagName
                && this._isBlock == other._isBlock
                && this._formatAsBlock == other._formatAsBlock
                && this._canContainBlock == other._canContainBlock
                && this._canContainInline == other._canContainInline
                && this.empty == other.empty
                && this.selfClosing == other.selfClosing
                && this._preserveWhitespace == other._preserveWhitespace;
        }
    }

}
