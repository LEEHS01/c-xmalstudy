using iWaterDataCollector.Global;
using iWaterDataCollector.Global.Services;
using iWaterDataCollector.iFixAdapter;
using iWaterDataCollector.iFixAdapter.Common;
using iWaterDataCollector.Net;
using iWaterDataCollector.View.Dialog;
using iWaterDataCollector.ViewModel.Dialog;
using iWaterDataCollector.ViewModel.UserControl;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

/********************************************
 * Main Page View Model
 * Log Set Name → Program
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.ViewModel
{
    public class MainWinViewModel : ViewModelBase, IDataErrorInfo
    {
        #region User CommandParameter
        /// <summary>
        /// Task Action CommandParameter
        /// </summary>
        private const string COMMAND_ACTION = "Action";
        /// <summary>
        /// Task Stop CommandParameter
        /// </summary>
        private const string COMMAND_STOP = "Stop";
        /// <summary>
        /// Setting Display CommandParameter
        /// </summary>
        private const string COMMAND_SETTING = "Setting";
        /// <summary>
        /// Home Page Display CommandParameter
        /// </summary>
        private const string COMMAND_BACK = "Back";
        /// <summary>
        /// For Tag Editing Popup Display CommandParameter
        /// </summary>
        private const string COMMAND_TAG_MANAGER = "Tag";
        #endregion
        #region TrayIcon 관련 Context Menu
        /// <summary>
        /// Tray Icon ContextMenu
        /// </summary>
        /// <remarks>
        /// 프로세스 Task 시작/종료 및 프로그램 종료에 대한 TrayIcon 용 ContextMenu
        /// </remarks>
        public ContextMenuStrip Menu { get; set; } = new ContextMenuStrip();
        /// <summary>
        /// 프로그램 종료 여부
        /// </summary>
        /// <remarks>
        /// TrayIcon의 ContextMenu를 통해서만 종료되도록 하기 위해 사용하는 Flag
        /// </remarks>
        public bool SystemShutdown { get; set; }
        #endregion
        #region User Variable 
        /// <summary>
        /// iWater Server 정보 Dictionary <see cref="Dictionary{TKey, TValue}" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// [Key] iWater Node Name
        /// </para>
        /// <para>
        /// [Value] <see cref="Server" /> Class
        /// </para>
        /// </remarks>
        private Dictionary<string, Server> _dicServers;
        /// <summary>
        /// Kafka 연계 Task Class 
        /// </summary>
        /// <remarks>
        /// Kafka Consumer 연결 Class
        /// </remarks>
        private Kafka2PDB kafka;
        /// <summary>
        /// 로그파일 삭제 Task Class
        /// </summary>
        private LogCleanerService logCleaner;

        private readonly string _myName;
        #endregion
        #region Binding Data
        /// <summary>
        /// View List - 화면전환용
        /// </summary>
        /// <remarks>
        /// Main ↔ Setting Page 전환
        /// </remarks>
        public ObservableCollection<ViewModelBase> Pages { get; } = new ObservableCollection<ViewModelBase>();
        /// <summary>
        /// 설정창 닫기(Arrow) Button Visible
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if(_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged(nameof(IsVisible));
                }
            }
        }
        private bool _isVisible;
        /// <summary>
        ///  Dialog Popup Display 시 전체 화면 Disable
        /// </summary>
        public bool OnDisable
        {
            get => _onDisable;
            set
            {
                if (_onDisable != value)
                {
                    _onDisable = value;
                    RaisePropertyChanged(nameof(OnDisable));
                }
            }
        }
        private bool _onDisable;
        /// <summary>
        /// 기능(Task) 동작 여부 Flag
        /// </summary>
        public bool IsAction
        {
            get => _isAction;
            set
            {
                if (_isAction != value)
                {
                    _isAction = value;
                    RaisePropertyChanged(nameof(IsAction));
                }
            }
        }
        private bool _isAction = false;
        /// <summary>
        /// 이중화(Redundancy) 사용 Flag
        /// </summary>
        public bool IsRedundancy
        {
            get => _isRedundancy;
            set
            {
                if(_isRedundancy != value)
                {
                    _isRedundancy = value;
                    RaisePropertyChanged(nameof(IsRedundancy));
                }
            }
        }
        private bool _isRedundancy;
        /// <summary>
        /// 이중화(Redundancy) 상태(None / Active / Secondary)
        /// </summary>
        public string State
        {
            get => _state;
            set
            {
                if(_state != value)
                {
                    _state = value;
                    RaisePropertyChanged(nameof(State));
                }
            }
        }
        private string _state;
        #endregion
        /// <summary>
        /// 생성자
        /// </summary>
        public MainWinViewModel()
        {
            AppData.Instance.Initialize(Properties.Resources.Setting);
            AppData.Instance.InitializeLog(Properties.Resources.Log);
            _myName = GetType().Name;
            BindCommand();
            BindViewModel();
            BindData();
            InitContextMenu();
            StartLogCleaner();

            AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "IRDC 프로그램 시작");
        }
        #region 명령등록
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
        /// <param name="obj">버튼 CommandName</param>
        private void Command_Handler(object obj)
        {
            if (obj == null)
                return;
            var command = obj.ToString();

            switch (command)
            {
                case COMMAND_ACTION:
                    IsAction = LaunchiWater();
                    if (IsAction)
                    {
                        (Pages[0] as HomeUCViewModel).IsStart = IsAction;
                        //Kafka 서버 Consumer 시작 Task
                        LaunchKafka();
                        //이중화 시작 Task
                        LaunchRedundancy();
                        //화면 변경 함수
                        ChangedState(IsAction);
                        AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "iWater Data 수집 프로세스 시작");
                    }
                    else
                    {
                        AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "iWater Data 수집 프로세스 시작 실패");
                    }
                    break;
                case COMMAND_STOP:
                    IsAction = false;
                    (Pages[0] as HomeUCViewModel).IsStart = IsAction;
                    //iWater Historian Sever 연결 해제
                    EndHistorian();
                    //Consumer 연결 해제
                    EndKafka();
                    //이중화 연결 해제
                    EndRedundacy();
                    //화면 변경 함수
                    ChangedState(IsAction);
                    AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "iWater Data 수집 프로세스 중지");
                    break;
                case COMMAND_TAG_MANAGER:
                    //Tag Manager 화면 Load
                    LoadTagMaster();
                    break;
                case COMMAND_SETTING:
                    IsVisible = !IsVisible;
                    //설정화면으로 전환
                    ChangedPage();
                    break;
                case COMMAND_BACK:
                    //메인화면으로 전환
                    EndSetting();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Historian Server 시작
        /// </summary>
        /// <remarks>
        /// iWater Historian 연결 함수
        /// </remarks>
        /// <returns>
        /// iWater Historian 연결 성공여부 for <see cref="bool" />.
        /// </returns>
        private bool LaunchiWater()
        {
            //연결 성공여부 Return 값
            var retVal = false;
            try
            {
                foreach (var item in AppData.Instance.ServerCollection)
                {
                    //Local SCADA 여부 확인
                    if (item.IsLocal)
                    {
                        //Local SCADA 설정 및 Dictionary 추가
                        LocalServer local = new LocalServer(item.NodeName);
                        _dicServers.Add(item.NodeName, local);
                    }
                    else
                    {
                        //Node 이름이 null인 경우 예외처리
                        if (string.IsNullOrEmpty(item.NodeName))
                        {
                            AppData.Instance.MsgIRDC.Warn(AppData.AppLog, _myName, $"iWater Sever [{item.NodeName}]이 없습니다.");
                            continue;
                        }
                        //이미 등록된 NodeName인 경우 예외처리
                        if (_dicServers.ContainsKey(item.NodeName))
                        {
                            AppData.Instance.MsgIRDC.Warn(AppData.AppLog, _myName, $"iWater Sever {item.NodeName}는 이미 등록된 Server 입니다.");
                            continue;
                        }
                        //Remote SCADA 설정 및 Dictionary 추가
                        RemoteServer remote = new RemoteServer(item.NodeName, item.Directory);
                        _dicServers.Add(item.NodeName, remote);
                    }
                    _dicServers[item.NodeName].ChangedState += Server_ChangedState;
                    _dicServers[item.NodeName].OnLoad += Server_OnLoad;
                    AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, $"{item.NodeName} iWater 시작");
                }
                //연결 성공
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                MessageBox.Show("iWater 실행을 하지 못하였습니다. Error Log를 확인하여 주세요.", "실행오류", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "iWater Historian 연결 오류", ex);
                _dicServers.Clear();
            }
            return retVal;
        }
        /// <summary>
        /// Content 화면 전환
        /// </summary>
        /// <remarks>
        /// 메인화면 ↔ 설정화면간 화면전환 함수
        /// </remarks>
        private void ChangedPage()
        {
            Pages[0].IsView = !IsVisible;
            Pages[1].IsView = IsVisible;
        }
        /// <summary>
        /// Tag Manager Load
        /// </summary>
        /// <remarks>
        /// 태그 설정 화면 Load 함수
        /// </remarks>
        private void LoadTagMaster()
        {
            //TagManager 선언
            var popWin = new TagMasterDgWinView();
            //ViewModel 선언
            var vm = new TagMasterDgWinViewModel();
            //Tagmanager 화면에 ViewModel 연결
            popWin.DataContext = vm;
            //Popup Show
            popWin.ShowDialog();
        }
        /// <summary>
        /// 각 iWater Server State Change Event
        /// </summary>
        /// <param name="name">SCADA Node Name</param>
        /// <param name="state">Connection State</param>
        /// <remarks>
        /// Historian 연결 상태가 변경 시 Event Collback
        /// </remarks>
        private void Server_ChangedState(string name, bool state)
        {
            AppData.Instance.ServerCollection.FirstOrDefault(t => t.NodeName.Equals(name)).IsState = state;
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, $"{name} iWater 상태 [{state}]");
        }
        /// <summary>
        /// 각 iWater Server Tag Load Event
        /// </summary>
        /// <param name="name">SCADA Node Name</param>
        /// <param name="dt">Tag Load Start DateTime</param>
        /// <param name="ts">Tag Load Working TimeSpan</param>
        private void Server_OnLoad(string name, DateTime dt, TimeSpan ts)
        {
            //Task 동작중 UserControl ViewModel Call
            var vm = Pages[0] as HomeUCViewModel;
            try
            {
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, $"{name} iWater Historian Tag Value Load [{ts}]");
                //화면의 시작 시간 및 작업시간 업데이트
                vm.UpdateTimeline(name, dt, ts, DateTime.Now);

                #region 최대 동작 시간 Update
                var server = AppData.Instance.ServerCollection.FirstOrDefault(t => t.NodeName == name);
                if (server != null) 
                {
                    server.RunningTime = ts;
                    if (server.MaxRunningTime < ts)
                    {
                        server.MaxRunningTime = ts;
                    }
                }
                #endregion

                #region Local Server 기준으로 다음 수행시간을 계산함            
                if (AppData.Instance.LocalNodeName.Equals(name))
                {
                    vm.NextTime = dt.AddMinutes(AppData.Instance.StorageIntervalD);
                    vm.ElapsedSecond = 0;
                }
                #endregion
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, $"{name} iWater Historian Tag Value Load Event 처리 오류");
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, $"{name} iWater Historian Tag Value Load Event 처리 오류", ex);
            }        
        }
        /// <summary>
        /// Kafka Confume 시작
        /// </summary>
        private void LaunchKafka()
        {
            //Kafak Topic 설정
            kafka = new Kafka2PDB(AppData.Instance.Topic.Split(','))
            {
                //Kafak로 들어오는 Tag Filtering 여부
                UseFilter = AppData.Instance.UseFilter,
                //PDB Node
                Node = AppData.Instance.LocalNodeName,
                //Kafka Server 정보(IP/Port)
                EndPoint = AppData.Instance.KafkaEndPoint,
                //Consumer Group ID
                Group_id = AppData.Instance.GroupID,
                //kafka Tag 설정 파일 Path
                KafkaPath = AppData.Instance.KafkaPath,
                //Kafka Tag PDB Set Interval
                SetInterval = AppData.Instance.PDBSetInterval,
                //이중화 Active 여부
                IsActive = AppData.Instance.IsRedundancy ? State.Equals(Global.REDUNDANCY_STATE.Active.ToString()) : true
            };
            //Kafka Consumer 시작
            kafka.Launch();
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, $"Kafka 시작[topic : {AppData.Instance.Topic}]");
        }
        /// <summary>
        /// 이중화 서버 시작
        /// </summary>
        private void LaunchRedundancy()
        {
            //이중화 상태 아닌 경우
            if (!AppData.Instance.IsRedundancy)
            {
                //상태 String Alone
                State = REDUNDANCY_STATE.Alone.ToString();
            }
            else
            {
                //Primary 서버 시작
                PrimarySock.Instance.OnChangeConnected += Primary_OnChangeConnected;
                //Secondary 서버 시작
                SecondarySock.Instance.OnChangeConnected += Secondary_OnChangeConnected;
                //Primary 서버 여부 확인
                if (AppData.Instance.IsPrimary)
                {
                    PrimarySock.Instance.Start(AppData.Instance.RedundancyPort);
                }
                else
                {
                    SecondarySock.Instance.Start(AppData.Instance.RedundancyIP, AppData.Instance.RedundancyPort);
                }
            }
        }
        /// <summary>
        /// Historian Server 종료
        /// </summary>
        private void EndHistorian()
        {
            foreach (var scada in _dicServers)
            {
                //iWater Server(scada) Class 내 실행 중인 연결 Task 종료
                scada.Value.EndTask();
                //iWater Server(scada) Class Event 해제
                scada.Value.ChangedState -= Server_ChangedState;
                scada.Value.OnLoad -= Server_OnLoad;
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, $"{scada.Key} iWater Historian 연결 종료");
            }
            //server 정보 Clear
            _dicServers.Clear();
            IsAction = false;
        }
        /// <summary>
        /// Kafka Confume 종료
        /// </summary>
        private void EndKafka()
        {
            //kafak Consumer 연결 해제
            kafka.Disconnect();
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "Kafka Consumer 연결 종료");
        }

        /// <summary>
        /// 이중화 서버 종료
        /// </summary>
        private void EndRedundacy()
        {
            //Primary Server 연결 중지
            PrimarySock.Instance.Stop();
            //Secondary Server 연결 중지
            SecondarySock.Instance.Stop();
            //이중화 Event 해제
            PrimarySock.Instance.OnChangeConnected -= Primary_OnChangeConnected;
            SecondarySock.Instance.OnChangeConnected -= Secondary_OnChangeConnected;
            //이중화 상태 없음(None)으로 변경
            State = REDUNDANCY_STATE.None.ToString();
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "이중화 연결 종료");
        }
        /// <summary>
        /// 설정 화면 종료
        /// </summary>
        /// <remarks>
        /// 설정 화면 전환 시 설정정보 저장 여부 확인
        /// </remarks>
        private void EndSetting()
        {
            //NullValue 확인
            var code = AppData.Instance.ValueException();
            switch (code)
            {
                case SETTING_WARN.NODE_NULL:
                    MessageBox.Show("Node Name은 Null일 수 없습니다.");
                    break;
                default:
                    //설정정보를 저장할지 선택
                    if (MessageBox.Show("설정을 저장하고 마치시겠습니까?", "설정저장", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        AppData.Instance.SettingSave();
                        Log.Instance.UpdateIWaterNode(Properties.Resources.Log);
                        AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "설정저장");
                    }
                    else
                    {
                        AppData.Instance.SettingLoad();
                        AppData.Instance.RollbackServerSetting();
                    }

                    IsVisible = !IsVisible;
                    ChangedPage();
                    break;
            }
        }
        #endregion

        #region Pages 등록
        /// <summary>
        /// 메인 창의 Page 등록
        /// </summary>
        /// <remarks>
        /// 메인화면에 나타날 Page를 등록. (Home ↔ Setting)
        /// </remarks>
        private void BindViewModel()
        {
            Pages.Add(new HomeUCViewModel());
            Pages.Add(new SettingUCViewModel());

            Pages[0].IsView = true;
        }
        #endregion

        #region 초기값 등록
        private void BindData()
        {
            _dicServers = new Dictionary<string, Server>();
            State = REDUNDANCY_STATE.None.ToString();
        }
        #endregion

        #region 이중화 Event Collback
        /// <summary>
        /// Primary 네트워크 Event Collback
        /// </summary>
        /// <remarks>
        /// Primary 네트워크에서 Secondary와 연결을 성공했는지 여부를 확인하는 Collback
        /// </remarks>
        /// <param name="isConnect">연결성공여부</param>
        private void Primary_OnChangeConnected(bool isConnect)
        {
            State = REDUNDANCY_STATE.Active.ToString();
            //Kafka 동작 이중화 사용여부 확인
            if (AppData.Instance.IsRedundancy)
            {
                //State = isConnect ? REDUNDANCY_STATE.Active.ToString() : REDUNDANCY_STATE.Active.ToString();
                //Kafka Consumer가 살아있는 경우
                if (kafka != null)
                {
                    //Kafka 이중화 Flag 변경
                    kafka.IsActive = true;
                }
            }
        }
        /// <summary>
        /// Secondary 네트워크 Event Collback
        /// </summary>
        /// <remarks>
        /// Secondary 네트워크에서 Primary와 연결을 성공했는지 여부를 확인하는 Collback
        /// </remarks>
        /// <param name="isConnect">연결성공여부</param>
        private void Secondary_OnChangeConnected(bool isConnect)
        {
            //Kafka 동작 이중화 사용여부 확인
            if (AppData.Instance.IsRedundancy)
            {
                //Secondary - Primary 연결 성공한경우 Secondary 네트워크 상태 StandBy로 변경
                State = isConnect ? REDUNDANCY_STATE.StandBy.ToString() : REDUNDANCY_STATE.Active.ToString();
                AppData.Instance.MsgIRDC.Debug(AppData.AppLog, _myName, $"이중화 네트워크 상태 변경 [{State}]");
                //Kafka Consumer가 살아있는 경우
                if (kafka != null)
                {
                    var isActive = State.Equals(REDUNDANCY_STATE.Active.ToString());
                    if (isActive != kafka.IsActive)
                    {
                        kafka.IsActive = isActive;
                        AppData.Instance.MsgIRDC.Debug(AppData.AppLog, _myName, $"Kafka 이중화 Flag 변경 : [{kafka.IsActive}]");
                    }
                }
            }
        }
        #endregion

        #region 보관기간이 지난 Log 삭제 [20250627]
        private void StartLogCleaner()
        {
            logCleaner = new LogCleanerService(AppData.Instance.LOG.Path, AppData.Instance.LogArchiveDuration, TimeSpan.FromDays(1));
            logCleaner.Start();
        }
        #endregion

        /// <summary>
        /// 자동시작
        /// </summary>
        /// <remarks>
        /// 자동시작 설정값을 확인하여 Action Command Call
        /// </remarks>
        public void CheckAutoAction()
        {
            if (AppData.Instance.IsAutoStart)
            {
                Command_Handler(COMMAND_ACTION);
            }
        }

        /// <summary>
        /// 외부에서 Error Log를 작성하고자 할 때 사용하는 함수
        /// </summary>
        /// <param name="ex"><see cref="Exception" /> 정보</param>
        public void ErrorLogWrite(Exception ex)
        {
            AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, _myName, "비정상적 Root Error", ex);
        }

        #region ContextMenu Control Setting
        /// <summary>
        /// ContextMenu 초기화
        /// </summary>
        private void InitContextMenu()
        {
            // 첫번째 메뉴
            var processStart = new ToolStripMenuItem("프로세스 시작")
            {
                Enabled = !IsAction
            };
            processStart.Click += ProcessStart_Click;
            // 두번째 메뉴
            var processEnd = new ToolStripMenuItem("프로세스 종료");
            processEnd.Enabled = IsAction;
            processEnd.Click += ProcessEnd_Click;
            //세번째 메뉴
            var loadView = new ToolStripMenuItem("열기");
            loadView.Click += LoadView_Click;
            // 네번째 메뉴
            var progreamClose = new ToolStripMenuItem("닫기");
            progreamClose.Click += ProgreamClose_Click;

            // 중간 Separator 추가
            Menu.Items.AddRange(new ToolStripItem[] {
            processStart,
            processEnd,
            new ToolStripSeparator(),
            loadView,
            progreamClose});
        }
        /// <summary>
        /// Program 화면 열기
        /// </summary>
        /// <remarks>
        /// Program 화면 원래크기로 띄우기
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadView_Click(object sender, EventArgs e)
        {
            LoadedView();
        }
        /// <summary>
        /// 프로그램 완전종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgreamClose_Click(object sender, EventArgs e)
        {
            // 프로그램을 강제로 종료하는 부분입니다.
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "ContextMenu - Program 닫기");
            SystemShutdown = true;
            //Log삭제 Task 종료부분 추가
            logCleaner?.Stop();
            System.Windows.Application.Current.Shutdown();
        }
        /// <summary>
        /// Task 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessStart_Click(object sender, EventArgs e)
        {
            //지금 동작중이지 않은경우
            if (!IsAction)
            {
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "ContextMenu - Process 시작");
                Command_Handler(COMMAND_ACTION);
            }
        }

        private void ProcessEnd_Click(object sender, EventArgs e)
        {
            //지금 동작중인 경우
            if(IsAction)
            {
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, _myName, "ContextMenu - Process 종료");
                Command_Handler(COMMAND_STOP);
            }
        }
        #endregion
        #region Delegate 및 event 정의
        public event Action<bool> OnAction;
        private void ChangedState(bool isStart)
        {
            Menu.Items[0].Enabled = !isStart;
            Menu.Items[1].Enabled = isStart;
            this.OnAction?.Invoke(isStart);
        }
        public event Action OnLoaded;
        private void LoadedView()
        {
            this.OnLoaded?.Invoke();
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
