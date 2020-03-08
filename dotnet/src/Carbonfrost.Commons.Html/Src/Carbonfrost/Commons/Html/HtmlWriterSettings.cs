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

using System.Text;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    public class HtmlWriterSettings {

        internal static readonly HtmlWriterSettings Default = new HtmlWriterSettings();

        private bool isSealed;
        private EscapeMode escapeMode = EscapeMode.Base;
        private Encoding _charset = Encoding.UTF8;
        private Encoder _charsetEncoder = Encoding.UTF8.GetEncoder();
        private bool _prettyPrint = true;
        private int _indentAmount = 1;

        public bool PrettyPrint {
            get {
                return _prettyPrint;
            }
            set {
                _prettyPrint = value;
            }
        }

        public int Indent {
            get {
                return _indentAmount;
            }
            set {
                if (value < 0)
                    throw Failure.Negative("value", value);

                this._indentAmount = value;
            }
        }

        public EscapeMode EscapeMode {
            get {
                return escapeMode;
            }
            set {
                ThrowIfSealed();
                this.escapeMode = value;
            }
        }

        public Encoding Charset {
            get {
                return _charset;
            }
            set {
                // TODO: this should probably update the doc's meta charset
                this._charset = value;
                _charsetEncoder = value.GetEncoder();
            }
        }

        public bool IsXhtml {
            get; set;
        }

        public HtmlWriterSettings Clone() {
            HtmlWriterSettings result = (HtmlWriterSettings) MemberwiseClone();
            return result;
        }

        void ThrowIfSealed() {
            if (this.isSealed)
                throw Failure.Sealed();
        }

        internal void Seal() {
            this.isSealed = true;
        }
    }
}
