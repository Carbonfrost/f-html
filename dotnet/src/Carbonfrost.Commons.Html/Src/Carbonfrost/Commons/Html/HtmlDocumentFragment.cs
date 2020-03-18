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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    public class HtmlDocumentFragment : DomDocumentFragment, IHtmlNode, IHtmlLoader<HtmlDocumentFragment> {

        public string OuterHtml {
            get {
                return InnerHtml;
            }
            set {
                InnerHtml = value;
            }
        }

        public string InnerHtml {
            get {
                return this.GetInnerHtml();
            }
            set {
                // TODO Should use OwnerDocument to create and load this
                var frag = new HtmlDocumentFragment();
                frag.LoadHtml(value);
                Empty();
                Append(frag.ChildNodes);
            }
        }

        public override DomNodeType NodeType {
            get {
                return DomNodeType.DocumentFragment;
            }
        }

        public static HtmlDocumentFragment Parse(string html) {
            return Parse(html, null);
        }

        public static HtmlDocumentFragment Parse(string html, HtmlReaderSettings settings) {
            return new HtmlDocumentFragment().LoadHtml(html, settings);
        }

        public HtmlDocumentFragment LoadHtml(string html) {
            return LoadHtml(html, null);
        }

        public new HtmlDocumentFragment Load(string fileName) {
            if (fileName == null) {
                throw new ArgumentNullException(nameof(fileName));
            }
            return Load(StreamContext.FromFile(fileName));
        }

        public new HtmlDocumentFragment Load(Uri source) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            return Load(StreamContext.FromSource(source));
        }

        public new HtmlDocumentFragment Load(Stream input) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }
            return Load(StreamContext.FromStream(input));
        }

        public new HtmlDocumentFragment Load(StreamContext input) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }
            return LoadHtml(input.ReadAllText(), new HtmlReaderSettings {
                BaseUri = input.Uri,
            });
        }

        public new HtmlDocumentFragment Load(XmlReader reader) {
            return (HtmlDocumentFragment) base.Load(reader);
        }

        protected override void LoadText(TextReader input) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }

            LoadHtml(input.ReadToEnd(), null);
        }

        private HtmlDocumentFragment LoadHtml(string html, HtmlReaderSettings settings) {
            settings = settings ?? HtmlReaderSettings.Default;

            var treeBuilder = settings.Mode.CreateTreeBuilder();
            BaseUri = settings.BaseUri;
            Append(treeBuilder.ParseFragment(
                html,
                settings.ContextElement,
                settings.BaseUri,
                HtmlParseErrorCollection.Tracking(settings.MaxErrors)
            ));
            return this;
        }
    }
}
