using iWaterDataCollector.Global;
using iWaterDataCollector.Global.Handler;
using iWaterDataCollector.Model.View;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

/********************************************
 * Remote Server Tag Setting Page
 * Remote Server Tag 설정 화면 ViewModel
 * Log Set Name → Program
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.ViewModel.UserControl
{
    public class RemoteTagRegisterUCViewModel : ViewModelBase, IDataErrorInfo
    {
        #region 화면 Event Inboke 시 사용되는 RelayCommand
        /// <summary>
        /// DataGrid Cell 수정 시 발생
        /// </summary>
        public IRelayCommand CellEditCommand { get; set; }
        /// <summary>
        /// DataGrid 현재 Cell 변경시 발생
        /// </summary>
        public IRelayCommand CurrentCellChangedCommand { get; set; }
        /// <summary>
        /// DataGrid Cell 선택 변경시 발생
        /// </summary>
        public IRelayCommand SelectionChangedCommand { get; set; }
        #endregion
        #region User Variable
        /// <summary>
        /// Filter를 사용하기 위해 Wrapping할 CollectionList
        /// </summary>
        private ICollectionView _tagCollection { get; set; }
        /// <summary>
        /// 이전 DataGrid Index 확인용 
        /// </summary>
        private int _preIndex = -1;
        /// <summary>
        /// DataGrid 내용 Update 시 예외처리용 Error코드 정리 Enum
        /// </summary>
        private GRID_WARN _code = GRID_WARN.NORMAL;
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
        /// <summary>
        /// DataGrid Row 추가
        /// </summary>
        private const string COMMAND_ROW_ADD = "Add";
        /// <summary>
        /// DataGrid Row 삭제
        /// </summary>
        private const string COMMAND_ROW_REMOVE = "Remove";
        #endregion
        #region Context Menu
        /// <summary>
        /// DataGrid Context Menu
        /// </summary>
        public ContextMenu Menu { get; set; } = new ContextMenu();
        #endregion
        #region Binding Data
        /// <summary>
        /// Remote Server List
        /// </summary>
        public ObservableCollection<ServerModel> DisplayServer
        {
            get
            {
                if (_displayServer == null)
                {
                    _displayServer = new ObservableCollection<ServerModel>();
                }
                return _displayServer;
            }
            set
            {

            }
        }
        private ObservableCollection<ServerModel> _displayServer;
        /// <summary>
        /// Remote Tag List
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
                _displayList = value;
            }
        }
        private ObservableCollection<TagModel> _displayList;
        /// <summary>
        /// Selected Remote Server
        /// </summary>
        public ServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (_selectedServer != value)
                {
                    _selectedServer = value;
                    ReflashList();
                    RaisePropertyChanged(nameof(SelectedServer));
                }
            }
        }
        private ServerModel _selectedServer;
        /// <summary>
        /// 적용 버튼 활성화/비활성화 Flag
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if(_isEnabled != value)
                {
                    _isEnabled = value;
                    RaisePropertyChanged(nameof(IsEnabled));
                }
            }
        }
        private bool _isEnabled;

        /// <summary>
        /// Remote Tag File Path
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
        /// Tag Selected Index
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    RaisePropertyChanged(nameof(SelectedIndex));
                }
            }
        }
        private int _selectedIndex;
        /// <summary>
        /// Selected Server Index
        /// </summary>
        public int SelectedServerIndex
        {
            get => _selectedServerIndex;
            set
            {
                if (_selectedServerIndex != value)
                {
                    _selectedServerIndex = value;
                    RaisePropertyChanged(nameof(SelectedServerIndex));
                }
            }
        }
        private int _selectedServerIndex;
        #endregion
        /// <summary>
        /// 생성자
        /// </summary>
        public RemoteTagRegisterUCViewModel()
        {
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
            //Load Server List
            LoadServer();
        }
        /// <summary>
        /// Remote Server List Load
        /// </summary>
        private void LoadServer()
        {
            var lremote = AppData.Instance.ServerCollection.Where(t => !t.IsLocal).ToList();
            if (lremote.Count <= 0)
            {
                IsEnabled = false;
                return;
            }
            else
            {
                foreach (var item in lremote)
                {
                    DisplayServer.Add(item);
                }

                SelectedServer = DisplayServer.First();
                SelectedServerIndex = 0;
                IsEnabled = true;
            }
        }
        /// <summary>
        /// Remote Server Tag List ReLoad
        /// </summary>
        private void ReflashList()
        {
            //DataGrid 초기화
            if(DisplayList.Count > 0)
                DisplayList.Clear();
            //선택된 Server의 Tag List Load
            var ltag = FileHandler.ReadTagCollection(SelectedServer.Directory);
            foreach (var item in ltag)
            {
                var tag = new TagModel(item.Name, item.Description);
                DisplayList.Add(tag);
            }
        }
        #endregion
        #region 명령등록
        /// <summary>
        /// 화면에서 Inboke될 Event Command 등록
        /// </summary>
        private void BindCommand()
        {
            Command = new RelayCommand<object>(Command_Handler);
            CellEditCommand = new RelayCommand<object>(CellEditCommand_Handler);
            CurrentCellChangedCommand = new RelayCommand<object>(CurrentCellChangedCommand_Handler);
            SelectionChangedCommand = new RelayCommand<object>(SelectionChangedCommand_Handler);
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
                //저장버튼클릭
                case COMMAND_COMMIT:
                    if (ValueExecption())
                    {
                        Save();
                    }
                    break;
                //Row 추가
                case COMMAND_ROW_ADD:
                    try
                    {
                        DisplayList.Add(new TagModel(string.Empty, string.Empty));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Tag 추가가 동작되지 않았습니다. 다시 시도해 주세요.");
                    }
                    break;
                //Row 삭제
                case COMMAND_ROW_REMOVE:
                    try
                    {
                        DisplayList.RemoveAt(SelectedIndex);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Tag 삭제가 동작되지 않았습니다. 다시 시도해 주세요.");
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Remote Server Tag 리스트 파일 저장
        /// </summary>
        private void Save()
        {
            if (string.IsNullOrEmpty(SelectedServer.Directory))
            {
                MessageBox.Show("원격지 Tag List 파일 경로가 없습니다.\n설정→iWater 서버설정에서 등록해 주세요.");
                return;
            }
            //쉼표(,)를 구분자로 하여 List<string>생성
            var ltag = DisplayList.Select(t => string.Join(",", t.Name, t.Description)).ToArray();
            //기존파일 백업
            FileHandler.MoveRecoveryCSVFile(SelectedServer.Directory);
            if (FileHandler.WriteCSVFile(SelectedServer.Directory, ltag))
            {
                MessageBox.Show("Remote Server Tag 리스트가 저장되었습니다.", "파일저장", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Remote Server Tag 리스트 저장을 실패하였습니다.", "파일저장", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// 이중 예외처리
        /// </summary>
        /// <remarks>
        /// 저장 버튼 클릭 시 List 내 Data 한 번 더 확인하기 위한 Function
        /// </remarks>
        /// <returns>예외발견 여부 <see cref="bool"/>
        /// <para>
        /// true : 예외없음
        /// </para>
        /// <para>
        /// false : 예외발생
        /// </para>
        /// </returns>
        private bool ValueExecption()
        {
            var msg = string.Empty;
            //Tag명이 Null & Empty인지 확인
            var idx = _displayList.ToList().FindIndex(t => string.IsNullOrEmpty(t.Name));
            if (idx >= 0)
            {
                MessageBox.Show($"Tag Name은 Null 일 수 없습니다. Row Num : {idx + 1}");
                return false;
            }
            //리스트 내에 같은 이름을 가진 내용이 있는지 확인
            var dic = _displayList.Select(t => t.Name).GroupBy(x => x).Where(g => g.Count() > 1).ToDictionary(x => x.Key, x => x.Count());
            if (dic.Count > 0)
            {
                var tagName = string.Join("\n\r", dic.Keys);
                MessageBox.Show($"중복된 Tag가 존재합니다.\n\r[Tag Name]\n\r{tagName}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// DataGrid Selection Changed Event Handler
        /// </summary>
        /// <remarks>
        /// Cell Editing 중 예외처리 부분을 모두 처리하지 않고 다른 Row(또는 Cell)로 넘어가려 할 시 RowSelection이 바뀌지 않도록 하기 위해 추가
        /// </remarks>
        /// <param name="obj">Event Class <see cref="SelectionChangedEventArgs" /></param>
        private void SelectionChangedCommand_Handler(object obj)
        {
            if (_preIndex < 0)
                return;
            if (_code == GRID_WARN.NORMAL)
                return;

            var args = obj as SelectionChangedEventArgs;
            var dg = args.Source as DataGrid;

            if (dg.Items.Count <= 0)
                return;

            var item = dg.Items[_preIndex] as ServerModel;


            if (dg.SelectedIndex < 0)
            {
                if (dg.SelectedIndex != _preIndex)
                {
                    dg.CurrentItem = item;
                    dg.SelectedIndex = _preIndex;
                }
            }
            else
            {
                if (dg.SelectedIndex != _preIndex)
                {
                    //DataGrid의 Selection 옵션을 없앰
                    dg.UnselectAllCells();
                }
            }
        }
        /// <summary>
        /// Current Cell Changed Event Handler
        /// </summary>
        /// <param name="obj"></param>
        private void CurrentCellChangedCommand_Handler(object obj)
        {
            //선택된 현재 Cell의 RowIndex를 이전 Index로 등록
            if (SelectedIndex > 0)
                _preIndex = SelectedIndex;
        }
        /// <summary>
        /// Cell Edit Event Handler
        /// </summary>
        /// <param name="obj">Event Variable <see cref="DataGridCellEditEndingEventArgs"/> </param>
        private void CellEditCommand_Handler(object obj)
        {
            //Cell 수정 종료에 대한 EventArgument
            var args = obj as DataGridCellEditEndingEventArgs;
            //TagNsme에 대해서만 예외처리 진행
            if (args.Column.DisplayIndex > 0)
                return;

            var txtB = args.EditingElement as TextBox;
            if (string.IsNullOrEmpty(txtB.Text))
            {
                MessageBox.Show("Tag Name은 Null 일 수 없습니다.");
                _preIndex = SelectedIndex;
                _code = GRID_WARN.NULL;
                return;
            }

            var nCnt = _displayList.Where((_, i) => i != args.Row.GetIndex()).Count(t => t.Name.Equals(txtB.Text));
            if (nCnt > 0)
            {
                MessageBox.Show("이미 등록된 Tag 입니다.");
                _preIndex = SelectedIndex;
                _code = GRID_WARN.UNIQUE;
                return;
            }
            _code = GRID_WARN.NORMAL;
        }
        #endregion
        #region Menu Initilize
        /// <summary>
        /// DataGrid ContextMenu 초기화
        /// </summary>
        public void InitContextMenu()
        {
            // 첫번째 메뉴
            var AddTag = new MenuItem
            {
                Header = "Add Tag",
                Command = Command,
                CommandParameter = COMMAND_ROW_ADD
            };
            var RemoveTag = new MenuItem
            {
                Header = "Remove Tag",
                Command = Command,
                CommandParameter = COMMAND_ROW_REMOVE
            };
            Menu.Items.Add(AddTag);
            Menu.Items.Add(RemoveTag);

            FilteringAction(true);
        }
        //사용여부가 확인되지 않음 임시 주석 2023.11.20
        //private void RemoveTag_Click(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //private void AddTag_Click(object sender, EventArgs e)
        //{
        //    Command_Handler(COMMAND_ROW_ADD);
        //}
        #endregion

        #region DataGrid Filter
        /// <summary>
        /// DataGrid Filter
        /// </summary>
        /// <remarks>
        /// Filter가 적용된 내용만 DataGrid에 표출됨
        /// </remarks>
        /// <param name="obj">검색어 <see cref="string"/></param>
        /// <returns>Filter 적용 여부</returns>
        public bool Filter(object obj)
        {
            var data = obj as TagModel;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(_filterString))
                {
                    return data.Name.Contains(_filterString) || data.Description.Contains(_filterString);
                }
                return true;
            }
            return false;
        }
        #endregion
        #region Filter TextBox 돋보기 Event & Delegate
        public event Action<bool> OnFilter;
        private void FilteringAction(bool isVisible)
        {
            this.OnFilter?.Invoke(isVisible);
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
