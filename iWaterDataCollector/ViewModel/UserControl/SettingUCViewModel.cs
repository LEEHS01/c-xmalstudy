using System.ComponentModel;

/********************************************
 * Setting Page
 * 설정 화면 ViewModel
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.ViewModel.UserControl
{
    public class SettingUCViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Binding Data
        /// <summary>
        /// Selected Page Index
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if(_selectedIndex != value)
                {
                    _selectedIndex = value;
                    RaisePropertyChanged(nameof(SelectedIndex));
                }
            }
        }
        private int _selectedIndex;
        #endregion
        /// <summary>
        /// 생성자
        /// </summary>
        public SettingUCViewModel()
        {
            SelectedIndex = 0;
        }
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
