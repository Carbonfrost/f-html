//
// Copyright 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

using System;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    public class HtmlElementDefinition : DomElementDefinition, IEquatable<HtmlElementDefinition> {

        private Flags _flags = Flags.CanContainBlock | Flags.CanContainInline | Flags.FormatAsBlock | Flags.IsBlock;

        // Can this tag hold block level tags?
        public bool CanContainBlock {
            get {
                return _flags.HasFlag(Flags.CanContainBlock);
            }
            set {
                ThrowIfReadOnly();
                SetFlags(Flags.CanContainBlock, value);
            }
        }

        // Element was encountered during parse and has no semantics in HTML 5
        internal bool IsUnknownTag {
            get {
                return _flags.HasFlag(Flags.IsUnknownTag);
            }
            set {
                ThrowIfReadOnly();
                SetFlags(Flags.IsUnknownTag, value);
            }
        }

        // only pcdata if not
        public bool CanContainInline {
            get {
                return _flags.HasFlag(Flags.CanContainInline);
            }
            set {
                ThrowIfReadOnly();
                SetFlags(Flags.CanContainInline, value);
            }
        }

        public bool IsInline {
            get {
                return !IsBlock;
            }
            set {
                ThrowIfReadOnly();
                IsBlock = !value;
            }
        }

        public bool IsData {
            get {
                return !CanContainInline && !IsEmpty;
            }
        }

        // block or inline
        public bool IsBlock {
            get {
                return _flags.HasFlag(Flags.IsBlock);
            }
            set {
                ThrowIfReadOnly();
                SetFlags(Flags.IsBlock, value);
            }
        }

        // should be formatted as a block
        public bool FormatAsBlock {
            get {
                return _flags.HasFlag(Flags.FormatAsBlock);
            }
            set {
                ThrowIfReadOnly();
                SetFlags(Flags.FormatAsBlock, value);
            }
        }

        internal HtmlElementDefinition(string tagName) : base(DomName.Create(tagName)) {
            ElementNodeType = typeof(HtmlElement);
        }

        public new HtmlElementDefinition Clone() {
            return (HtmlElementDefinition) base.Clone();
        }

        protected override DomNodeDefinition CloneCore() {
            return new HtmlElementDefinition(LocalName) {
                _flags = _flags,
                WhitespaceMode = WhitespaceMode,
                IsEmpty = IsEmpty,
                IsSelfClosing = IsSelfClosing,
                ElementNodeType = ElementNodeType,
            };
        }

        public override bool Equals(object obj) {
            HtmlElementDefinition other = obj as HtmlElementDefinition;
            return Equals(other);
        }

        public override int GetHashCode() {
            int hashCode = 0;
            unchecked {
                hashCode += 1000000007 * Name.GetHashCode();
                hashCode += 1000000009 * _flags.GetHashCode();
            }
            return hashCode;
        }

        public bool Equals(HtmlElementDefinition other) {
            if (object.ReferenceEquals(this, other)) {
                return true;
            }
            if (other == null) {
                return false;
            }

            return Name == other.Name && _flags == other._flags;
        }

        private void SetFlags(Flags flag, bool value) {
            _flags = value ? (_flags | flag) : (_flags & ~flag);
        }

        [Flags]
        enum Flags {
            None,
            CanContainBlock = 1 << 0,
            FormatAsBlock = 1 << 1,
            CanContainInline = 1 << 2,
            IsBlock = 1 << 3,
            IsUnknownTag = 1 << 4,
        }
    }

}
