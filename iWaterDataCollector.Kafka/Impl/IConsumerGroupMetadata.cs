﻿// Copyright 2020 Confluent Inc.
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

namespace iWaterDataCollector.Kafka.Impl
{
    /// <summary>
    ///     The consumer group metadata associated with a consumer.
    /// </summary>
    public interface IConsumerGroupMetadata { }

    internal class ConsumerGroupMetadata : IConsumerGroupMetadata
    {
        internal byte[] serializedMetadata;
    }
}
