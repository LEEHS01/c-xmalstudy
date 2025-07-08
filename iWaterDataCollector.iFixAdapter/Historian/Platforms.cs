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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iWaterDataCollector.iFixAdapter.Historian
{
    public abstract class Platforms : IDisposable
    {
        #region Log Class 선언
        /// <summary>
        /// Log Interface : Program
        /// </summary>
        public static ILog PlatformLog;
        #endregion
        /// <summary>
        /// Server User Name;
        /// Default Value is Empty
        /// </summary>
        protected readonly string user = string.Empty;
        /// <summary>
        /// Server User Password;
        /// Default Value is Empty
        /// </summary>
        protected readonly string password = string.Empty;
        /// <summary>
        /// Task Class 변수
        /// </summary>
        protected Task TagLoadTask;
        /// <summary>
        /// Task Token
        /// </summary>
        protected CancellationTokenSource TokenSource;
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
        protected string Name
        {
            get => _name;
        }
        private string _name;
        /// <summary>
        /// Historian Server Info Directory
        /// </summary>
        protected string Directory
        {
            get => _path;
            set => _path = value;
        }
        private string _path;
        /// <summary>
        /// Historian Connection Time
        /// </summary>
        protected DateTime LaunchTime;

        protected int Interval => AppData.Instance.StorageInterval;
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name">NodeName</param>
        public Platforms(string name)
        {
            _name = name;
            _path = DirectoryHandler.GetDefaultDirectory(AppData.Instance.DefaultPath, name);
            //디렉토리 없는경우 디렉토리 생성
            DirectoryHandler.CreateDirectory(_path);
            StartFileCleaner();
        }

        protected abstract int Connect();

        protected abstract void Disconnect(int handle);

        protected abstract void Start();

        protected abstract void Stop();

        #region 보관기간이 지난 파일 삭제 [20250627]
        private void StartFileCleaner()
        {
            fileCleaner = new FileCleanerService(Name, Directory, AppData.Instance.ArchiveDuration, TimeSpan.FromDays(1));
            fileCleaner.Start();
        }
        #endregion

        /// <summary>
        /// Historian Server Value Convert(String)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string ConvertToString(IHU_DATA_SAMPLE item)
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

        public void Dispose()
        {
            fileCleaner.Stop();
            fileCleaner = null;
            lMetadata.Clear();
            lMetadata = null;
            TokenSource.Cancel();
            TokenSource.Dispose();
        }
        #region Delegate 및 event 정의
        public event Action<string, bool> ChangedState;

        protected void ChangedStateDelegate(string name, bool isConnection)
        {
            ChangedState?.Invoke(name, isConnection);
        }
        #endregion
    }
}
