using iWaterDataCollector.Global;
using iWaterDataCollector.Global.Handler;
using iWaterDataCollector.iFixAdapter.Common;
using iWaterDataCollector.Model.Tag;
using log4net;
using Proficy.Historian.UserAPI;
using Proficy.iFixToolkit.Adapter2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/********************************************
 * iWater Remote SCADA 연결 Class
 ********************************************/
namespace iWaterDataCollector.iFixAdapter
{
    public class RemoteServer : Server
    {
        #region Log Class 선언
        /// <summary>
        /// Log Interface : Program
        /// </summary>
        private static ILog _log;
        #endregion
        /// <summary>
        /// 동작 시간
        /// </summary>
        public Stopwatch sw = new Stopwatch();
        /// <summary>
        /// Current Working Time
        /// </summary>
        private DateTime _workTime;
        /// <summary>
        /// Task 동작 시간
        /// </summary>
        private int _workDay = -1;
        /// <summary>
        /// Data Load 여부 확인 Flag
        /// </summary>
        private bool onLoad = false;
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name">Remote SCADA Node Name</param>
        /// <param name="tagPath">Remote Server Tag List 경로</param>
        public RemoteServer(string name, string tagPath)
            : base(name)
        {
            _log = Log.Instance.GetUserLogger(Name, GetType().Name);

            GetMetadata(tagPath);
            CreateMetadata(tagPath, _tokenSource.Token);
            if (lMetadata.Count > 0)
            {
                //Metadata 저장 일자 확인
                _workTime = DateTime.Now;

                //Historian Server Connection Start
                LaunchTime = DateTime.Now;
                LoadTagTask(AppData.Instance.StorageInterval, _tokenSource.Token);
                AppData.Instance.MsgIRDC.Info(_log, Name, "Remote Server 시작");
            }
            else
            {
                AppData.Instance.MsgIRDC.Warn(_log, Name, "대상 Tag가 없습니다. Remote Server를 시작하지 않습니다.");
            }
        }
        /// <summary>
        /// Remote Server Metadata 파일 Load
        /// </summary>
        /// <param name="path">Remote Tag 경로</param>
        private void GetMetadata(string path)
        {
            lMetadata = FileHandler.ReadTagCollection(path);
            AppData.Instance.MsgIRDC.Debug(_log, Name, $"Metadata File Read (Tag Count : {lMetadata.Count})");
        }
        /// <summary>
        /// iWater 연결 Task 중지
        /// </summary>
        public override void EndTask()
        {
            base.EndTask();
            AppData.Instance.MsgIRDC.Debug(_log, Name, "iWater Remote Server 연결 해제");
            lMetadata.Clear();
        }

        #region Load Historian Tag Value
        /// <summary>
        /// Historian Value Load
        /// </summary>
        /// <param name="workTime"></param>
        private int ReadHistorianValue(DateTime workTime)
        {
            var status = (int)EVENT_CODE.Normal;
            var handle = -1;
            try
            {
                status = Connect(out handle);
                if (status == (int)EVENT_CODE.Normal)
                {
                    /*
                    //Tag Name Extraction
                    //string[] aName = new string[1];
                    //aName[0] = "ASWS.606-354-FRI-1052-1.F_CV";
                    */
                    string[] aName = lMetadata.Select(t => t.FullName).ToArray();

                    AppData.Instance.MsgIRDC.Debug(_log, Name, $"Metadata Name Select (Count : [{aName.Length}])");
                    var code = IHUAPI.ihuReadCurrentValue(handle, aName, out IHU_DATA_SAMPLE[] datas, out ihuErrorCode[] codes);

                    //Tag Value Load
                    if (code == ihuErrorCode.OK)
                    {
                        string value = string.Empty;
                        string quality = string.Empty;

                        if (Status == (int)EVENT_CODE.Disconnect)
                        {
                            Status = (int)EVENT_CODE.Normal;
                            AppData.Instance.MsgIRDC.Debug(_log, Name, $"Server Status 재확인[{EVENT_CODE.Normal}]");
                            ChangedStateDelegate(Name, true);
                        }

                        for (int i = 0; i < datas.Length; i++)
                        {
                            if (codes[i] != ihuErrorCode.OK)
                            {
                                AppData.Instance.MsgIRDC.Warn(_log, Name, $"{datas[i].Tagname} Code Error[({(int)codes[i]}){codes[i]}]");
                                continue;
                            }

                            IHU_DATA_SAMPLE item = datas[i];

                            value = ConvertToString(item);
                            quality = item.Quality.QualityStatus.ToString() == "OPCGood" ? "100" : "0";

                            var tag = lMetadata.FirstOrDefault(t => t.FullName.Equals(item.Tagname));

                            if (tag.IsCheck == false)
                            {
                                tag.TimeStamp = $"{workTime:yyyy-MM-dd HH:mm:ss}";
                                tag.Value = value;
                                tag.Quality = quality;
                            }
                            else
                            {
                                tag.TimeStamp = $"{workTime:yyyy-MM-dd HH:mm:ss}";
                                _ = Eda.GetOneFloat(Name, tag.Name, AppData.F_CV, out float pdbvalue);
                                tag.Value = $"{pdbvalue:F4}";
                                tag.Quality = quality;
                            }
                        }
                    }
                    else
                    {
                        AppData.Instance.MsgIRDC.Warn(_log, Name, $"Historian Tag Load Code[({(int)code}){code}]");
                        Status = (int)EVENT_CODE.Fail;
                        ChangedStateDelegate(Name, false);
                    }
                }
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, Name, $"iWater Remote Server[{Name}] Error", ex);
                ChangedStateDelegate(Name, false);
            }
            finally
            {
                if (handle > 0)
                {
                    Disconnect(handle);
                    Status = (int)EVENT_CODE.Disconnect;
                }
                AppData.Instance.MsgIRDC.Info(_log, Name, "Metadata Load iWater Disconnect");
            }

