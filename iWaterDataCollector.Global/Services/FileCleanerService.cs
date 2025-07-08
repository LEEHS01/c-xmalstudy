using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Windows.Shapes;

namespace iWaterDataCollector.Global.Services
{
    public class FileCleanerService
    {
        private string _name;
        private CancellationTokenSource _tokenSource;
        private Task _cleaningTask;
        private readonly string _logDirectory;
        private readonly int _retentionDays;
        private readonly TimeSpan _interval;

        public FileCleanerService(string name, string logDirectory, int retentionDays, TimeSpan interval)
        {
            _name = $"{name}[{GetType().Name}]";
            _logDirectory = logDirectory;
            _retentionDays = retentionDays;
            _interval = interval;
        }

        public void Start()
        {
            if (_cleaningTask != null && !_cleaningTask.IsCompleted)
                return;

            _tokenSource = new CancellationTokenSource();
            _cleaningTask = Task.Run(() => RunAsync(_tokenSource.Token));
        }

        public void Stop()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }
            _tokenSource.Dispose();
        }

        private async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_interval, token); // 일정 주기마다 반복

                    DeleteOldFiles(_logDirectory);
                    DeleteOldFiles(Path.Combine(_logDirectory, Code.INFO));
                }
                catch (OperationCanceledException)
                {
                    break; // 정상 종료
                }
                catch (Exception ex)
                {
                    AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _name, "[FileCleaner] 오류", ex);
                }
            }
        }

        private void DeleteOldFiles(string path)
        {
            if (!Directory.Exists(path))
                return;

            var today = DateTime.Today;
            var files = Directory.GetFiles(path, "*");
            var cnt = 0;
            foreach (var file in files)
            {
                try
                {
                    var creationDate = File.GetLastWriteTime(file).Date;
                    if ((today - creationDate).TotalDays > _retentionDays)
                    {
                        File.Delete(file);
                        cnt++;
                    }
                }
                catch (Exception ex)
                {
                    AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _name, $"[FileCleaner] 파일 삭제 실패: {file}", ex);
                }
            }

            AppData.Instance.MsgIRDC.Info(AppData.AppLog, _name, $"[FileCleaner]파일 경로 [{ path }] 파일({cnt}건) 삭제됨");
        }
    }

}
