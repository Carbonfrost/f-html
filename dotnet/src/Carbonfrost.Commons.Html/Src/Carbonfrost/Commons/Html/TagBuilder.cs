//
// - TagBuilder.cs -
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

    public class TagBuilder {

	    public bool IsBlock { get; set; }
	    public bool FormatAsBlock { get; set; }
	    public string Name { get; set; }
	    public bool CanContainBlocks { get; set; }
	    public bool IsEmpty { get; set; }
	    public bool IsSelfClosing { get; set; }
	    public bool PreserveWhitespace { get; set; }

	    public Tag Build() {
	        return new Tag(this.Name) {
	            _isBlock = IsBlock,
	            _formatAsBlock = FormatAsBlock,
	            _canContainBlock = CanContainBlocks,
	            empty = IsEmpty,
	            selfClosing = IsSelfClosing,
	            _preserveWhitespace = PreserveWhitespace,
	        };
	    }
	}
}

