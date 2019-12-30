//
// - TagLibrary.Static.cs -
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
using System.IO;
using System.Text;

using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Html {

    partial class TagLibrary {

        public static TagLibrary FromFile(string fileName) {
            return (TagLibrary) new TagLibrarySource().Load(StreamContext.FromFile(fileName), typeof(TagLibrary));
        }

        public static TagLibrary FromStream(Stream stream,
                                            Encoding encoding = null)
        {
            return (TagLibrary) new TagLibrarySource().Load(StreamContext.FromStream(stream), typeof(TagLibrary));
        }

        public static TagLibrary FromStreamContext(StreamContext streamContext) {
            if (streamContext == null)
                throw new ArgumentNullException("streamContext");

            return (TagLibrary) new TagLibrarySource().Load(streamContext, typeof(TagLibrary));
        }
    }
}

