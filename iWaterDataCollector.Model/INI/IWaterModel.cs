using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.INI
{
    /// <summary>
    /// iWater Setting Class
    /// </summary>
    public class IWaterModel : ModelBase
    {
        public IWaterModel()
        { }
        public bool IsAutoConnection
        {
            get => _isAutoConnection;
            set
            {
                if(_isAutoConnection != value)
                {
                    _isAutoConnection = value;
                    RaisePropertyChanged(nameof(IsAutoConnection));
                }
            }
        }
        private bool _isAutoConnection;
        /// <summary>
        /// Tag Value 저장주기
        /// </summary>
        public int StorageInterval
        {
            get => _storageInterval;
            set
            {
                if (_storageInterval != value)
                {
                    _storageInterval = value;
                    RaisePropertyChanged(nameof(StorageInterval));
                }
            }
        }
        private int _storageInterval;
        /// <summary>
        /// 파일 보관 기간
        /// </summary>
        public int LogArchiveDuration
        {
            get => _logrchiveDuration;
            set
            {
                if (_logrchiveDuration != value)
                {
                    _logrchiveDuration = value;
                    RaisePropertyChanged(nameof(LogArchiveDuration));
                }
            }
        }
        private int _logrchiveDuration;
        /// <summary>
        /// 파일 보관 기간
        /// </summary>
        public int ArchiveDuration
        {
            get => _archiveDuration;
            set
            {
                if (_archiveDuration != value)
                {
                    _archiveDuration = value;
                    RaisePropertyChanged(nameof(ArchiveDuration));
                }
            }
        }
        private int _archiveDuration;
        /// <summary>
        /// 누실데이터 백업 최대 기간
        /// </summary>
        public int RecoveryMaxDuration
        {
            get => _recoveryMaxDuration;
            set
            {
                if (_recoveryMaxDuration != value)
                {
                    _recoveryMaxDuration = value;
                    RaisePropertyChanged(nameof(RecoveryMaxDuration));
                }
            }
        }
        private int _recoveryMaxDuration;
        /// <summary>
        /// 누실데이터 백업 사용유무
        /// </summary>
        public bool UseRecovery
        {
            get => _useRecovery;
            set
            {
                if(_useRecovery != value)
                {
                    _useRecovery = value;
                    RaisePropertyChanged(nameof(UseRecovery));
                }
            }
        }
        private bool _useRecovery;
        /// <summary>
        /// Local Server 정보
        /// </summary>
        public string Local
        {
            get => _local;
            set
            {
                if (_local != value)
                {
                    _local = value;
                    RaisePropertyChanged(nameof(Local));
                }
            }
        }
        private string _local;
        /// <summary>
        /// Remote Server 정보
        /// </summary>
        public string Remote
        {
            get => _remote;
            set
            {
                if (_remote != value)
                {
                    _remote = value;
                    RaisePropertyChanged(nameof(Remote));
                }
            }
        }
        private string _remote;

        public int PDBSetInterval
        {
            get => _pdbSetInterval;
            set
            {
                if (value != _pdbSetInterval)
                {
                    _pdbSetInterval = value;
                    RaisePropertyChanged(nameof(PDBSetInterval));
                }
            }
        }
        private int _pdbSetInterval;
    }
}
