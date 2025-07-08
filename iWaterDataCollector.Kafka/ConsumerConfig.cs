// *** Auto-generated from librdkafka v1.9.2 *** - do not modify manually.
//
// Copyright 2018-2022 Confluent Inc.
//
// Licensed under the Apache License, Version 2.0 (the 'License');
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Refer to LICENSE for more information.

using iWaterDataCollector.Kafka.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iWaterDataCollector.Kafka
{
    /// <summary>
    ///     Consumer configuration properties
    /// </summary>
    public class ConsumerConfig : ClientConfig
    {

        /// <summary>
        ///     Initialize a new empty <see cref="ConsumerConfig" /> instance.
        /// </summary>
        public ConsumerConfig() : base() { }

        /// <summary>
        ///     Initialize a new <see cref="ConsumerConfig" /> instance wrapping
        ///     an existing <see cref="ClientConfig" /> instance.
        ///     This will change the values "in-place" i.e. operations on this class WILL modify the provided collection
        /// </summary>
        public ConsumerConfig(ClientConfig config) : base(config) { }

        /// <summary>
        ///     Initialize a new <see cref="ConsumerConfig" /> instance wrapping
        ///     an existing key/value pair collection.
        ///     This will change the values "in-place" i.e. operations on this class WILL modify the provided collection
        /// </summary>
        public ConsumerConfig(IDictionary<string, string> config) : base(config) { }

        /// <summary>
        ///     Check if any properties have been set that have implications for
        ///     application logic and therefore shouldn't be set via external
        ///     configuration, independent of the code. Throw an ArgumentException
        ///     if so.
        /// </summary>
        public ConsumerConfig ThrowIfContainsNonUserConfigurable()
        {
            var toCheck = new string[] { "enable.partition.eof", "partition.assignment.strategy", "enable.auto.commit", "enable.auto.offset.store" };
            this.Where(kv => toCheck.Contains(kv.Key)).ToList()
                .ForEach(kv => { throw new ArgumentException($"Consumer config property '{kv.Key}' is not user configurable."); });
            return this;
        }

        /// <summary>
        ///     A comma separated list of fields that may be optionally set
        ///     in <see cref="Confluent.Kafka.ConsumeResult{TKey,TValue}" />
        ///     objects returned by the
        ///     <see cref="Confluent.Kafka.Consumer{TKey,TValue}.Consume(System.TimeSpan)" />
        ///     method. Disabling fields that you do not require will improve
        ///     throughput and reduce memory consumption. Allowed values:
        ///     headers, timestamp, topic, all, none
        ///
        ///     default: all
        ///     importance: low
        /// </summary>
        public string ConsumeResultFields { set { this.SetObject("dotnet.consumer.consume.result.fields", value); } }

        /// <summary>
        ///     Action to take when there is no initial offset in offset store or the desired offset is out of range: 'smallest','earliest' - automatically reset the offset to the smallest offset, 'largest','latest' - automatically reset the offset to the largest offset, 'error' - trigger an error (ERR__AUTO_OFFSET_RESET) which is retrieved by consuming messages and checking 'message->err'.
        ///
        ///     default: largest
        ///     importance: high
        /// </summary>
        public AutoOffsetReset? AutoOffsetReset { get { return (AutoOffsetReset?)GetEnum(typeof(AutoOffsetReset), "auto.offset.reset"); } set { this.SetObject("auto.offset.reset", value); } }

        /// <summary>
        ///     Client group id string. All clients sharing the same group.id belong to the same group.
        ///
        ///     default: ''
        ///     importance: high
        /// </summary>
        public string GroupId { get { return Get("group.id"); } set { this.SetObject("group.id", value); } }

        /// <summary>
        ///     Enable static group membership. Static group members are able to leave and rejoin a group within the configured `session.timeout.ms` without prompting a group rebalance. This should be used in combination with a larger `session.timeout.ms` to avoid group rebalances caused by transient unavailability (e.g. process restarts). Requires broker version >= 2.3.0.
        ///
        ///     default: ''
        ///     importance: medium
        /// </summary>
        public string GroupInstanceId { get { return Get("group.instance.id"); } set { this.SetObject("group.instance.id", value); } }

        /// <summary>
        ///     The name of one or more partition assignment strategies. The elected group leader will use a strategy supported by all members of the group to assign partitions to group members. If there is more than one eligible strategy, preference is determined by the order of this list (strategies earlier in the list have higher priority). Cooperative and non-cooperative (eager) strategies must not be mixed. Available strategies: range, roundrobin, cooperative-sticky.
        ///
        ///     default: range,roundrobin
        ///     importance: medium
        /// </summary>
        public PartitionAssignmentStrategy? PartitionAssignmentStrategy { get { return (PartitionAssignmentStrategy?)GetEnum(typeof(PartitionAssignmentStrategy), "partition.assignment.strategy"); } set { this.SetObject("partition.assignment.strategy", value); } }

        /// <summary>
        ///     Client group session and failure detection timeout. The consumer sends periodic heartbeats (heartbeat.interval.ms) to indicate its liveness to the broker. If no hearts are received by the broker for a group member within the session timeout, the broker will remove the consumer from the group and trigger a rebalance. The allowed range is configured with the **broker** configuration properties `group.min.session.timeout.ms` and `group.max.session.timeout.ms`. Also see `max.poll.interval.ms`.
        ///
        ///     default: 45000
        ///     importance: high
        /// </summary>
        public int? SessionTimeoutMs { get { return GetInt("session.timeout.ms"); } set { this.SetObject("session.timeout.ms", value); } }

        /// <summary>
        ///     Group session keepalive heartbeat interval.
        ///
        ///     default: 3000
        ///     importance: low
        /// </summary>
        public int? HeartbeatIntervalMs { get { return GetInt("heartbeat.interval.ms"); } set { this.SetObject("heartbeat.interval.ms", value); } }

        /// <summary>
        ///     Group protocol type. NOTE: Currently, the only supported group protocol type is `consumer`.
        ///
        ///     default: consumer
        ///     importance: low
        /// </summary>
        public string GroupProtocolType { get { return Get("group.protocol.type"); } set { this.SetObject("group.protocol.type", value); } }

        /// <summary>
        ///     How often to query for the current client group coordinator. If the currently assigned coordinator is down the configured query interval will be divided by ten to more quickly recover in case of coordinator reassignment.
        ///
        ///     default: 600000
        ///     importance: low
        /// </summary>
        public int? CoordinatorQueryIntervalMs { get { return GetInt("coordinator.query.interval.ms"); } set { this.SetObject("coordinator.query.interval.ms", value); } }

        /// <summary>
        ///     Maximum allowed time between calls to consume messages (e.g., rd_kafka_consumer_poll()) for high-level consumers. If this interval is exceeded the consumer is considered failed and the group will rebalance in order to reassign the partitions to another consumer group member. Warning: Offset commits may be not possible at this point. Note: It is recommended to set `enable.auto.offset.store=false` for long-time processing applications and then explicitly store offsets (using offsets_store()) *after* message processing, to make sure offsets are not auto-committed prior to processing has finished. The interval is checked two times per second. See KIP-62 for more information.
        ///
        ///     default: 300000
        ///     importance: high
        /// </summary>
        public int? MaxPollIntervalMs { get { return GetInt("max.poll.interval.ms"); } set { this.SetObject("max.poll.interval.ms", value); } }

        /// <summary>
        ///     Automatically and periodically commit offsets in the background. Note: setting this to false does not prevent the consumer from fetching previously committed start offsets. To circumvent this behaviour set specific start offsets per partition in the call to assign().
        ///
        ///     default: true
        ///     importance: high
        /// </summary>
        public bool? EnableAutoCommit { get { return GetBool("enable.auto.commit"); } set { this.SetObject("enable.auto.commit", value); } }

        /// <summary>
        ///     The frequency in milliseconds that the consumer offsets are committed (written) to offset storage. (0 = disable). This setting is used by the high-level consumer.
        ///
        ///     default: 5000
        ///     importance: medium
        /// </summary>
        public int? AutoCommitIntervalMs { get { return GetInt("auto.commit.interval.ms"); } set { this.SetObject("auto.commit.interval.ms", value); } }

        /// <summary>
        ///     Automatically store offset of last message provided to application. The offset store is an in-memory store of the next offset to (auto-)commit for each partition.
        ///
        ///     default: true
        ///     importance: high
        /// </summary>
        public bool? EnableAutoOffsetStore { get { return GetBool("enable.auto.offset.store"); } set { this.SetObject("enable.auto.offset.store", value); } }

        /// <summary>
        ///     Minimum number of messages per topic+partition librdkafka tries to maintain in the local consumer queue.
        ///
        ///     default: 100000
        ///     importance: medium
        /// </summary>
        public int? QueuedMinMessages { get { return GetInt("queued.min.messages"); } set { this.SetObject("queued.min.messages", value); } }

        /// <summary>
        ///     Maximum number of kilobytes of queued pre-fetched messages in the local consumer queue. If using the high-level consumer this setting applies to the single consumer queue, regardless of the number of partitions. When using the legacy simple consumer or when separate partition queues are used this setting applies per partition. This value may be overshot by fetch.message.max.bytes. This property has higher priority than queued.min.messages.
        ///
        ///     default: 65536
        ///     importance: medium
        /// </summary>
        public int? QueuedMaxMessagesKbytes { get { return GetInt("queued.max.messages.kbytes"); } set { this.SetObject("queued.max.messages.kbytes", value); } }

        /// <summary>
        ///     Maximum time the broker may wait to fill the Fetch response with fetch.min.bytes of messages.
        ///
        ///     default: 500
        ///     importance: low
        /// </summary>
        public int? FetchWaitMaxMs { get { return GetInt("fetch.wait.max.ms"); } set { this.SetObject("fetch.wait.max.ms", value); } }

        /// <summary>
        ///     Initial maximum number of bytes per topic+partition to request when fetching messages from the broker. If the client encounters a message larger than this value it will gradually try to increase it until the entire message can be fetched.
        ///
        ///     default: 1048576
        ///     importance: medium
        /// </summary>
        public int? MaxPartitionFetchBytes { get { return GetInt("max.partition.fetch.bytes"); } set { this.SetObject("max.partition.fetch.bytes", value); } }

        /// <summary>
        ///     Maximum amount of data the broker shall return for a Fetch request. Messages are fetched in batches by the consumer and if the first message batch in the first non-empty partition of the Fetch request is larger than this value, then the message batch will still be returned to ensure the consumer can make progress. The maximum message batch size accepted by the broker is defined via `message.max.bytes` (broker config) or `max.message.bytes` (broker topic config). `fetch.max.bytes` is automatically adjusted upwards to be at least `message.max.bytes` (consumer config).
        ///
        ///     default: 52428800
        ///     importance: medium
        /// </summary>
        public int? FetchMaxBytes { get { return GetInt("fetch.max.bytes"); } set { this.SetObject("fetch.max.bytes", value); } }

        /// <summary>
        ///     Minimum number of bytes the broker responds with. If fetch.wait.max.ms expires the accumulated data will be sent to the client regardless of this setting.
        ///
        ///     default: 1
        ///     importance: low
        /// </summary>
        public int? FetchMinBytes { get { return GetInt("fetch.min.bytes"); } set { this.SetObject("fetch.min.bytes", value); } }

        /// <summary>
        ///     How long to postpone the next fetch request for a topic+partition in case of a fetch error.
        ///
        ///     default: 500
        ///     importance: medium
        /// </summary>
        public int? FetchErrorBackoffMs { get { return GetInt("fetch.error.backoff.ms"); } set { this.SetObject("fetch.error.backoff.ms", value); } }

        /// <summary>
        ///     Controls how to read messages written transactionally: `read_committed` - only return transactional messages which have been committed. `read_uncommitted` - return all messages, even transactional messages which have been aborted.
        ///
        ///     default: read_committed
        ///     importance: high
        /// </summary>
        public IsolationLevel? IsolationLevel { get { return (IsolationLevel?)GetEnum(typeof(IsolationLevel), "isolation.level"); } set { this.SetObject("isolation.level", value); } }

        /// <summary>
        ///     Emit RD_KAFKA_RESP_ERR__PARTITION_EOF event whenever the consumer reaches the end of a partition.
        ///
        ///     default: false
        ///     importance: low
        /// </summary>
        public bool? EnablePartitionEof { get { return GetBool("enable.partition.eof"); } set { this.SetObject("enable.partition.eof", value); } }

        /// <summary>
        ///     Verify CRC32 of consumed messages, ensuring no on-the-wire or on-disk corruption to the messages occurred. This check comes at slightly increased CPU usage.
        ///
        ///     default: false
        ///     importance: medium
        /// </summary>
        public bool? CheckCrcs { get { return GetBool("check.crcs"); } set { this.SetObject("check.crcs", value); } }

        /// <summary>
        ///     Allow automatic topic creation on the broker when subscribing to or assigning non-existent topics. The broker must also be configured with `auto.create.topics.enable=true` for this configuraiton to take effect. Note: The default value (false) is different from the Java consumer (true). Requires broker version >= 0.11.0.0, for older broker versions only the broker configuration applies.
        ///
        ///     default: false
        ///     importance: low
        /// </summary>
        public bool? AllowAutoCreateTopics { get { return GetBool("allow.auto.create.topics"); } set { this.SetObject("allow.auto.create.topics", value); } }

    }
}
