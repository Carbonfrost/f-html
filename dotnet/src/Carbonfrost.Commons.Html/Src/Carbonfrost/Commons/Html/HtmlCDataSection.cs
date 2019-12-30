//
// - HtmlCDataSection.cs -
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

namespace Carbonfrost.Commons.Html {

    public class HtmlCDataSection : HtmlCharacterData {

        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.CDataSection;
            }
        }

        public override string NodeName {
            get {
                return NodeNames.CDataSection;
            }
        }

        public override TResult AcceptVisitor<TArgument, TResult>(HtmlNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitCDataSection(this, argument);
        }

        public override void AcceptVisitor(HtmlNodeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            visitor.VisitCDataSection(this);
        }
    }
}
