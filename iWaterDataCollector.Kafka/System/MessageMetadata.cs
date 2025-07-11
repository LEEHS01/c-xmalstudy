﻿// Copyright 2016-2018 Confluent Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Refer to LICENSE for more information.

using iWaterDataCollector.Kafka.Common;
using iWaterDataCollector.Kafka.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Kafka.System
{
    /// <summary>
    ///     All components of <see cref="Message{TKey,TValue}" /> except Key and Value.
    /// </summary>
    public class MessageMetadata
    {
        /// <summary>
        ///     The message timestamp. The timestamp type must be set to CreateTime. 
        ///     Specify Timestamp.Default to set the message timestamp to the time
        ///     of this function call.
        /// </summary>
        public Timestamp Timestamp { get; set; }

        /// <summary>
        ///     The collection of message headers (or null). Specifying null or an 
        ///      empty list are equivalent. The order of headers is maintained, and
        ///     duplicate header keys are allowed.
        /// </summary>
        public Headers Headers { get; set; }
    }
}
