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

using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    class QuirksModeAnnotation {

        public readonly QuirksMode Value;

        public QuirksModeAnnotation(QuirksMode value) {
            Value = value;
        }
    }

    static partial class Extensions {

        public static QuirksMode GetQuirksMode(this DomDocument self) {
            var anno = self.Annotation<QuirksModeAnnotation>();
            if (anno != null) {
                return anno.Value;
            }
            return QuirksMode.None;
        }

        public static void SetQuirksMode(this DomDocument self, QuirksMode value) {
            self.RemoveAnnotations<QuirksModeAnnotation>();
            if (value != QuirksMode.None) {
                self.AddAnnotation(new QuirksModeAnnotation(value));
            }
        }
    }
}
