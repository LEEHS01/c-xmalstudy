using iWaterDataCollector.Global;
using iWaterDataCollector.Model.INI;
using System.ComponentModel;

/********************************************
 * Redundancy Setting Page
 * Kafka 동작을 위한 이중화 설정 화면 ViewModel
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.ViewModel.UserControl
{
    public class RedundancySettingUCViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Binding Data
        public RedundancyModel Redundancy { get; set; } = new RedundancyModel();
        /// <summary>
        /// 이중화 Primary 여부 Flag
        /// </summary>
        public bool IsPrimary
        {
            get => Redundancy.IsPrimary;
            set
            {
                if(Redundancy.IsPrimary != value)
                {
                    Redundancy.IsPrimary = value;
                }
            }
        }
        /// <summary>
        /// 이중화 Secondary 여부 Flag
        /// </summary>
        public bool IsSecondary
        {
            get => _isSecondary;
            set
            {
                if(_isSecondary != value)
                {
                    _isSecondary = value;
                    RaisePropertyChanged(nameof(IsSecondary));
                }
            }
        }
        private bool _isSecondary;
        #endregion
        /// <summary>
        /// 생성자
        /// </summary>
        public RedundancySettingUCViewModel()
        {
            BindData();
        }
        #region 초기값 등록
        /// <summary>
        /// Binding 되는 Variable 설정 함수
        /// </summary>
        /// <remarks>
        /// 화면에 보여지는 초기값을 설정하는 Function
        /// </remarks>
        private void BindData()
        {
            Redundancy = AppData.Instance.REDUNDANCY;
            IsSecondary = !Redundancy.IsPrimary;
        }
        #endregion

        #region IDataErrorInfo 인터페이스 Member
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    default: return string.Empty;
                }
            }
        }
        public string Error
        {
            get
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
