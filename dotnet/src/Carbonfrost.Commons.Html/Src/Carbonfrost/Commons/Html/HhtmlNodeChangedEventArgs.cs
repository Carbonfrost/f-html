//
// - HhtmlNodeChangedEventArgs.cs -
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Html {

    public class HhtmlNodeChangedEventArgs : EventArgs {

        public HtmlNodeChangedAction Action { get; private set; }
        public HtmlNode NewParent { get; private set; }
        public string NewValue { get; private set; }
        public HtmlNode Node { get; private set; }
        public HtmlNode OldParent { get; private set; }
        public string OldValue { get; private set; }

        public HhtmlNodeChangedEventArgs(
            HtmlNodeChangedAction action,
            HtmlNode newParent,
            string newValue,
            HtmlNode node,
            HtmlNode oldParent,
            string oldValue)
        {
            this.Action = action;
            this.NewParent = newParent;
            this.NewValue = newValue;
            this.Node = node;
            this.OldParent = oldParent;
            this.OldValue = oldValue;
        }
    }

}