            return status;
        }
        #region 일정 주기 반복 태스크(Value Load) 시작하기 - LoadTagTask(action, interval, token)
        /// <summary>
        /// 일정 주기 반복 태스크 시작하기
        /// </summary>
        /// <param name="interval">간격 (단위 : 밀리초)</param>
        /// <param name="delay">지연 (단위 : 밀리초)</param>
        /// <param name="token">취소 토큰</param>
        /// <returns>태스크</returns>
        private Task LoadTagTask(int interval, CancellationToken token)
        {
            //호출순번 (2)
            Action aWork = async () =>
            {
                AppData.Instance.MsgIRDC.Debug(_log, Name, "Remote Server Load Task 시작");
                if (token.IsCancellationRequested)
                {
                    AppData.Instance.MsgIRDC.Debug(_log, Name, "Remote Server Task 중지 신호 확인");
                    return;
                }
                var now = DateTime.Now;

                //Connection Check
                var check = Connect(out int handle);
                if(check == (int)EVENT_CODE.Normal)
                    Disconnect(handle);

                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        AppData.Instance.MsgIRDC.Debug(_log, Name, "Remote Server Task 중지 신호 확인");
                        break;
                    }

                    now = DateTime.Now;
                    if ((now.Minute - _workTime.Minute) % interval == 0 && now.Second == 0)
                    {
                        var status = (int)EVENT_CODE.Disconnect;
                        if (onLoad == false)
                        {
                            _workTime = now;
                            AppData.Instance.MsgIRDC.Debug(_log, Name, "Current Tag Load 시작");

                            sw.Restart();
                            status = ReadHistorianValue(_workTime);
                            sw.Stop();

                            if (status == (int)EVENT_CODE.Normal)
                            {
                                AppData.Instance.MsgIRDC.Info(_log, Name, $"Current Tag Load 완료 Working Time : {_workTime} | RunTime : {sw.Elapsed}");

                                var fullPath = Path.Combine(Directory, $"[{Name}]{now:yyyy_MM_dd_HH}.csv");
                                AppData.Instance.MsgIRDC.Debug(_log, Name, $"Remote Server Current Tag File Name : {fullPath}");
                                var line = lMetadata.Select(t => t.CSVFormat()).ToArray();
                                if (ExportFile(fullPath, line))
                                {
                                    AppData.Instance.MsgIRDC.Debug(_log, Name, $"Remote Server 마지막 Tag 저장 시간 : {_workTime}");
                                    FileHandler.WriteLastWorkingTime(AppData.Instance.DefaultPath, Name, _workTime);

                                    AppData.Instance.MsgIRDC.Debug(_log, Name, "Remote Server Current Tag File 저장");
                                }
                                else
                                {
                                    AppData.Instance.MsgIRDC.Debug(_log, Name, "Remote Server Current Tag File 실패");
                                }
                            }
                            else
                            {
                                AppData.Instance.MsgIRDC.Warn(_log, Name, $"iWater Remote Server[{status}] Error : File 저장 실패");
                            }
 //이벤트 발생
                                OnHistorianLoad(Name, _workTime, sw.Elapsed);
                            onLoad = true;
                        }
                    }
                    else if (now.Second != 0)
                    {
                        onLoad = false;
                    }

                    if (token.IsCancellationRequested || interval == Timeout.Infinite)
                    {
                        AppData.Instance.MsgIRDC.Debug(_log, Name, "Remote Server Task 중지 신호 확인");
                        break;
                    }
                    //Tag List가 생기지 않아 Sleep 조절 (300배수에서 100배수)
                    Thread.Sleep(100);
                }
            };

            return Task.Factory.StartNew(aWork, token); //호출순번 (1)
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagPath">RemoteServer 설정Metadata Path</param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Task CreateMetadata(string tagPath, CancellationToken token)
        {
            //호출순번 (2)
            Action aWork = async () =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var now = DateTime.Now;
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        AppData.Instance.MsgIRDC.Debug(_log, Name, "Metadata File Move Task 중지 신호 확인");
                        break;
                    }

                    now = DateTime.Now;
                    #region Master Data File 저장 매 0시
                    if (_workDay != now.Day)
                    {
                        _workDay = now.Day;
//                        2024.07.24 ---------------------->수정부분
//                        var result = handler.MoveMetadataFile(this.Directory, path);
                        var ltag = FileHandler.ReadTagCollection(tagPath);
                        var result = FileHandler.MoveMetadataFile(this.Directory, tagPath, ltag);
#if DETAIL_LOG
                        if (result)
                        {
                            AppData.Instance.MsgIRDC.Info(_log, Name, "Metadata File Move");
                        }
                        else
                        {
                            AppData.Instance.MsgIRDC.Warn(_log, Name, "Metadata File Move Fail");
                        }
#endif
                    }
#endregion

                    if (token.IsCancellationRequested)
                    {
                        AppData.Instance.MsgIRDC.Debug(_log, Name, "Metadata File Move Task 중지 신호 확인");
                        break;
                    }
                    Thread.Sleep(1000);
                }
            };

            return Task.Factory.StartNew(aWork, token); //호출순번 (1)
        }
#endregion
#endregion
    }
}
