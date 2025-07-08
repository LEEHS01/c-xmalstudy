using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/********************************************
 * ViewModel Base Class
 ********************************************/
namespace iWaterDataCollector.ViewModel
{
    /// <summary>
    /// View(화면) DataContent Model
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 명령생성
        /// </summary>
        public IRelayCommand Command { get; set; }
        /// <summary>
        /// 화면 View 유무
        /// </summary>
        public bool IsView
        {
            get => _isView;
            set
            {
                if(_isView != value)
                {
                    _isView = value;
                    RaisePropertyChanged(nameof(IsView));
                }
            }
        }
        private bool _isView;

        #region INotifyPropertyChanged Member
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string callerName = null)
        {
            if (string.IsNullOrWhiteSpace(callerName))
            {
                return;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));
        }
        #endregion
    }
}
