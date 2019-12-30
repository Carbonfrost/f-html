//
// - HtmlDocumentSource.cs -
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

    public class HtmlDocumentSource : StreamingSource {

        // TODO Implement `HtmlDocumentSource'

        public override void Save(StreamContext outputTarget, object value) {
            if (outputTarget == null)
                throw new ArgumentNullException("outputTarget");
            if (value == null)
                throw new ArgumentNullException("value");

            HtmlDocument document = value as HtmlDocument;
            if (document == null)
                throw Failure.NotInstanceOf("value", value, typeof(HtmlDocument));

            throw new NotImplementedException();
        }

        public override object Load(StreamContext inputSource, Type instanceType) {
            if (inputSource == null)
                throw new ArgumentNullException("inputSource");

            if (typeof(HtmlDocument).Equals(instanceType))
                return HtmlDocument.FromStreamContext(inputSource);
            else
                throw Failure.NotInstanceOf("instanceType", instanceType, typeof(HtmlDocument));
        }
    }
}

