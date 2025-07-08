using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.View
{
    /// <summary>
    /// Timelne Log Class
    /// </summary>
    public class TimelineModel : ModelBase
    {
        /// <summary>
        /// 동작된 Noad Name
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }
        private string _name;
        /// <summary>
        /// Task 시작시간
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    RaisePropertyChanged(nameof(StartTime));
                }
            }
        }
        private DateTime _startTime;
        /// <summary>
        /// Task 동작시간
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

        public DateTime LoggingTime
        {
            get => _loggingTime;
            set
            {
                if (_loggingTime != value)
                {
                    _loggingTime = value;
                    RaisePropertyChanged(nameof(LoggingTime));
                }
            }
        }
        private DateTime _loggingTime;
    }
}
