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
 * iWater Local SCADA 연결 Class
 ********************************************/
namespace iWaterDataCollector.iFixAdapter
{
    public class LocalServer : Server
    {
        #region Log Class 선언
        /// <summary>
        /// Log Interface : Program
        /// </summary>
        private static ILog _log;
        #endregion
        /// <summary>
        /// Tag List Map
        /// </summary>
        private Dictionary<string, int> tagMap = new Dictionary<string, int>();
        /// <summary>
        /// 동작 시간
        /// </summary>
        public Stopwatch sw = new Stopwatch();
        /// <summary>
        /// PDB Tag Path 
        /// </summary>
        public string PDBPath { get; set; }
        /// <summary>
        /// Task 동작 시간
        /// </summary>
        private int _workDay = -1;
        /// <summary>
        /// Save Collection Interval Time
        /// </summary>
        private int _interval = -1;
        /// <summary>
        /// Current Working Time
        /// </summary>
        private DateTime _workTime;
        /// <summary>
        /// Data Load 여부 Flag
        /// </summary>
        private bool onLoad = false;
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name">SCADA NodeName</param>
        public LocalServer(string name)
            : base(name)
        {
            lMetadata = new List<LoadTagModel>();

            _log = Log.Instance.GetUserLogger(Name, GetType().Name);

            //Historian Tag 저장 간격
            _interval = AppData.Instance.StorageInterval;
            //Metadata 저장 일자 확인
            _workTime = DateTime.Now;

            //PDB Tag List File Path
            PDBPath = AppData.Instance.PDBPath;

            //Historian Server Connection Start
            onLoad = false;
            LaunchTime = DateTime.Now;
            LoadTagTask(_interval, _tokenSource.Token);
        }
        /// <summary>
        /// Local Server Disconnect
        /// </summary>
        public override void EndTask()
        {
            base.EndTask();
            AppData.Instance.MsgIRDC.Debug(_log, Name, "iWater Local Server 연결 해제");
            lMetadata.Clear();
            tagMap.Clear();
        }

