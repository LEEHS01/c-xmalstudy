using iWaterDataCollector.Global;
using iWaterDataCollector.Global.Handler;
using iWaterDataCollector.Global.Services;
using iWaterDataCollector.Model.Tag;
using log4net;
using Proficy.Historian.UserAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/********************************************
 * iWater 연결 Main Class
 ********************************************/
namespace iWaterDataCollector.iFixAdapter.Common
{
    public abstract class Server
    {
        /// <summary>
        /// Task Token
        /// </summary>
        protected CancellationTokenSource _tokenSource = new CancellationTokenSource();
        /// <summary>
        /// Master Tag List
        /// </summary>
        protected List<LoadTagModel> lMetadata;
        /// <summary>
        /// 로그파일 삭제 Task Class
        /// </summary>
        private FileCleanerService fileCleaner;
        /// <summary>
        /// Historian Server Name
        /// </summary>
        public string Name
        {
            get => _name;
        }
        private string _name;
        /// <summary>
        /// 프로젝트 기본 경로
        /// </summary>
        public string DefaultDirectory
        {
            get => _defaultDirectory;
            set => _defaultDirectory = value;
        }
        private string _defaultDirectory;
        /// <summary>
        /// Historian Server Info Directory
        /// </summary>
        public string Directory
        {
            get => _path;
            set => _path = value;
        }
        private string _path;
        /// <summary>
        /// Historian Server 상태
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Historian Tag Export Path
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// Server User Name;
        /// Default Value is Empty
        /// </summary>
        private readonly string _user = string.Empty;
        /// <summary>
        /// Server User Password;
        /// Default Value is Empty
        /// </summary>
        private readonly string _password = string.Empty;
        /// <summary>
        /// 파일삭제 시간
        /// </summary>
        private int _deleteTime = 0;
        /// <summary>
        /// Historian Connection Time
        /// </summary>
        protected DateTime LaunchTime;
        /// <summary>
        /// Last Working Time
        /// </summary>
        private DateTime _lastWorkTime;
        /// <summary>
        /// 최대 복구 시간
        /// </summary>
        private int _maxDuration;
        /// <summary>
        /// 복구 사용유무
        /// </summary>
        private bool _isUseRecovery;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name">NodeName</param>
        public Server(string name)
        {
            Status = (int)EVENT_CODE.Disconnect;
            _name = name;
            //디렉토리 지정
            _defaultDirectory = AppData.Instance.DefaultPath;
            _path = DirectoryHandler.GetDefaultDirectory(_defaultDirectory, name);
            //디렉토리 없는경우 디렉토리 생성
            DirectoryHandler.CreateDirectory(_path);
            //최대 복구 시간
            _maxDuration = AppData.Instance.RecoveryMaxDuration;
            _isUseRecovery = AppData.Instance.UseRecovery;
            //마지막 Load 시간 Read
            _lastWorkTime = FileHandler.LoadLastWorkingTime(_defaultDirectory, name);
            if (_isUseRecovery)
            {
                OnRecovery += Server_OnRecovery;
            }
            StartFileCleaner();
            //쌓인 Tag List 파일 삭제
            //DeleteFile();
        }

