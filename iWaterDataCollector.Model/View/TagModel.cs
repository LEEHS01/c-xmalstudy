using iWaterDataCollector.Model.Tag;

namespace iWaterDataCollector.Model.View
{
    /// <summary>
    /// Tag 기본 Class
    /// </summary>
    public class TagModel : ModelBase
    {
        public TagModel(string name, string description, bool isSelected = false)
        {
            IsSelected = isSelected;
            Name = name;
            Description = description;
        }

        public TagModel(string name, string description, bool isKafka, bool isSelected = false)
        {
            Name = name;
            Description = description;
            IsSelected = isSelected;
            if(isKafka)
            {
                Pulse = new PulseTagModel();
            }
        }

        /// <summary>
        /// 선택여부
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }
        private bool _isSelected;
        /// <summary>
        /// Tag 이름
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
        /// Tag 설명
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged(nameof(Description));
                }
            }
        }
        private string _description;
        /// <summary>
        /// Pulse 정보
        /// </summary>
        public PulseTagModel Pulse
        {
            get => _pulse;
            set
            {
                _pulse = value;
            }
        }
        private PulseTagModel _pulse;
    }
}
