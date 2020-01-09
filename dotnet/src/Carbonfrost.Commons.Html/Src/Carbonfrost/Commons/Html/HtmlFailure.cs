//
// - HtmlFailure.cs -
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

namespace Carbonfrost.Commons.Html {

    static class HtmlFailure {

        public static Exception ParentNodeRequired() {
            return Failure.Prepare(new InvalidOperationException("Node must have a parent for the operation."));
        }

        public static Exception CannotContainWhitespace(string argName) {
            return Failure.Prepare(new ArgumentException("Argument cannot contain whitespace", argName));
        }

        public static Exception QueueDidNotMatch() {
            return Failure.Prepare(new Exception("Queue did not match expected sequence"));
        }

        public static Exception CouldNotParseQuery(string query, string remainder) {
            return Failure.Prepare(new Exception(string.Format("Could not parse query '{0}': unexpected token at '{1}'", query, remainder)));
        }

        public static ArgumentException NotSelectorSelectionCannotBeEmpty() {
            return Failure.Prepare(new ArgumentException(":not(selector) subselect must not be empty"));
        }

        public static Exception MatchesSelectorNotEmpty() {
            return Failure.Prepare(new Exception(":matches(regex) query must not be empty"));
        }

        public static Exception ContainsSelectorNotEmpty() {
            return Failure.Prepare(new Exception(":contains(text) query must not be empty"));
        }

        public static Exception HasSelectorNotEmpty() {
            return Failure.Prepare(new ArgumentException(":has(el) subselect must not be empty"));
        }

        public static Exception CannotParseAttributeQuery(string query, string remainder) {
            return Failure.Prepare(new Exception(string.Format("Could not parse attribute query '{0}': unexpected token at '{0}'", query, remainder)));
        }

        public static Exception UnknownCombinator(char combinator) {
            return Failure.Prepare(new Exception("Unknown combinator: " + combinator));
        }

        public static Exception QueueDidNotLongEnough()
        {
            return Failure.Prepare(new Exception("Queue not long enough to consume sequence"));
        }
    }
}
