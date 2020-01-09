//
// - TagLibrarySource.cs -
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
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Html {

    [StreamingSourceUsage(ContentTypes = Html.ContentTypes.TagLibrary,
                          Extensions = ".tagLibrary.xml")]
    public sealed partial class TagLibrarySource : StreamingSource {

        // The build tool should generate code that sets this
        static readonly Func<StreamingSource> Impl = null;

        public override void Save(StreamContext outputTarget, object value) {
            if (outputTarget == null)
                throw new ArgumentNullException("outputTarget");
            if (value == null)
                throw new ArgumentNullException("value");

            TagLibrary lib = value as TagLibrary;
            if (lib == null)
                throw Failure.NotInstanceOf("value", value, typeof(TagLibrary));

            GetRealStreamingSource().Save(outputTarget, lib);
        }

        public override object Load(StreamContext inputSource, Type instanceType) {
            if (inputSource == null)
                throw new ArgumentNullException("inputSource");

            return GetRealStreamingSource().Load(inputSource, instanceType);
        }

        private StreamingSource GetRealStreamingSource() {
            // Fallback, requires (F5) Property Trees to be on the class path
            StreamingSource answer;
            if (Impl == null)
                answer = StreamingSource.Create(
                    typeof(TagLibrarySource),
                    ContentType.Parse(Html.ContentTypes.TagLibrary).Parent,
                    null,
                    ServiceProvider.Root);
            else
                answer = Impl();

            if (answer == null || object.ReferenceEquals(this, answer)) {
                // TODO Throw structured failure
            }

            return answer;
        }
    }
}