        #region Metadata Export
        /// <summary>
        /// Historian MetaData Load
        /// </summary>
        /// <param name="interval">주기(sec)</param>
        private Task CreateMetadata()
        {
            if (LoadHistorianTags())
            {
                LoadPDBTags();
                var fullPath = Path.Combine(Directory, Code.INFO, $"{DateTime.Now:yyyyMMdd}_TagList.csv");
                AppData.Instance.MsgIRDC.Debug(_log, Name, $"Local Server Current Tag File Name : {fullPath}");
                var line = lMetadata.Select(t => t.CSVFormat($"{DateTime.Now:yyyy-MM-dd}")).ToArray();
                AppData.Instance.MsgIRDC.Info(_log, Name, $"Metadata Historian Server Load (Tag Count : {lMetadata.Count})");

                ExportFile(fullPath, line, FileMode.Create);
                
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// Historian Server Tag List Load
        /// </summary>
        /// <returns>
        /// Load Tag 완료 여부 <see cref="bool"/> 
        /// </returns>
        private bool LoadHistorianTags()
        {
            var retVal = false;
            //iWater 연결
            if (Connect(out int handle) == (int)EVENT_CODE.Normal)
            {
                ihuErrorCode code;
                try
                {
                    //List 초기화
                    tagMap.Clear();
                    lMetadata.Clear();
                    //총 Tag Count 확인
                    AppData.Instance.MsgIRDC.Debug(_log, Name, "IHUAPI.ihuFetchTagCache 실행");
                    code = IHUAPI.ihuFetchTagCache(handle, "*", out int count);
                    if (code == ihuErrorCode.OK)
                    {
                        AppData.Instance.MsgIRDC.Info(_log, Name, $"Historian Tag Load Count : {count}");
                        //Historian 태그 리스트가져오기 
                        for (int i = 0; i < count; i++)
                        {
                            var tag = new LoadTagModel();
                            AppData.Instance.MsgIRDC.Debug(_log, Name, "IHUAPI.ihuGetStringTagPropertyByIndex 실행");
                            code = IHUAPI.ihuGetStringTagPropertyByIndex(i, ihuTagProperties.Tagname, out string tagName);
                            if (code == ihuErrorCode.OK)
                            {
                                AppData.Instance.MsgIRDC.Info(_log, Name, $"Tag Property[Name : {tagName}");

                                var words = tagName.Split('.');
                                if (words.Length > 2 && words[2] == AppData.F_CV)
                                {
                                    tag.Name = words[1];
                                }
                                else
                                {
                                    tag.Name = tagName;
                                }
                                tag.FullName = tagName;
                            }
                            else
                            {
                                AppData.Instance.MsgIRDC.Debug(_log, Name, $"Historian TagName Load Warn Code[({(int)code}){code.ToString()}]");
                            }
                            //총 Tag Count 확인
                            AppData.Instance.MsgIRDC.Debug(_log, Name, "IHUAPI.ihuGetStringTagPropertyByIndex 실행");
                            code = IHUAPI.ihuGetStringTagPropertyByIndex(i, ihuTagProperties.Description, out string desc);
                            if (code == ihuErrorCode.OK)
                            {
                                AppData.Instance.MsgIRDC.Debug(_log, Name, $"Tag Property[Description : {desc}");
                                tag.Description = desc.Replace(',', '*');
                            }
                            else
                            {
                                AppData.Instance.MsgIRDC.Debug(_log, Name, $"Historian Tag Description Load Warn Code[({(int)code}){code.ToString()}]");
                            }
                            //실제 데이터
                            lMetadata.Add(tag);
                            tagMap.Add(tag.FullName, i);
                        }
                        AppData.Instance.MsgIRDC.Debug(_log, Name, "Metadata Load 완료");
                    }
                    else
                    {
                        AppData.Instance.MsgIRDC.Debug(_log, Name, $"Historian Tag Load Warn Code[({(int)code}){code}]");
                    }
                    retVal = true;
                }
                catch (Exception ex)
                {
                    
                    Status = (int)EVENT_CODE.Fail;
                    AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, Name, "Historian Tag Load Error", ex);
                    retVal = false;
                }
                finally
                {
                    if (handle > 0)
                    {
                        Disconnect(handle);
                        Status = (int)EVENT_CODE.Disconnect;
                    }
                    AppData.Instance.MsgIRDC.Debug(_log, Name, "Metadata Load iWater Disconnect");
                }
            }
            return retVal;
        }
        /// <summary>
        /// PDB Setting Tag List File Load
        /// </summary>
        private void LoadPDBTags()
        {
            //PDB Tag List 경로가 없는 경우 넘기기
            try
            {
                if (string.IsNullOrEmpty(PDBPath))
                {
                    AppData.Instance.MsgIRDC.Debug(_log, Name, "PDBPath가 지정되어있지 않습니다.");
                    return;
                }

                var names = FileHandler.ReadTagNameCollection(PDBPath);
                //PDB Tag 없는 경우 예외처리
                AppData.Instance.MsgIRDC.Debug(_log, Name, $"PDB Tag Count : {names.Count}");
                if (names.Count > 0)
                {
                    lMetadata.Where(t => names.Contains(t.Name))
                         .ToList()
                         .ForEach(t => t.IsCheck = true);
                }
            }
            catch (Exception ex)
            {
                Status = (int)EVENT_CODE.Fail;
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, Name, "Historian Tag PDB Check Error", ex);
            }
        }
        #endregion
        #region Load Historian Tag Value
        private void ReadiWaterValue(DateTime now)
        {
            var status = (int)EVENT_CODE.Disconnect;
            AppData.Instance.MsgIRDC.Debug(_log, Name, "Current Tag Load 시작");

            sw.Restart();
            status = ReadHistorianValue(now);
            sw.Stop();

            if (status == (int)EVENT_CODE.Normal)
            {
                AppData.Instance.MsgIRDC.Debug(_log, Name, $"Current Tag Load 완료 Working Time : {_workTime} | RunTime : {sw.Elapsed}");
                var fullPath = Path.Combine(Directory, $"[{Name}]{now:yyyy_MM_dd_HH}.csv");
                AppData.Instance.MsgIRDC.Debug(_log, Name, $"Current Tag File Save [파일명 : {fullPath}]");

                var line = lMetadata.Select(t => t.CSVFormat()).ToArray();
                if (ExportFile(fullPath, line))
                {
                    AppData.Instance.MsgIRDC.Debug(_log, Name, $"Server 마지막 Tag 저장 시간 : {_workTime}");
                    FileHandler.WriteLastWorkingTime(AppData.Instance.DefaultPath, Name, _workTime);
                    onLoad = true;
                    AppData.Instance.MsgIRDC.Info(_log, Name, "Local Server Current Tag File 저장");
                }
                else
                {
                    AppData.Instance.MsgIRDC.Warn(_log, Name, "Local Server Current Tag File 실패");
                }
            }
            else
            {
                AppData.Instance.MsgIRDC.Warn(_log, Name, $"iWater Local Server[{status}] Error");
            }
        }
        /// <summary>
        /// Historian Value Load
        /// </summary>
        /// <param name="workTime"></param>
        private int ReadHistorianValue(DateTime workTime)
        {
            var status = (int)EVENT_CODE.Disconnect;
            var handle = -1;

            try
            {
                status = Connect(out handle);
                if (status == (int)EVENT_CODE.Normal)
                {
                    //Tag Name Extraction
                    var aName = lMetadata.Select(t => t.FullName).ToArray();
                    var code = IHUAPI.ihuReadCurrentValue(handle, aName, out IHU_DATA_SAMPLE[] datas, out ihuErrorCode[] codes);
                    AppData.Instance.MsgIRDC.Debug(_log, Name, $"Historian Tag Load Code[({(int)code}){code.ToString()}]");
                    if (code == ihuErrorCode.OK)
                    {
                        string value = string.Empty;
                        string quality = string.Empty;
                        if (Status == (int)EVENT_CODE.Disconnect)
                        {
                            Status = (int)EVENT_CODE.Normal;
                            AppData.Instance.MsgIRDC.Debug(_log, Name, $"Server Status 재확인[{EVENT_CODE.Normal.ToString()}]");
                            ChangedStateDelegate(Name, true);
                        }

                        for (int i = 0; i < datas.Length; i++)
                        {
                            if (codes[i] == ihuErrorCode.OK)
                            {
                                IHU_DATA_SAMPLE item = datas[i];

                                AppData.Instance.MsgIRDC.Debug(_log, Name, $"{item.Tagname} 데이터 Convert");
                                value = ConvertToString(item);
                                AppData.Instance.MsgIRDC.Debug(_log, Name, $"{item.Tagname} 데이터 Convert 완료 : [Value : [{value}]");

                                quality = item.Quality.QualityStatus.ToString() == "OPCGood" ? "100" : "0";

                                AppData.Instance.MsgIRDC.Debug(_log, Name, $"{item.Tagname} [quality : [{quality}]");
                                if (tagMap.TryGetValue(item.Tagname, out int idx))
                                {
                                    var tag = lMetadata[idx];
                                    if (tag.IsCheck == false)
                                    {
                                        tag.TimeStamp = workTime.ToString("yyyy-MM-dd HH:mm:ss");
                                        tag.Value = value;
                                        tag.Quality = quality;
                                    }
                                    else
                                    {
                                        tag.TimeStamp = workTime.ToString("yyyy-MM-dd HH:mm:ss");
                                        _ = Eda.GetOneFloat(Name, tag.Name, AppData.F_CV, out float pdbvalue);
                                        AppData.Instance.MsgIRDC.Debug(_log, Name, $"{item.Tagname} [Value : [{pdbvalue}]");
                                        tag.Value = $"{pdbvalue:F4}";
                                        tag.Quality = quality;
                                    }

                                    lMetadata[idx] = tag;
                                }
                            }
                            else
                            {
                                AppData.Instance.MsgIRDC.Warn(_log, Name, $"{datas[i].Tagname} Code Error[({(int)code}){code}]");
                            }
                        }
                    }
                    else
                    {
                        AppData.Instance.MsgIRDC.Warn(_log, Name, $"Current Tag[{Name}] List Load Error[({(int)code}){code}]");
                        Status = (int)EVENT_CODE.Fail;
                        ChangedStateDelegate(Name, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Status = (int)EVENT_CODE.Disconnect;
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, Name, $"iWater Local Server[{Name}] Error", ex);
                ChangedStateDelegate(Name, false);
            }
            finally
            {
                if (handle > 0)
                {
                    Disconnect(handle);
                    Status = (int)EVENT_CODE.Disconnect;
                }
                AppData.Instance.MsgIRDC.Debug(_log, Name, "Metadata Load iWater Disconnect");
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
                //var status = (int)EVENT_CODE.Disconnect;
                if (token.IsCancellationRequested)
                {
                    AppData.Instance.MsgIRDC.Info(_log, Name, "Local Server Task 중지 신호 확인");
                    return;
                }

                var now = DateTime.Now;
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        AppData.Instance.MsgIRDC.Info(_log, Name, "Local Server Task 중지 신호 확인");
                        break;
                    }
                    now = DateTime.Now;
                    #region Master Data File 저장 매 0시
                    if (_workDay != now.Day)
                    {
                        _workDay = now.Day;

                        AppData.Instance.MsgIRDC.Debug(_log, Name, $"Master Data File 저장 시작 시각 : {now:yyyy-MM-dd HH:mm:ss}");
                        await CreateMetadata();
                        AppData.Instance.MsgIRDC.Debug(_log, Name, $"Master Data File 저장 완료 시각 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        OnReadyMetadata();
                    }
                    #endregion

                    if ((now.Minute - _workTime.Minute) % interval == 0 && now.Second == 0)
                    {
                        if (onLoad == false)
                        {
                            _workTime = now;
                            AppData.Instance.MsgIRDC.Debug(_log, Name, $"Tag Read 동작 시각 : {_workTime:yyyy-MM-dd HH:mm:ss}");
                            ReadiWaterValue(now);
                            AppData.Instance.MsgIRDC.Debug(_log, Name, $"Tag Read 동작 완료 시각 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                            onLoad = true;
                            //동작시간 위치 이동 [2023-11-09]
                            OnHistorianLoad(Name, _workTime, sw.Elapsed);
                        }
                    }
                    else if (now.Second != 0)
                    {
                        onLoad = false;
                    }

                    if (token.IsCancellationRequested || interval == Timeout.Infinite)
                    {
                        AppData.Instance.MsgIRDC.Info(_log, Name, "Local Server Task 중지 신호 확인");
                        break;
                    }
                    //Tag List가 생기지 않아 Sleep 조절 (300배수에서 100배수)
                    Thread.Sleep(100);
                }
            };

            return Task.Factory.StartNew(aWork, token); //호출순번 (1)
        }
        #endregion
        #endregion
    }
}
