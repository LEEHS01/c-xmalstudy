using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.Tag
{
    /// <summary>
    /// Pulse Tag 정보
    /// </summary>
    public class PulseTagModel : ModelBase
    {
        #region 생성자
        public PulseTagModel()
        {
            OnValue = string.Empty;
            OffValue = string.Empty;
            DuringTime = 0;
        }
        public PulseTagModel(string sOn, string sOff, int duringTime)
        {
            OnValue = sOn;
            OffValue = sOff;
            DuringTime = duringTime;
        }
        #endregion

        /// <summary>
        /// On Value
        /// </summary>
        public string OnValue
        {
            get => _onValue;
            set
            {
                if (_onValue != value)
                {
                    _onValue = value;
                    RaisePropertyChanged(nameof(OnValue));
                }
            }
        }
        private string _onValue;
        /// <summary>
        /// 유지시간
        /// </summary>
        public int DuringTime
        {
            get => _duringTime;
            set
            {
                if (_duringTime != value)
                {
                    _duringTime = value;
                    RaisePropertyChanged(nameof(DuringTime));
                }
            }
        }
        private int _duringTime;
        /// <summary>
        /// Off Value
        /// </summary>
        public string OffValue
        {
            get => _offValue;
            set
            {
                if (_offValue != value)
                {
                    _offValue = value;
                    RaisePropertyChanged(nameof(OffValue));
                }
            }
        }
        private string _offValue;
    }
}
