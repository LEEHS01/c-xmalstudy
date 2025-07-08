using iWaterDataCollector.Global.Handler;
using iWaterDataCollector.Global;
using iWaterDataCollector.Model.Tag;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using iWaterDataCollector.iFixAdapter.Common;
using Proficy.Historian.UserAPI;
using Confluent.Kafka;
using System.Configuration;
using System.Linq;
using Proficy.iFixToolkit.Adapter2;

/********************************************
 * iWater Local SCADA 연결 Class
 ********************************************/
namespace iWaterDataCollector.iFixAdapter
{
    public class Kafka2PDB
    {
        private const int EdaKey = 1;
        /// <summary>
        /// Task Token
        /// </summary>
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private List<KafkaTagModel> lKafkaTag = new List<KafkaTagModel>();

        public bool IsActive { get; set; }
        public bool UseFilter { get; set; }
        public string Node { get; set; }
        public string EndPoint { get; set; }
        public string Group_id { get; set; }
        public string KafkaPath { get; set; }
        public int SetInterval { get; set; }

        private List<string> _topics = new List<string>();

        private string _myName;

        public Kafka2PDB(string[] topics)
        {
            _myName = GetType().Name;
            foreach (var topic in topics)
            {
                _topics.Add(topic);
            }
        }

        public void Launch()
        {
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "Kafka Consume 시작");
            SetOnMessageTask(KeepOn, _tokenSource.Token);
        }