        protected int Connect(out int handle)
        {
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, $"[{Name}]IHUAPI.ihuConnect 실행");
            ihuErrorCode code = IHUAPI.ihuConnect(Name, _user, _password, out handle);
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, $"[{Name}:{handle}] Connection Information({code.ToString()}[{(int)code}])");
            if (code == ihuErrorCode.OK)
            {
                Status = (int)EVENT_CODE.Normal;
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, $"[{Name}:{handle}] Connect Success ({EVENT_CODE.Action.ToString()}[{(int)EVENT_CODE.Action}])");
                ChangedStateDelegate(Name, true);
            }
            else
            {
                Status = (int)EVENT_CODE.Disconnect;
                AppData.Instance.MsgIRDC.Warn(AppData.AppLog, Name, $"[{Name}] Fail to connect ({EVENT_CODE.Fail.ToString()}[{(int)EVENT_CODE.Fail}])");
                ChangedStateDelegate(Name, false);
            }

            return Status;
        }
        
        protected void Disconnect(int handle)
        {
            _ = IHUAPI.ihuDisconnect(handle);
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, $"[{Name}:{handle}]IHUAPI.ihuDisconnect 실행");
        }

        public virtual void EndTask()
        {
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, $"[{Name}]Historian Task Cancel Response");
            _tokenSource.Cancel();
            fileCleaner?.Stop();
            Status = (int)EVENT_CODE.Disconnect;
            ChangedStateDelegate(Name, false);
            if (_isUseRecovery)
            {
                OnRecovery -= Server_OnRecovery;
            }
        }

        #region 일정 주기 반복 태스크 시작하기 - StartPeriodicTask(interval, delay, token)
        /// <summary>
        /// 일정 주기 반복 태스크 시작하기
        /// </summary>
        /// <param name="interval">간격 (단위 : 밀리초)</param>
        /// <param name="delay">지연 (단위 : 밀리초)</param>
        /// <param name="token">취소 토큰</param>
        /// <returns>태스크</returns>
        protected Task HistorianTask(Action action, int interval, CancellationToken token)
        {
            Action aWrapper = () =>
            {
                if (token.IsCancellationRequested)
                {
                    AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, "Server Task 중지 신호 확인");
                    return;
                }

                action();
            };

            //호출순번 (2)
            Action aWork = () =>
            {
                if (token.IsCancellationRequested)
                {
                    AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, "Server Task 중지 신호 확인");
                    return;
                }

                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, "Server Task 중지 신호 확인");
                        break;
                    }

                    //Task.Factory.StartNew(aWrapper, token);
                    Task.Run(aWrapper, token);

                    if (token.IsCancellationRequested || interval == Timeout.Infinite)
                    {
                        AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, "Server Task 중지 신호 확인");
                        break;
                    }
                    Thread.Sleep(interval);
                }
            };

            return Task.Factory.StartNew(aWork, token); //호출순번 (1)
        }
        #endregion


        #region 보관기간이 지난 파일 삭제 [20250627]
        private void StartFileCleaner()
        {
            fileCleaner = new FileCleanerService(Name, Directory, AppData.Instance.ArchiveDuration, TimeSpan.FromDays(1));
            fileCleaner.Start();
        }
        #endregion

        #region Recovery Historian Tag Value
        private Task RecoveryTask(CancellationToken token)
        {
            Action aWork = () =>
            {
                IHU_RETRIEVED_RAW_VALUES[] values = null;
                ihuErrorCode[] results = null;
                int handle = -1;
                try
                {
                    //Tag Name Extraction
                    var aName = lMetadata.Select(t => t.FullName).ToArray();
                    var lTagInfo = new List<LoadTagModel>();

                    foreach (var name in aName)
                    {
                        lTagInfo.Add(new LoadTagModel()
                        {
                            FullName = name
                        });
                    }
                    var recoveryTiem = LaunchTime - _lastWorkTime;

                    AppData.Instance.MsgIRDC.Debug(AppData.AppLog, Name, $"누실 데이터 복구 가능 최대시간 : {_maxDuration}");
                    AppData.Instance.MsgIRDC.Debug(AppData.AppLog, Name, $"누실 데이터 복구 시간 : {recoveryTiem}");
                    //최대 복구 시간 사용자 설정 값으로 등록
                    if (recoveryTiem.Days >= _maxDuration)
                    {
                        _lastWorkTime = LaunchTime.AddDays(_maxDuration * (-1));
                        AppData.Instance.MsgIRDC.Debug(AppData.AppLog, Name, $"누실 데이터 복구 시간 재계산 : {LaunchTime - _lastWorkTime}");
                    }
                    //iWater 연결 시 마다 Connection
                    if (Connect(out handle) == (int)EVENT_CODE.Normal)
                    {
                        while (LaunchTime.AddMinutes(-1) > _lastWorkTime)
                        {
                            var endTime = _lastWorkTime.AddMinutes(1);
                            IHU_TIMESTAMP startTimeStamp = new IHU_TIMESTAMP(_lastWorkTime.ToUniversalTime());
                            IHU_TIMESTAMP endTimeStamp = new IHU_TIMESTAMP(endTime.ToUniversalTime());

                            if (IHUAPI.ihuReadMultiTagRawDataByTime(handle, aName, startTimeStamp, endTimeStamp, out values, out results) == ihuErrorCode.OK)
                            {
                                if (values != null)
                                {
                                    foreach (var item in values)
                                    {
                                        //저장 Class
                                        var model = lTagInfo.Where(t => t.FullName.Equals(item.Tagname)).FirstOrDefault();
                                        //Load된 Tag Info
                                        var tag = item.Values.ToList().FirstOrDefault(t => t.TimeStamp.ToDateTime().ToLocalTime().Second == 0);

                                        //model.FullName = item.Tagname;
                                        model.Name = item.Tagname.Split('.')[1];
                                        model.Value = ConvertToString(tag);
                                        model.TimeStamp = tag.TimeStamp.ToDateTime().ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                        model.Quality = tag.Quality.QualityStatus.ToString() == "OPCGood" ? "100" : "0";
                                    }

                                    var fullPath = Path.Combine(Directory, Code.RECOVERY, $"[{Name}]{endTime:yyyy_MM_dd_HH}.csv");
                                    var line = lTagInfo.Select(t => t.CSVFormat()).ToArray();
                                    var result = ExportFile(fullPath, line);
                                    if (result)
                                        AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, "Tag 파일 저장 완료");
                                    else
                                        AppData.Instance.MsgIRDC.Warn(AppData.AppLog, Name, "Tag 파일 저장에 문제가 생겼습니다.");
                                }
                            }
                            _lastWorkTime = endTime;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, Name, "Recovery Error", ex);
                }
                finally
                {
                    if (handle > 0)
                    {
                        Disconnect(handle);
                        Status = (int)EVENT_CODE.Disconnect;
                    }
#if DETAIL_LOG
                    AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, "Metadata Load iWater Disconnect");
#endif
                }
            };
            return Task.Factory.StartNew(aWork, token);
        }
        #endregion

        /// <summary>
        /// Historian Loss Tag Data Recovery
        /// </summary>
        protected void Server_OnRecovery()
        {
            //이전 구동 시간이 마지막 Historian Load 시간보다 1분이상 길 경우 누락데이터 Load
            if (LaunchTime - _lastWorkTime > TimeSpan.FromMinutes(1))
            {
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, Name, "Recovery Task 시작");
                RecoveryTask(_tokenSource.Token);
            }
        }

        /// <summary>
        /// Historian Server Value Convert(String)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string ConvertToString(IHU_DATA_SAMPLE item)
        {
            string value;
            switch (item.ValueDataType)
            {
                case ihuDataType.Short:
                    value = $"{item.Value.Short}";
                    break;
                case ihuDataType.Integer:
                    value = $"{item.Value.Integer}";
                    break;
                case ihuDataType.Float:
                    value = $"{item.Value.Float:F4}";
                    break;
                case ihuDataType.DoubleFloat:
                    value = $"{item.Value.DoubleFloat:F4}";
                    break;
                default:
                    {
                        value = "0";
                    }
                    break;
            }

            return value;
        }

        /// <summary>
        /// File Export
        /// </summary>
        protected bool ExportFile(string path, string[] lines, FileMode mode = FileMode.Append)
        {
            return FileHandler.WriteTagCSVFile(path, lines, mode);
        }

        #region Delegate 및 event 정의
        /// <summary>
        /// Connection Event
        /// </summary>
        public event Action OnConnected;
        /// <summary>
        /// Connection Event Delegate
        /// </summary>
        public void OnConnection()
        {
            OnConnected?.Invoke();
        }

        public event Action<string, bool> ChangedState;

        public void ChangedStateDelegate(string name, bool isConnection)
        {
            ChangedState?.Invoke(name, isConnection);
        }
        /// <summary>
        /// 누실데이터 백업 Event
        /// </summary>
        public event Action OnRecovery;
        /// <summary>
        /// 누실데이터 백업 Event Delegate
        /// </summary>
        public void OnReadyMetadata()
        {
            OnRecovery?.Invoke();
        }

        public event Action<string, DateTime, TimeSpan> OnLoad;

        public void OnHistorianLoad(string name, DateTime dt, TimeSpan ts)
        {
            OnLoad?.Invoke(name, dt, ts);
        }
        #endregion
    }
}
