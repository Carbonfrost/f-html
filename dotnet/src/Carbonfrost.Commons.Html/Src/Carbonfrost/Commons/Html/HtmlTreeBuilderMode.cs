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
using System.Collections.Generic;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Html.Parser;

namespace Carbonfrost.Commons.Html {

    public struct HtmlTreeBuilderMode : IPseudoEnum<HtmlTreeBuilderMode> {

        private readonly int _value;
        private readonly string _name;
        private static HtmlTreeBuilderMode[] _allValues = {};

        public static readonly HtmlTreeBuilderMode Html5 = new HtmlTreeBuilderMode(nameof(Html5));
        public static readonly HtmlTreeBuilderMode Xml = new HtmlTreeBuilderMode(nameof(Xml));

        public HtmlTreeBuilderMode(string name) {
            if (string.IsNullOrEmpty(name)) {
                throw Failure.NullOrEmptyString(nameof(name));
            }

            if (TryParse(name, out HtmlTreeBuilderMode existing)) {
                this = existing;
                return;
            }

            int index = _allValues.Length;
            Array.Resize(ref _allValues, index + 1);

            _name = name;
            _value = index;
            _allValues[index] = this;
        }

        public static HtmlTreeBuilderMode Parse(string text) {
            HtmlTreeBuilderMode result;
            var ex = _TryParse(text, out result);
            if (ex != null) {
                throw ex;
            }

            return result;
        }

        internal TreeBuilder CreateTreeBuilder() {
            if (this == Xml) {
                return new XmlTreeBuilder();
            }
            return new HtmlTreeBuilder();
        }

        public static bool TryParse(string text, out HtmlTreeBuilderMode result) {
            return _TryParse(text, out result) == null;
        }

        private static Exception _TryParse(string text, out HtmlTreeBuilderMode result) {
            result = default(HtmlTreeBuilderMode);
            if (text == null) {
                return new ArgumentNullException(nameof(text));
            }

            text = text.Trim();
            if (text.Length == 0) {
                return Failure.AllWhitespace(nameof(text));
            }

            var existing = Array.FindIndex(_allValues, m => m._name == text);
            bool exists = existing >= 0;
            if (exists) {
                result = _allValues[existing];
                return null;
            }
            return Failure.NotParsable(nameof(text), typeof(HtmlTreeBuilderMode));
        }

        public static IEnumerable<HtmlTreeBuilderMode> GetValues() {
            return new [] {
                Html5,
                Xml,
            };
        }

        public static string GetName(HtmlTreeBuilderMode value) {
            return value._name;
        }

        public static IReadOnlyList<string> GetNames() {
            return Array.ConvertAll(_allValues, v => v._name);
        }

        public int ToInt32() {
            return _value;
        }

        public bool Equals(HtmlTreeBuilderMode other) {
            return _value == other._value;
        }

        public int CompareTo(HtmlTreeBuilderMode other) {
            return _value.CompareTo(other._value);
        }

        TypeCode IConvertible.GetTypeCode() {
            return TypeCode.Int32;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider) {
            return Convert.ToBoolean(_value, provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider) {
            return Convert.ToByte(_value, provider);
        }

        char IConvertible.ToChar(IFormatProvider provider) {
            return Convert.ToChar(_value, provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider) {
            return Convert.ToDateTime(_value, provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider) {
            return Convert.ToDecimal(_value, provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider) {
            return Convert.ToDouble(_value, provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider) {
            return Convert.ToInt16(_value, provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider) {
            return ToInt32();
        }

        long IConvertible.ToInt64(IFormatProvider provider) {
            return Convert.ToInt64(_value, provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider) {
            return Convert.ToSByte(_value, provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider) {
            return Convert.ToSingle(_value, provider);
        }

        string IConvertible.ToString(IFormatProvider provider) {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
            return Convert.ChangeType(_value, conversionType, provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider) {
            return Convert.ToUInt16(_value, provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider) {
            return Convert.ToUInt32(_value, provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider) {
            return Convert.ToUInt64(_value, provider);
        }

        public override string ToString() {
            return _name == null ? _value.ToString() : _name;
        }

        public static bool operator ==(HtmlTreeBuilderMode x, HtmlTreeBuilderMode y) {
            return x._value == y._value;
        }

        public static bool operator !=(HtmlTreeBuilderMode x, HtmlTreeBuilderMode y) {
            return x._value != y._value;
        }

        public string ToString(string format, IFormatProvider formatProvider) {
            switch (format) {
                case "G":
                case "g":
                    return _name;
                case "X":
                case "x":
                case "F":
                case "f":
                case "D":
                case "d":
                    return _value.ToString(format);
                default:
                    throw new FormatException();
            }
        }

        public override int GetHashCode() {
            int hashCode = 707945396;
            hashCode = hashCode * -1521134295 + _value.GetHashCode();
            hashCode = hashCode * -1521134295 + _name.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj) {
            return obj is HtmlTreeBuilderMode y && Equals(y);
        }
    }
}
