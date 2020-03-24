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

    public class HtmlReaderSettings : DomReaderSettings {

        public static readonly HtmlReaderSettings Default = ReadOnly(
            new HtmlReaderSettings()
        );

        private bool _keepEntityReferences;
        private HtmlTreeBuilderMode _mode;
        private Uri _baseUri;
        private string _context;

        public bool KeepEntityReferences {
            get {
                return _keepEntityReferences;
            }
            set {
                ThrowIfReadOnly();
                _keepEntityReferences = value;
            }
        }

        public HtmlTreeBuilderMode Mode {
            get {
                return _mode;
            }
            set {
                ThrowIfReadOnly();
                _mode = value;
            }
        }

        public Uri BaseUri {
            get {
                return _baseUri;
            }
            set {
                ThrowIfReadOnly();
                _baseUri = value;
            }
        }

        public string Context {
            get {
                return _context;
            }
            set {
                ThrowIfReadOnly();
                _context = value;
            }
        }

        internal HtmlElement ContextElement {
            get {
                // Create a context element for the fragment based on the name from the
                // settings
                string contextName = Context;
                if (string.IsNullOrEmpty(Context)) {
                    contextName = "body";
                }
                return new HtmlElement(contextName);
            }
        }

        public HtmlReaderSettings()
            : this(null)
        {
        }

        public HtmlReaderSettings(HtmlReaderSettings settings) {
            if (settings == null) {
                MaxErrors = -1;
            } else {
                MaxErrors = settings.MaxErrors;
                KeepEntityReferences = settings.KeepEntityReferences;
                Mode = settings.Mode;
                BaseUri = settings.BaseUri;
                Context = settings.Context;
            }
        }

        public static HtmlReaderSettings ReadOnly(HtmlReaderSettings settings) {
            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }
            return (HtmlReaderSettings) settings.CloneReadOnly();
        }

        public new HtmlReaderSettings Clone() {
            return (HtmlReaderSettings) base.Clone();
        }

        protected override DomReaderSettings CloneCore() {
            return new HtmlReaderSettings(this);
        }
    }
}
