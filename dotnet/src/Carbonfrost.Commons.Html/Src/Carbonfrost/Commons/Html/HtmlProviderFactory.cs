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
using System.Reflection;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    public class HtmlProviderFactory : DomProviderFactory {

        private static readonly Assembly THIS_ASSEMBLY = typeof(HtmlProviderFactory).Assembly;

        public static readonly HtmlProviderFactory Instance = new HtmlProviderFactory();

        public override bool IsProviderObject(Type providerObjectType) {
            if (providerObjectType == null) {
                throw new ArgumentNullException(nameof(providerObjectType));
            }
            return providerObjectType.Assembly == THIS_ASSEMBLY;
        }

        protected override DomDocument CreateDomDocument() {
            return new HtmlDocument().WithSchema(new HtmlSchema());
        }

        protected override IDomNodeFactory CreateDomNodeFactory(IDomNodeTypeProvider nodeTypeProvider) {
            return new HtmlNodeFactory(
                DomNodeTypeProvider.Compose(nodeTypeProvider, new HtmlNodeTypeProvider())
            );
        }

        private class HtmlNodeFactory : DomNodeFactory {

            public HtmlNodeFactory(IDomNodeTypeProvider nodeTypeProvider) : base(nodeTypeProvider) {
            }

            public override DomText CreateText() {
                return new HtmlText();
            }
        }
    }

}
