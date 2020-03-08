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
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Html.Parser;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.Commons.Html {

    public class HtmlProcessingInstruction : DomProcessingInstruction<HtmlProcessingInstruction> {

        static readonly Regex split = new Regex(@"\s+");

        internal HtmlProcessingInstruction(string target, Uri baseUri) : base(target) {
            BaseUri = baseUri;
        }

        internal static HtmlProcessingInstruction Create(Token.Comment commentToken, Uri baseUri) {
            string fullContent = commentToken.Data.Substring(1, commentToken.Data.Length - 2);
            return FromFullContent(fullContent, baseUri);
        }

        internal static HtmlProcessingInstruction FromFullContent(string text, Uri baseUri) {
            string[] results = split.Split(text, 2);

            if (results.Length < 2) {
                Array.Resize(ref results, 2);
                results[1] = string.Empty;
            }

            results[0] = results[0].Trim();
            results[1] = results[1].Trim();
            return new HtmlProcessingInstruction(results[0], baseUri) {
                Data = results[1]
            };
        }
    }
}