        public void Disconnect()
        {
            //_consume.Disconnect();
            _tokenSource.Cancel();
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "Kafka Message 처리 Task 중지");
        }

        private void LoadKafakTags()
        {
            lKafkaTag = FileHandler.ReadKafkaCollection(KafkaPath);
            AppData.Instance.MsgIRDC.Debug(AppData.AppLog, _myName, "Kafka Message 처리 Tag 정보 Load");
        }

        private void KeepOn(object obj)
        {
            var tag = lKafkaTag[(int)obj];

            Thread.Sleep(TimeSpan.FromSeconds(tag.pulse.DuringTime));
            SetPDBValue(tag.Name, tag.pulse.OffValue);
            AppData.Instance.MsgIRDC.Info(AppData.PDBLog, _myName, $"{tag.Name} DuringTime:{tag.pulse.DuringTime}초 후 OffValue[{tag.pulse.OffValue}] 입력");
        }

        private void SetPDBValue(string tag, string val)
        {
            var fVal = 0F;
            var type = AppData.F_CV;
            int code;
            try
            {
                if (bool.TryParse(val, out bool bVal))
                {
                    fVal = bVal ? 1.0F : 0.0F;
                    code = Eda.SetOneFloat(Node, tag, type, fVal, EdaKey);
                }
                else if (float.TryParse(val, out fVal))
                {
                    code = Eda.SetOneFloat(Node, tag, type, fVal, EdaKey);
                }
                else
                {
                    type = AppData.A_CV;
                    code = Eda.SetOneAscii(Node, tag, type, val, EdaKey);
                }

                if (code != 0)
                {
                    //에러 로그파일 생성
                    AppData.Instance.MsgIRDC.Info(AppData.PDBErrLog, _myName, $"Tag {tag} - Value [{val}] Error(CODE:{code})");
                    
                }
                else
                {
                    //제어 명령을 내릴 때 로그파일 생성
                    AppData.Instance.MsgIRDC.Info(AppData.PDBLog, _myName, $"Tag {tag} - Value [{val}] Set");
                    //엔진 로그 임시 주석(20250609)
                    //logging(Node, "iRDC", tag, val);
                }
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Warn(AppData.PDBLog, _myName, "Eda.SetOne Value Error");
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "Eda.SetOne Value Error", ex);
            }
        }

        /// <summary>
        /// 추가 함수 ----> Log 수정부 (2025)
        /// </summary>
        /// <param name="node"></param>
        /// <param name="applicationName"></param>
        /// <param name="tagname"></param>
        /// <param name="val"></param>
        private void logging(string node, string applicationName, string tagname, string val)
        {
            string pcname = Environment.MachineName;
            string txt = $"[{node}]{tagname} set to {val} by {pcname} [APP] {applicationName}.exe";
            Eda.SendMsg(Globals.TyperAll, Globals.AlarmDestinationA, 0, txt);
        }

        private Task SetOnMessageTask(Action<object> action, CancellationToken token)
        {
            AppData.Instance.MsgIRDC.Debug(AppData.AppLog, _myName, "Kafka Message 처리 Task 시작");
            Action<object> aWrapper = (index) =>
            {
                if (token.IsCancellationRequested)
                {
                    AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "Kafka Message 처리 Task 중지 신호 확인");
                    return;
                }

                action(index);
            };

            Action aWork = () =>
            {
                try
                {
                    var config = new ConsumerConfig
                    {
                        BootstrapServers = EndPoint,     // Kafka 브로커 주소
                        GroupId = Group_id,         // Consumer 그룹 ID
                        CheckCrcs = true,
                        EnableAutoOffsetStore = false,
                        EnableAutoCommit = true,
                        StatisticsIntervalMs = 5000,
                        ConnectionsMaxIdleMs = 60000,
                        SessionTimeoutMs = 6000,
                        AutoOffsetReset = AutoOffsetReset.Latest, // 가장 처음부터 읽기
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
                        (_, e) => AppData.Instance.MsgIRDC.Warn(AppData.KafukaLog, _myName, $"[{e}]")
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
                            AppData.Instance.MsgIRDC.Debug(AppData.KafukaLog, _myName, $"Partitions incrementally assigned: [{string.Join(",", partitions.Select(p => p.Partition.Value))}]" +
                                $", all: [{string.Join(",", c.Assignment.Concat(partitions).Select(p => p.Partition.Value))}]");
                        }
                    )
                    .SetPartitionsRevokedHandler((c, partitions) =>
                    {
                        #region Note
                        // Since a cooperative assignor (CooperativeSticky) has been configured, the revoked
                        // assignment is incremental (may remove only some partitions of the current assignment).
                        #endregion
                        var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
                        AppData.Instance.MsgIRDC.Debug(AppData.KafukaLog, _myName, $"Partitions incrementally revoked: [{string.Join(",", partitions.Select(p => p.Partition.Value))}], " +
                            $"remaining: [{string.Join(",", remaining.Select(p => p.Partition.Value))}]");
                    })
                    .SetPartitionsLostHandler((c, partitions) =>
                    {
                        #region Note
                        // The lost partitions handler is called when the consumer detects that it has lost ownership
                        // of its assignment (fallen out of the group).
                        #endregion
                        AppData.Instance.MsgIRDC.Debug(AppData.KafukaLog, _myName, $"Consumer 통신오류확인 [{Group_id}]Partitions were lost : [{string.Join(", ", partitions)}]");
                    })
                    .Build())
                    {
                        consumer.Subscribe(_topics);
                        AppData.Instance.MsgIRDC.Info(AppData.KafukaLog, _myName, "Kafuka Consumer 시작");
                        try
                        {
                            if (token.IsCancellationRequested)
                            {
                                AppData.Instance.MsgIRDC.Info(AppData.KafukaLog, _myName, "Consumer 중지 신호 확인");
                                consumer.Close();
                                return;
                            }
                            var msg = string.Empty;
                            var model = new MessageModel();
                            while (true)
                            {
                                try
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        AppData.Instance.MsgIRDC.Info(AppData.KafukaLog, _myName, "Consumer 중지 신호 확인");
                                        break;
                                    }

                                    var consumeResult = consumer.Consume(token);

                                    if (consumeResult.IsPartitionEOF)
                                    {
                                        continue;
                                    }
                                    msg = consumeResult.Message.Value;
                                    AppData.Instance.MsgIRDC.Info(AppData.KafukaLog, _myName, $"[{Group_id}][{consumeResult.Topic}] {consumeResult.Message.Value}");
                                    try
                                    {
                                        #region Note
                                        // Store the offset associated with consumeResult to a local cache. Stored offsets are committed to Kafka by a background thread every AutoCommitIntervalMs. 
                                        // The offset stored is actually the offset of the consumeResult + 1 since by convention, committed offsets specify the next message to consume. 
                                        // If EnableAutoOffsetStore had been set to the default value true, the .NET client would automatically store offsets immediately prior to delivering messages to the application. 
                                        // Explicitly storing offsets after processing gives at-least once semantics, the default behavior does not.
                                        #endregion
                                        //consumer.StoreOffset(consumeResult);
                                        model = JsonConvert.DeserializeObject<MessageModel>(msg);
                                        var idx = lKafkaTag.FindIndex(t => t.Name.Equals(model.tag));
                                        AppData.Instance.MsgIRDC.Debug(AppData.KafukaLog, _myName, $"설정된 Kafuka Tag[{model.tag}]의 Index [{idx}]");
                                        //Kafka Tag List에 없는 경우
                                        if (idx < 0)
                                        {
                                            AppData.Instance.MsgIRDC.Debug(AppData.KafukaLog, _myName, "Non Kafka List - Tag {model.tag}");
                                            if (UseFilter)
                                            {
                                                AppData.Instance.MsgIRDC.Info(AppData.KafukaLog, _myName, "Filter 처리");
                                                continue;
                                            }
                                            else
                                            {
                                                //Pulse Tag에 포함되지 않은 경우 단순 Set 진행
                                                SetPDBValue(model.tag, model.value);
                                            }
                                        }
                                        //Kafka Tag List에 있는 경우
                                        else
                                        {
                                            AppData.Instance.MsgIRDC.Info(AppData.KafukaLog, _myName, $"Kafka List - Tag {model.tag}");
                                            if (lKafkaTag[idx].IsPulse == false)
                                            {
                                                //Pulse Tag에 포함되지 않은 경우 단순 Set 진행
                                                SetPDBValue(model.tag, model.value);
                                            }
                                            else
                                            {
                                                //Pulse Tag에 포함된 경우만 Off 신호 Set
                                                AppData.Instance.MsgIRDC.Debug(AppData.KafukaLog, _myName, $"이중화 상태 Active {IsActive}");
                                                //이중화 기능 추가
                                                if (IsActive)
                                                {
                                                    if (model.value.Equals(lKafkaTag[idx].pulse.OnValue))
                                                    {
                                                        SetPDBValue(model.tag, model.value);
                                                        _ = Task.Factory.StartNew(aWrapper, idx, token);
                                                    }
                                                    else
                                                    {
                                                        AppData.Instance.MsgIRDC.Warn(AppData.KafukaLog, _myName, $"Pulse Tag {model.tag}에 입력된 값({model.value})이 On Value({lKafkaTag[idx].pulse.OnValue})가 아닙니다. - PDB에 입력되지 않습니다.");
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    catch (KafkaException ex)
                                    {
                                        AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "KafkaException 발생", ex);
                                    }
                                    catch (Exception ex)
                                    {
                                        AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "Exception 발생", ex);
                                    }
                                }
                                catch (ConsumeException ex)
                                {
                                    AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "ConsumeException 발생", ex);
                                }
                                catch (Exception ex)
                                {
                                    AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "Exception 발생", ex);
                                }

                                Thread.Sleep(SetInterval);
                            }
                        }
                        catch (OperationCanceledException ocex)
                        {
                            consumer.Close();
                            AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "OperationCanceledException 발생", ocex);
                        }
                        catch (Exception ex)
                        {
                            consumer.Close();
                            AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "Exception 발생", ex);
                        }
                        finally
                        {
                            consumer.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "Kafka Message Dequeu Error", ex);
                }
            };
            return Task.Factory.StartNew(aWork, token);
        }
    }
}
