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

    class HtmlNodeTypeProvider : IDomNodeTypeProvider {

        public static readonly IDomNodeTypeProvider Instance = new HtmlNodeTypeProvider();

        private HtmlNodeTypeProvider() {}

        public DomName GetAttributeName(Type attributeType) {
            return null;
        }

        public Type GetAttributeNodeType(string name) {
            return typeof(HtmlAttribute);
        }

        public Type GetAttributeNodeType(DomName name) {
            return typeof(HtmlAttribute);
        }

        public DomName GetElementName(Type elementType) {
            return null;
        }

        public Type GetElementNodeType(string name) {
            return typeof(HtmlElement);
        }

        public Type GetElementNodeType(DomName name) {
            return typeof(HtmlElement);
        }

        public Type GetProcessingInstructionNodeType(string target) {
            return typeof(HtmlProcessingInstruction);
        }

        public string GetProcessingInstructionTarget(Type processingInstructionType) {
            return null;
        }
    }
}
