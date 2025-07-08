using iWaterDataCollector.Global;
using iWaterDataCollector.Kafka.Exceptions;
using iWaterDataCollector.Kafka.System;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace iWaterDataCollector.Kafka
{
    public class Consume : IDisposable
    {
        /// <summary>
        /// Log 설정
        /// </summary>
        private static ILog _log;
        private static ILog _debugLog;
        private static ILog _errorLog;
        private Message _message = new Message();
        /// <summary>
        /// kafka Message Queue
        /// </summary>
        public ConcurrentQueue<string> Message = new ConcurrentQueue<string>();
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private string _endPoint;
        private List<string> _topics = new List<string>();

        public Consume(string endPoint, List<string> topics)
        {
            _log = Log.Instance.GetUserLogger(LOG_SECTION.Kafka.ToString(), GetType().Name);
            _debugLog = Log.Instance.GetUserLogger(LOG_SECTION.Debug.ToString(), GetType().Name);
            _errorLog = Log.Instance.GetUserLogger(LOG_SECTION.Error.ToString(), GetType().Name);

            _endPoint = endPoint;
            _topics = topics;

#if DETAIL_LOG
            _message.SetDebug(_debugLog, $"Kafka server 정보 : {endPoint}");
            _message.SetDebug(_debugLog, $"등록 Topic : [{string.Join(",", _topics)}]");
#endif
        }

        public void Launch(string group_id)
        {
            _message.Set(_log, $"Consume[{group_id}] 시작");
            ConsumeTask(group_id, _tokenSource.Token);
        }

        public void Disconnect()
        {
            _message.Set(_log, "Consume 중지 신호 전달");
            _tokenSource.Cancel();
        }

        private Task ConsumeTask(string group_id, CancellationToken token)
        {
            Action aWork = () =>
            {
                var config = new ConsumerConfig
                {
                    #region 참고
                    //BootstrapServers = _endPoint,
                    //GroupId = group_id,
                    ////EnableAutoOffsetStore = false,
                    //EnableAutoCommit = false,
                    //StatisticsIntervalMs = 1000,
                    //MaxPollIntervalMs = 300000,
                    //HeartbeatIntervalMs = 3000,
                    ////AutoCommitIntervalMs = 1000,
                    //SessionTimeoutMs = 10000,
                    //CheckCrcs = true,
                    ////AutoOffsetReset = AutoOffsetReset.Earliest,
                    //AutoOffsetReset = AutoOffsetReset.Latest,
                    //EnablePartitionEof = true,
                    #endregion

                    BootstrapServers = _endPoint,
                    GroupId = group_id,
                    CheckCrcs = true,
                    EnableAutoOffsetStore = false,
                    EnableAutoCommit = true,
                    StatisticsIntervalMs = 5000,
                    ConnectionsMaxIdleMs = 60000,
                    SessionTimeoutMs = 6000,
                    AutoOffsetReset = AutoOffsetReset.Latest,
                    EnablePartitionEof = true,

                    // A good introduction to the CooperativeSticky assignor and incremental rebalancing:
                    // https://www.confluent.io/blog/cooperative-rebalancing-in-kafka-streams-consumer-ksqldb/
                    PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
                };

                #region using Note
                // Note: If a key or value deserializer is not set (as is the case below), the 
                // deserializer corresponding to the appropriate type from Confluent.Kafka.Deserializers
                // will be used automatically (where available). The default deserializer for string
                // is UTF8. The default deserializer for Ignore returns null for all input data
                // (including non-null data).
                #endregion
                using (var consumer = new ConsumerBuilder<string, string>(config)
                #region Note
                // Note: All handlers are called on the main .Consume thread.
                //.SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                //.SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
                #endregion
                .SetErrorHandler
                (
                    (_, e) => _message.SetWarn(_log, $"[{e.ToString()}]")
                )
                .SetStatisticsHandler((_, json) => Console.WriteLine(""))
                .SetPartitionsAssignedHandler
                (
                    (c, partitions) =>
                    {
                        #region Note
                        // Since a cooperative assignor (CooperativeSticky) has been configured, the
                        // partition assignment is incremental (adds partitions to any existing assignment).
                        // Possibly manually specify start offsets by returning a list of topic/partition/offsets
                        // to assign to, e.g.:
                        // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                        #endregion
#if DETAIL_LOG
                        _message.SetDebug(_debugLog, $"Partitions incrementally assigned: [{string.Join(",", partitions.Select(p => p.Partition.Value))}]" +
                            $", all: [{string.Join(",", c.Assignment.Concat(partitions).Select(p => p.Partition.Value))}]");
#endif
                    }
                )
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    #region Note
                    // Since a cooperative assignor (CooperativeSticky) has been configured, the revoked
                    // assignment is incremental (may remove only some partitions of the current assignment).
                    #endregion
                    var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
#if DETAIL_LOG
                    _message.SetDebug(_debugLog, $"Partitions incrementally revoked: [{string.Join(",", partitions.Select(p => p.Partition.Value))}], " +
                        $"remaining: [{string.Join(",", remaining.Select(p => p.Partition.Value))}]");
#endif
                })
                .SetPartitionsLostHandler((c, partitions) =>
                {
                    #region Note
                    // The lost partitions handler is called when the consumer detects that it has lost ownership
                    // of its assignment (fallen out of the group).
                    #endregion
#if DETAIL_LOG
                    _message.Set(_log, $"Consumer 통신오류확인 [{group_id}]Partitions were lost : [{string.Join(", ", partitions)}]");
#endif
                })
                .Build())
                {
                    consumer.Subscribe(_topics);

                    try
                    {
                        if (token.IsCancellationRequested)
                        {
                            _message.Set(_log, "Consumer 중지 신호 확인");
                            return;
                        }

                        while (true)
                        {
                            try
                            {
                                if (token.IsCancellationRequested)
                                {
                                    _message.Set(_log, "Consumer 중지 신호 확인");
                                    break;
                                }

                                var consumeResult = consumer.Consume(token);

                                if (consumeResult.IsPartitionEOF)
                                {
                                    continue;
                                }
                                Message.Enqueue(consumeResult.Message.Value);
                                _message.Set(_log, $"[{group_id}][{consumeResult.Topic}] {consumeResult.Message.Value}");
                                try
                                {
                                    #region Note
                                    // Store the offset associated with consumeResult to a local cache. Stored offsets are committed to Kafka by a background thread every AutoCommitIntervalMs. 
                                    // The offset stored is actually the offset of the consumeResult + 1 since by convention, committed offsets specify the next message to consume. 
                                    // If EnableAutoOffsetStore had been set to the default value true, the .NET client would automatically store offsets immediately prior to delivering messages to the application. 
                                    // Explicitly storing offsets after processing gives at-least once semantics, the default behavior does not.
                                    #endregion
                                    //consumer.StoreOffset(consumeResult);
                                }
                                catch (KafkaException ex)
                                {
                                    _message.SetError(_errorLog, "KafkaException 발생", ex);
                                }
                                catch (Exception ex)
                                {
                                    _message.SetError(_errorLog, "Exception 발생", ex);
                                }
                            }
                            catch (ConsumeException ex)
                            {
                                _message.SetError(_errorLog, "ConsumeException 발생", ex);
                            }
                            catch (Exception ex)
                            {
                                _message.SetError(_errorLog, "Exception 발생", ex);
                            }
                        }
                    }
                    catch (OperationCanceledException ocex)
                    {
                        consumer.Close();
                        _message.SetError(_errorLog, "OperationCanceledException 발생", ocex);
                    }
                    catch(Exception ex)
                    {
                        consumer.Close();
                        _message.SetError(_errorLog, "Exception 발생", ex);
                    }
                    finally
                    {
                        consumer.Close();
                    }
                }
            };

            return Task.Factory.StartNew(aWork, token);
        }

        public void Dispose()
        {

        }
    }
}
