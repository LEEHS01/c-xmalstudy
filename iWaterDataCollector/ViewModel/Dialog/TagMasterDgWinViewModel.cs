using iWaterDataCollector.Global;
using System.ComponentModel;
using System.Linq;

/********************************************
 * TagManager Dialog Popup Page
 * Tag Manager 화면 ViewModel
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.ViewModel.Dialog
{
    public class TagMasterDgWinViewModel : ViewModelBase, IDataErrorInfo
    {
        /// <summary>
        /// 적용 버튼 활성화/비활성화 Flag
        /// </summary>
        /// <remarks>
        /// SCADA 서버 목록에 Remote Sever가 있는 경우 Remote Server Tag 목록 설정 화면이 보이도록 하는 Flag
        /// </remarks>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged(nameof(IsVisible));
                }
            }
        }
        private bool _isVisible;
        /// <summary>
        /// 생성자
        /// </summary>
        public TagMasterDgWinViewModel()
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
            //SCADA Server 리스트에 Remote Server가 있는지 확인)
            IsVisible = AppData.Instance.ServerCollection.Count(t => !t.IsLocal) > 0;
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
