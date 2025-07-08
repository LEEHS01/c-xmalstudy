using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.INI
{
    /// <summary>
    /// Kafka Server Setting Class
    /// </summary>
    public class KafkaModel : ModelBase
    {
        public KafkaModel()
        {
            EndPoint = string.Empty;
            Topic = string.Empty;
            ConsumerID = string.Empty;
        }
        /// <summary>
        /// Kafka Server IP
        /// </summary>
        public string EndPoint
        {
            get => _endPoint;
            set
            {
                if (_endPoint != value)
                {
                    _endPoint = value;
                    RaisePropertyChanged(nameof(EndPoint));
                }
            }
        }
        private string _endPoint;
        public string ConsumerID
        {
            get => _consumerID;
            set
            {
                if (_consumerID != value)
                {
                    _consumerID = value;
                    RaisePropertyChanged(nameof(ConsumerID));
                }
            }
        }
        private string _consumerID;

        /// <summary>
        /// Kafka Topic
        /// </summary>
        public string Topic
        {
            get => _topic;
            set
            {
                if (_topic != value)
                {
                    _topic = value;
                    RaisePropertyChanged(nameof(Topic));
                }
            }
        }
        private string _topic;

        public bool UseFilter
        {
            get => _useFilter;
            set
            {
                if (_useFilter != value)
                {
                    _useFilter = value;
                    RaisePropertyChanged(nameof(UseFilter));
                }
            }
        }
        private bool _useFilter;
    }
}
