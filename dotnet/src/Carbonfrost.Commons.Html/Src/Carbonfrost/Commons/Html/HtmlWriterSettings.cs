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

using System;
using System.Text;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    public class HtmlWriterSettings : DomWriterSettings {

        internal static readonly HtmlWriterSettings Default = new HtmlWriterSettings();

        private EscapeMode _escapeMode = EscapeMode.Base;
        private Encoding _charset = Encoding.UTF8;
        private Encoder _charsetEncoder = Encoding.UTF8.GetEncoder();
        private bool _isXhtml;
        private int _indentAmount = 1;

        public EscapeMode EscapeMode {
            get {
                return _escapeMode;
            }
            set {
                ThrowIfReadOnly();
                _escapeMode = value;
            }
        }

        public Encoding Charset {
            get {
                return _charset;
            }
            set {
                ThrowIfReadOnly();
                // TODO: this should probably update the doc's meta charset
                _charset = value;
                _charsetEncoder = value.GetEncoder();
            }
        }

        public bool IsXhtml {
            get {
                return _isXhtml;
            }
            set {
                ThrowIfReadOnly();
                _isXhtml = value;
            }
        }

        public HtmlWriterSettings(HtmlWriterSettings settings) {
            if (settings != null) {
                IsXhtml = settings.IsXhtml;
                Charset = settings.Charset;
                EscapeMode = settings.EscapeMode;
                Indent = settings.Indent;
                IndentWidth = settings.IndentWidth;
            }
        }

        public HtmlWriterSettings() {
            Indent = true;
        }

        public static HtmlWriterSettings ReadOnly(HtmlWriterSettings settings) {
            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }
            return (HtmlWriterSettings) settings.CloneReadOnly();
        }

        public new HtmlWriterSettings Clone() {
            return (HtmlWriterSettings) base.Clone();
        }

        protected override DomWriterSettings CloneCore() {
            return new HtmlWriterSettings(this);
        }
    }
}
