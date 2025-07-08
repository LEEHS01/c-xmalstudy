using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.INI
{
    /// <summary>
    /// 이중화 Setting Class
    /// </summary>
    public class RedundancyModel : ModelBase
    {
        /// <summary>
        /// 이중화 사용 여부
        /// </summary>
        public bool IsUse
        {
            get => _isUse;
            set
            {
                if (_isUse != value)
                {
                    _isUse = value;
                    RaisePropertyChanged(nameof(IsUse));
                }
            }
        }
        private bool _isUse;
        /// <summary>
        /// 이중화 Primary 서버 체크
        /// </summary>
        public bool IsPrimary
        {
            get => _isPrimary;
            set
            {
                if (_isPrimary != value)
                {
                    _isPrimary = value;
                    RaisePropertyChanged(nameof(IsPrimary));
                }
            }
        }
        private bool _isPrimary;
        /// <summary>
        /// 이중화 Primary 서버 IP
        /// </summary>
        public string IP
        {
            get => _ip;
            set
            {
                if (_ip != value)
                {
                    _ip = value;
                    RaisePropertyChanged(nameof(IP));
                }
            }
        }
        private string _ip;
        /// <summary>
        /// 이중화 Primary 서버 Port
        /// </summary>
        public int Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    RaisePropertyChanged(nameof(Port));
                }
            }
        }
        private int _port;
    }
}
