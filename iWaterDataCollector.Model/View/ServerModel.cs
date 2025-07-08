using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.View
{
    /// <summary>
    /// View용 iWater Info Class
    /// </summary>
    public class ServerModel : ModelBase
    {
        private static readonly string Local = "Local";
        private static readonly string Remote = "Remote";

        public ServerModel()
        {
            NodeName = string.Empty;
            Directory = string.Empty;
            IsState = false;
            IsLocal = false;
        }
        public ServerModel(string name, string path, bool isLocal = false)
        {
            NodeName = name;
            Directory = path;
            IsState = false;
            IsLocal = isLocal;
        }
        /// <summary>
        /// Server 연결 상태
        /// </summary>
        public bool IsState
        {
            get => _isState;
            set
            {
                if (_isState != value)
                {
                    _isState = value;
                    RaisePropertyChanged(nameof(IsState));
                }
            }
        }
        private bool _isState;
        /// <summary>
        /// Noad Name
        /// </summary>
        public string NodeName
        {
            get => _nodeName;
            set
            {
                if (_nodeName != value)
                {
                    _nodeName = value;
                    RaisePropertyChanged(nameof(NodeName));
                }
            }
        }
        private string _nodeName;
        /// <summary>
        /// Local 서버 여부
        /// </summary>
        public bool IsLocal
        {
            get => _isLocal;
            set
            {
                if (_isLocal != value)
                {
                    _isLocal = value;
                    RaisePropertyChanged(nameof(IsLocal));
                }
            }
        }
        private bool _isLocal;
        /// <summary>
        /// Local 서버 여부에 따른 Server Type String
        /// </summary>
        public string Type => IsLocal ? Local : Remote;
        /// <summary>
        /// 서버 폴더 경로
        /// </summary>
        public string Directory
        {
            get => _directory;
            set
            {
                if (_directory != value)
                {
                    _directory = value;
                    RaisePropertyChanged(nameof(Directory));
                }
            }
        }
        private string _directory;
        /// <summary>
        /// Task 동작 시간
        /// </summary>
        public TimeSpan RunningTime
        {
            get => _runningTime;
            set
            {
                if (_runningTime != value)
                {
                    _runningTime = value;
                    RaisePropertyChanged(nameof(RunningTime));
                }
            }
        }
        private TimeSpan _runningTime;
        /// <summary>
        /// 최대 Task 동작 시간
        /// </summary>
        public TimeSpan MaxRunningTime
        {
            get => _maxRunningTime;
            set
            {
                if (_maxRunningTime != value)
                {
                    _maxRunningTime = value;
                    RaisePropertyChanged(nameof(MaxRunningTime));
                }
            }
        }
        private TimeSpan _maxRunningTime;

        #region Server Connection용 Default 계정정보
        public string User = "";
        public string Password = "";
        #endregion
        /// <summary>
        /// CSV파일 형식 내포함수
        /// </summary>
        /// <param name="x"></param>
        public static implicit operator string(ServerModel x) { return x.CSVFormat(); }
        /// <summary>
        /// 내부함수 본문
        /// </summary>
        /// <returns></returns>
        public string CSVFormat()
        {
            return string.Join(",", NodeName, Directory);
        }

        public event Action<bool> OnFilter;
        private void FilteringAction(bool isVisible)
        {
            this.OnFilter?.Invoke(isVisible);
        }
    }
}
