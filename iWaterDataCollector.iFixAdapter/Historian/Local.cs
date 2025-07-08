using Confluent.Kafka;
using iWaterDataCollector.Global;
using iWaterDataCollector.Model.Tag;
using log4net;
using Proficy.Historian.UserAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace iWaterDataCollector.iFixAdapter.Historian
{
    public class Local : Platforms
    {


        public Local(string name) : base(name)
        {
            ///Log 초기화
            PlatformLog = Log.Instance.GetUserLogger(Name, GetType().Name);

            //LaunchTime = DateTime.Now;
            //LoadTagTask(_interval, _tokenSource.Token);

            lMetadata = new List<LoadTagModel>();

            //Historian Tag 저장 간격
            //_interval = ;
            //Metadata 저장 일자 확인
            //_workTime = DateTime.Now;

            //PDB Tag List File Path
            //PDBPath = AppData.Instance.PDBPath;

            //Historian Server Connection Start
            //onLoad = false;

        }

        #region Task 동작
        protected override void Start()
        {
            if (TagLoadTask != null && !TagLoadTask.IsCompleted)
                return;

            TokenSource = new CancellationTokenSource();
            TagLoadTask = Task.Run(() => RunAsync(TokenSource.Token));
        }

        protected override void Stop()
        {
            base.Dispose();
        }
        #endregion

        #region iWater Connect/DisConnect
        protected override int Connect()
        {
            int handle;
            try
            {
                AppData.Instance.MsgIRDC.Info(PlatformLog, Name, $"IHUAPI.ihuConnect 실행");
                ihuErrorCode code = IHUAPI.ihuConnect(Name, user, password, out handle);
                AppData.Instance.MsgIRDC.Info(PlatformLog, Name, $"[{handle}] Connection Information({code}[{(int)code}])");
                if (code == ihuErrorCode.OK)
                {
                    AppData.Instance.MsgIRDC.Info(PlatformLog, Name, $"[{handle}] Connect Success ({EVENT_CODE.Action}[{(int)EVENT_CODE.Action}])");
                    ChangedStateDelegate(Name, true);
                }
                else
                {
                    AppData.Instance.MsgIRDC.Warn(PlatformLog, Name, $"[Fail to connect ({EVENT_CODE.Fail}[{(int)EVENT_CODE.Fail}])");
                    ChangedStateDelegate(Name, false);
                }
            }
            catch (Exception ex)
            {
                handle = -1;
                AppData.Instance.MsgIRDC.Warn(PlatformLog, Name, $"[Fail to connect ({EVENT_CODE.Error}[{(int)EVENT_CODE.Error}])");
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, Name, $"[Fail to connect ({EVENT_CODE.Error})", ex);
            }

            return handle;
        }

        protected override void Disconnect(int handle)
        {
            _ = IHUAPI.ihuDisconnect(handle);
            AppData.Instance.MsgIRDC.Info(PlatformLog, Name, $"[{handle}]IHUAPI.ihuDisconnect 실행");
        }
        #endregion

        #region Task 실행
        private async Task RunAsync(CancellationToken token)
        {
            var now = DateTime.Now;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(100, token); // 일정 주기마다 반복
                    now = DateTime.Now;
                    #region Master Data File 저장 매 0시
                    if (_workDay != now.Day)
                    {
                        _workDay = now.Day;

#if DETAIL_LOG
                        _message.SetDebug(_log, $"Master Data File 저장 시작 시각 : {now:yyyy-MM-dd HH:mm:ss}");
#endif
                        await CreateMetadata();
#if DETAIL_LOG
                        _message.SetDebug(_log, $"Master Data File 저장 완료 시각 : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
#endif
                        OnReadyMetadata();
                    }
                    #endregion


                    //DeleteOldFiles(_logDirectory);
                    //DeleteOldFiles(Path.Combine(_logDirectory, Code.INFO));
                }
                catch (OperationCanceledException)
                {
                    break; // 정상 종료
                }
                catch (Exception ex)
                {
                    AppData.Instance.MsgIRDC.Warn(PlatformLog, Name, "[TagLoadTask] 오류");
                    AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, Name, "[TagLoadTask] 오류", ex);
                }
            }
        }

        #endregion

    }
}
