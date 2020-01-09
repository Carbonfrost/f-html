//
// - TagLibrary.cs -
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

using System;
using System.ComponentModel;
using System.Collections.Generic;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
// using ComponentCollection = Carbonfrost.Commons.Core.Runtime.Components.ComponentCollection;

namespace Carbonfrost.Commons.Html {

    // [RuntimeComponentUsage(Name = Html.ComponentTypes.TagLibrary)]
    [StreamingSource(typeof(TagLibrarySource))]
    public partial class TagLibrary : IUriContext {
        //IRuntimeComponent, IMakeReadOnly {

        private readonly TagCollection tags;
        // private readonly ComponentCollection dependencies = new ComponentCollection();
        // private ComponentName tagLibraryName;

        private static readonly TagLibrary html5 = CreateHtml5Library();

        public TagCollection Tags {
            get { return tags; }
        }

        protected Uri BaseUri { get; set; }

        // public ComponentName TagLibraryName {
        //     get { return tagLibraryName; }
        //     set {
        //         ThrowIfReadOnly();
        //         tagLibraryName = value;
        //     }
        // }

        public static TagLibrary Html5 {
            get { return html5; }
        }

        public TagLibrary() {
            this.tags = new TagCollection();
        }

        public Tag GetTag(string tagName) {
            if (tagName == null)
                throw new ArgumentNullException("tagName");

            tagName = tagName.Trim().ToLowerInvariant();
            if (tagName.Length == 0)
                throw Failure.AllWhitespace("tagName");

            // TODO This behavior causes any undefined tag to be
            // added to HTML 5 (need different behavior)

            var tags = this.Tags;
            lock (tags) {
                Tag tag = tags.GetValueOrDefault(tagName);
                if (tag == null) {
                    // not defined: create default; go anywhere, do anything! (incl be inside a <p>)
                    tag = new Tag(tagName);
                    tag._isBlock = false;
                    tag._canContainBlock = true;
                }
                return tag;
            }
        }

        public bool IsKnownTag(string tagName) {
            if (tagName == null)
                throw new ArgumentNullException("tagName");
            if (tagName.Length == 0)
                throw Failure.EmptyString("tagName");

            return tags.Contains(tagName);
        }

        public bool IsKnownTag(Tag tag) {
            if (tag == null)
                throw new ArgumentNullException("tag");

            return IsKnownTag(tag.Name);
        }

        public override string ToString() {
            // return Convert.ToString(tagLibraryName);
            return null;
        }

        // `IRuntimeComponent' implementation
        public Uri Source {
            get; protected set;
        }

        // string IRuntimeComponent.ComponentType {
        //     get {
        //         return Html.ComponentTypes.TagLibrary;
        //     }
        // }

        // ComponentName IRuntimeComponent.ComponentName {
        //     get {
        //         return tagLibraryName;
        //     }
        // }

        // IEnumerable<Component> IDependencyProvider.Dependencies {
        //     get {
        //         return dependencies;
        //     }
        // }

        // `IUriContext'
        Uri IUriContext.BaseUri {
            get {
                return BaseUri;
            }
            set {
                BaseUri = value;
            }
        }

        static TagLibrary CreateHtml5Library() {
            return new Html5TagLibrary();
        }

        // `IMakeReadOnly' implementation
        public bool IsReadOnly {
            get; private set;
        }

        internal void MakeReadOnly() {
            IsReadOnly = true;
            this.tags.MakeReadOnly();
            // this.dependencies.MakeReadOnly();
        }

        protected void ThrowIfReadOnly() {
            if (this.IsReadOnly)
                throw Failure.Sealed();
        }
    }

}
