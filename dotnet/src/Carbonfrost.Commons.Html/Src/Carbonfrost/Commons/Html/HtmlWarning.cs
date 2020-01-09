//
// - HtmlWarning.cs -
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
using System.Diagnostics;

namespace Carbonfrost.Commons.Html {

	static class HtmlWarning {

	    public static void UnreadTokenPending() {
	        Debug.Fail("There is an unread token pending!");
	    }

        public static void ExpectedChildInParentCollection() {
            Debug.Fail("Expected this child in the parent's children collection");
        }

        public static void PoppingTDNotInCell() {
            Debug.Fail("pop td not in cell");
        }

        public static void PoppingHtml() {
            Debug.Fail("popping html!");
        }

        public static void UnexpectedTokenType() {
            Debug.Fail("Unexpected token type");
        }

        public static void ShouldNotBeReachable() {
            Debug.Fail("Should not be reachable");
        }

        public static void ReconstructUnexpectedlyEmpty() {
            Debug.Fail("entry is null.");
        }

        public static void FosterParentTableUnexpectedlyNull() {
            Debug.Fail("lastTable is null.");
        }

        public static void ElementShouldBeOnStack() {
            Debug.Fail("element should be on stack.");
        }
	}
}

