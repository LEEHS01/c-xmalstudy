using iWaterDataCollector.Global;
using iWaterDataCollector.Global.Handler;
using iWaterDataCollector.Model.View;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

/********************************************
 * PDB Tag Setting Page 
 * Tag Data를 PDB에서 받아올 Tag를 설정하는 화면 ViewModel
 * Log Set Name → Program
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.ViewModel.UserControl
{
    public class PDBTagCheckerUCViewModel : ViewModelBase, IDataErrorInfo
    {
        #region User Variable
        /// <summary>
        /// Filter를 사용하기 위해 Wrapping할 CollectionList
        /// </summary>
        private ICollectionView _tagCollection { get; set; }

        private readonly string _myName;
        #endregion
        #region User CommandParameter
        /// <summary>
        /// List 초기화
        /// </summary>
        private const string COMMAND_INITIALIZE = "Initialize";
        /// <summary>
        /// 적용
        /// </summary>
        private const string COMMAND_COMMIT = "Commit";
        #endregion
        #region Binding Data
        /// <summary>
        /// Historian / PDB Tag List
        /// </summary>
        public ObservableCollection<TagModel> DisplayList
        {
            get
            {
                if (_displayList == null)
                {
                    _displayList = new ObservableCollection<TagModel>();
                }
                this._tagCollection = CollectionViewSource.GetDefaultView(_displayList);
                this._tagCollection.Filter = Filter;
                return _displayList;
            }
            set
            {

            }
        }
        private ObservableCollection<TagModel> _displayList;
        /// <summary>
        /// PDB Tag 설정 File Path
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    RaisePropertyChanged(nameof(FilePath));
                }
            }
        }
        private string _filePath;
        /// <summary>
        /// Tag 검색 String
        /// </summary>
        public string FilterString
        {
            get => _filterString;
            set
            {
                if (_filterString != value)
                {
                    _filterString = value;
                    RaisePropertyChanged(nameof(FilterString));
                    _tagCollection.Refresh();
                }
            }
        }
        private string _filterString;
        /// <summary>
        /// Historian Tag 기준일
        /// </summary>
        public string CreateDate
        {
            get => _createDate;
            set
            {
                if(_createDate != value)
                {
                    _createDate = value;
                    RaisePropertyChanged(nameof(CreateDate));
                }
            }
        }
        private string _createDate;
        #endregion
        /// <summary>
        /// 생성자
        /// </summary>
        public PDBTagCheckerUCViewModel()
        {
            _myName = GetType().Name;
            BindData();
            BindCommand();
        }
        #region 초기값등록
        /// <summary>
        /// Binding 되는 Variable 설정 함수
        /// </summary>
        /// <remarks>
        /// 화면에 보여지는 초기값을 설정하는 Function
        /// </remarks>
        private void BindData()
        {
            //DataGrid List Load
            ReflashList();
        }
        /// <summary>
        /// Tag List ReLoad
        /// </summary>
        private void ReflashList()
        {
            DisplayList.Clear();
            //Local SCADA Path 확인
            var localPath = DirectoryHandler.GetDefaultDirectory(AppData.Instance.DefaultPath, AppData.Instance.LocalNodeName);
            //Info 폴더 경로 확인
            var infoPath = DirectoryHandler.InfoDirectory(localPath);
            var ltag = FileHandler.ReadTagCollection(infoPath, AppData.Instance.PDBPath, out string date);
            AppData.Instance.MsgIRDC.Debug(AppData.AppLog, _myName, "Historian Info Tag File Load 완료");
            foreach (var item in ltag)
            {
                var tag = new TagModel(item.Name, item.Description, item.IsCheck);
                DisplayList.Add(tag);
            }
            //생성일자 등록
            CreateDate = date;
        }
        #endregion
        #region 명령등록
        /// <summary>
        /// 화면에서 Inboke될 Event Command 등록
        /// </summary>
        private void BindCommand()
        {
            Command = new RelayCommand<object>(Command_Handler);
        }
        /// <summary>
        /// 화면 버튼 움직임에 대한 Command Handler 함수
        /// </summary>
        /// <remarks>
        /// 화면에서 버튼을 클릭할 시 연결되는 Command Handler Function
        /// </remarks>
        /// <param name="obj">버튼 CommandName <see cref="string"/></param>
        private void Command_Handler(object obj)
        {
            if (obj == null)
                return;
            var command = obj.ToString();

            switch (command)
            {
                //DataGrid 초기화 --> 저장 이전 값으로 모두 Reload
                case COMMAND_INITIALIZE:
                    ReflashList();
                    break;
                //저장버튼 클릭
                case COMMAND_COMMIT:
                    Save();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// PDB 설정된 Historian Tag 리스트 파일 저장
        /// </summary>
        private void Save()
        {
            //체크박스가 선택된 리스트 추출
            var ltag = DisplayList.Where(t => t.IsSelected).Select(t => t.Name).ToArray();
            //기존 PDB Tag 설정 파일 백업
            FileHandler.MoveRecoveryCSVFile(AppData.Instance.PDBPath);
            //설정된 경로에 csv파일로 저장
            if (FileHandler.WriteCSVFile(AppData.Instance.PDBPath, ltag))
            {
                MessageBox.Show("pdb Tag 리스트가 저장되었습니다.", "파일저장", MessageBoxButton.OK, MessageBoxImage.Information);
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "pdb Tag 리스트가 저장되었습니다.");
            }
            else
            {
                MessageBox.Show("pdb Tag 리스트 저장을 실패하였습니다.", "파일저장", MessageBoxButton.OK, MessageBoxImage.Warning);
                AppData.Instance.MsgIRDC.Warn(AppData.AppLog, _myName, "pdb Tag 리스트 저장을 실패하였습니다.");
            } 
        }
        #endregion
        #region DataGrid Filter
        public bool Filter(object obj)
        {
            var data = obj as TagModel;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(FilterString))
                {
                    return data.Name.Contains(FilterString) || data.Description.Contains(FilterString);
                }
                return true;
            }
            return false;
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
