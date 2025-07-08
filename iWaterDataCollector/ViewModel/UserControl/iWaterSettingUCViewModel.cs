using iWaterDataCollector.Global;
using iWaterDataCollector.Global.Handler;
using iWaterDataCollector.Model.INI;
using iWaterDataCollector.Model.View;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

/********************************************
 * Setting Page
 * 설정 화면 ViewModel
 * Log Set Name → Program
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.ViewModel.UserControl
{
    public class iWaterSettingUCViewModel : ViewModelBase, IDataErrorInfo
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

        public IRelayCommand CellEditingCommand { get; set; }
        #endregion
        #region User CommandParameter
        /// <summary>
        /// DataGrid Row 추가
        /// </summary>
        private const string COMMAND_ROW_ADD = "Add";
        /// <summary>
        /// DataGrid Row 삭제
        /// </summary>
        private const string COMMAND_ROW_REMOVE = "Remove";
        #endregion
        #region Binding Data
        /// <summary>
        /// SCADA Server Dictionary
        /// </summary>
        /// <remarks>
        /// SCADA Server 설정 내역
        /// </remarks>
        public ObservableCollection<ServerModel> DisplayServers { get; set; } = new ObservableCollection<ServerModel>();
        /// <summary>
        /// Task 설정 
        /// </summary>
        /// <remarks>
        /// 자동시작, 파일보관기간, 동작 Interval등 설정값
        /// </remarks>
        public IWaterModel IWater { get; set; } = new IWaterModel();
        /// <summary>
        /// Log 파일 보관기간
        /// </summary>
        public int LogArchiveDuration
        {
            get => _logArchiveDuration;
            set
            {
                if(_logArchiveDuration != value)
                {
                    _logArchiveDuration = value;
                    RaisePropertyChanged(nameof(LogArchiveDuration));
                }
            }
        }
        private int _logArchiveDuration;

        /// <summary>
        /// 선택된 SCADA Server Index
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
        /// 선택된 SCADA Server Class
        /// </summary>
        public ServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (_selectedServer != value)
                {
                    _selectedServer = value;
                    RaisePropertyChanged(nameof(SelectedServer));
                }
            }
        }
        private ServerModel _selectedServer;
        /// <summary>
        /// 화면의 현재 Cell 확인용 Variable
        /// </summary>
        public DataGridCellInfo CellInfo
        {
            get => _cellInfo;
            set
            {
                if (_cellInfo != value)
                {
                    _cellInfo = value;
                    RaisePropertyChanged(nameof(CellInfo));
                }
            }
        }
        private DataGridCellInfo _cellInfo;
        #endregion
        #region User Variable
        /// <summary>
        /// 이전 DataGrid Index 확인용 
        /// </summary>
        private int _preIndex = -1;
        /// <summary>
        /// DataGrid 내용 Update 시 예외처리용 Error코드 정리 Enum
        /// </summary>
        private GRID_WARN _code = GRID_WARN.NORMAL;
        #endregion
        /// <summary>
        /// 생성자
        /// </summary>
        public iWaterSettingUCViewModel()
        {
            BindData();
            BindCommand();
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
            DisplayServers.Clear();
            IWater = AppData.Instance.IWATER;
            LogArchiveDuration = AppData.Instance.LogArchiveDuration;
            DisplayServers = AppData.Instance.SettingServerCollection;
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
            CellEditingCommand = new RelayCommand<object>(CellEditingCommand_Handler);
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
            //예외사항 확인
            if (_preIndex < 0)
                return;
            //Grid 상태가 정상일 때만 동작하도록 예외
            if (_code == GRID_WARN.NORMAL)
                return;

            var args = obj as SelectionChangedEventArgs;
            var dg = args.Source as DataGrid;
            var item = dg.Items[_preIndex] as ServerModel;


            if (dg.SelectedIndex < 0)
            {
                if (dg.SelectedIndex != _preIndex)
                {
                    dg.CurrentItem = item;
                    dg.SelectedIndex = _preIndex;
                    if(_code == GRID_WARN.UNIQUE_CHECK)
                    {
                        _code = GRID_WARN.NORMAL;
                    }
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
            if (SelectedIndex >= 0)
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
            //Column별로 예외처리 방식이 다르기에 Switch문 사용
            switch (args.Column.DisplayIndex)
            {
                //Local SCADA CheckBox Column
                case 0:
                    #region Local SCADA 선택시 예외처리
                    var chkB = args.EditingElement as CheckBox;

                    if ((bool)chkB.IsChecked)
                    {
                        //변경된 Row Index를 제외한 나머지 List의 Local유무 Flag를 해제
                        DisplayServers.Where((_, i) => i != args.Row.GetIndex()).ToList().ForEach(t => t.IsLocal = false);
                    }
                    else
                    {
                        //변경된 Row 이외에 Local Flag가 하나도 없는경우
                        var cCnt = DisplayServers.Where((_, i) => i != args.Row.GetIndex()).Count(t => t.IsLocal);
                        if (cCnt <= 0)
                        {
                            //Message Box 표출
                            MessageBox.Show("최소 1개의 Local 서버정보가 필요합니다.");
                            chkB.IsChecked = true;
                            _preIndex = SelectedIndex;
                            _code = GRID_WARN.UNIQUE_CHECK;
                            break;
                        }
                    }
                    _code = GRID_WARN.NORMAL;
                    #endregion
                    break;
                //NodeName Column
                case 1:
                    var txtB = args.EditingElement as TextBox;
                    var nCnt = DisplayServers.Where((_, i) => i != args.Row.GetIndex()).Count(t => t.NodeName.Equals(txtB.Text));

                    if (string.IsNullOrEmpty(txtB.Text))
                    {
                        MessageBox.Show("Node Name은 Null 일 수 없습니다.");
                        args.Cancel = true;
                        _preIndex = SelectedIndex;
                        _code = GRID_WARN.NULL;
                        break;
                    }

                    if (nCnt > 0)
                    {
                        MessageBox.Show("이미 등록된 Node Name 입니다.");
                        args.Cancel = true;
                        _preIndex = SelectedIndex;
                        _code = GRID_WARN.UNIQUE;
                        break;
                    }
                    _code = GRID_WARN.NORMAL;
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// Cell Editing Event Handler
        /// </summary>
        /// <param name="obj"></param>
        private void CellEditingCommand_Handler(object obj)
        {
            //Cell 수정 종료에 대한 EventArgument
            var args = obj as DataGridPreparingCellForEditEventArgs;//PreparingCellForEdit;
            if(args.Column.DisplayIndex == 0)
            {
                #region Local SCADA 선택시 예외처리
                var chkB = args.EditingElement as CheckBox;

                if ((bool)chkB.IsChecked)
                {
                    //변경된 Row Index를 제외한 나머지 List의 Local유무 Flag를 해제
                    DisplayServers.Where((_, i) => i != args.Row.GetIndex()).ToList().ForEach(t => t.IsLocal = false);
                }
                else
                {
                    //변경된 Row 이외에 Local Flag가 하나도 없는경우
                    var cCnt = DisplayServers.Where((_, i) => i != args.Row.GetIndex()).Count(t => t.IsLocal);
                    if (cCnt <= 0)
                    {
                        //Message Box 표출
                        MessageBox.Show("최소 1개의 Local 서버정보가 필요합니다.");
                        chkB.IsChecked = true;
                        _preIndex = SelectedIndex;
                        _code = GRID_WARN.UNIQUE_CHECK;
                    }
                }
                _code = GRID_WARN.NORMAL;
                #endregion
            }

        }
        /// <summary>
        /// 화면 버튼 움직임에 대한 Command Handler 함수
        /// </summary>
        /// <remarks>
        /// 화면에서 버튼을 클릭할 시 연결되는 Command Handler Function
        /// </remarks>
        /// <param name="obj">버튼 CommandName</param>
        private void Command_Handler(object obj)
        {
            if (obj == null)
                return;
            var command = obj.ToString();

            switch (command)
            {
                case COMMAND_ROW_ADD:
                    try
                    {
                        DisplayServers.Add(new ServerModel());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("추가가 동작되지 않았습니다. 다시 시도해 주세요.");
                    }
                    break;
                case COMMAND_ROW_REMOVE:
                    try
                    {
                        if (SelectedServer.IsLocal)
                        {
                            MessageBox.Show("Local Sever는 삭제할 수 없습니다.");
                            return;
                        }
                        _preIndex = -1;
                        DisplayServers.RemoveAt(SelectedIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("삭제가 동작되지 않았습니다. 다시 시도해 주세요.");
                    }
                    break;
                default:
                    if (DirectoryHandler.GetFileDirectory(SelectedServer.Directory, out string path))
                    {
                        SelectedServer.Directory = path;
                    }
                    break;
            }
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
