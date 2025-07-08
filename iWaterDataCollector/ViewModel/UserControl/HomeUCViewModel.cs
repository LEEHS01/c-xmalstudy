using iWaterDataCollector.Global;
using iWaterDataCollector.Global.Handler;
using iWaterDataCollector.Model.View;
using log4net;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

/********************************************
 * Kafka Tag Setting Page
 * Kafka Tag(Pulse Tag 포함) 설정 화면 ViewModel
 * Log Set Name → Program
 ********************************************/
namespace iWaterDataCollector.ViewModel.UserControl
{
    public class HomeUCViewModel : ViewModelBase, IDataErrorInfo
    {
        /// <summary>
        /// DataGrid Cell 선택 변경시 발생
        /// </summary>
        public IRelayCommand SelectionChangedCommand { get; set; }
        #region User Variable
        /// <summary>
        /// Log 시간 역순 Sort용 List
        /// </summary>
        public List<TimelineModel> SortTempList { get; set; } = new List<TimelineModel>();
        /// <summary>
        /// Progress Bar 및 누실시간 확인용 Timer
        /// </summary>
        private DispatcherTimer dt = new DispatcherTimer();
        #endregion
        #region Binding Data
        /// <summary>
        /// SCADA Server List
        /// </summary>
        public ObservableCollection<ServerModel> DisplayServers { get; set; } = new ObservableCollection<ServerModel>();
        /// <summary>
        /// Log 표출용 List
        /// </summary>
        public ObservableCollection<TimelineModel> DisplayList { get; set; } = new ObservableCollection<TimelineModel>();
        /// <summary>
        /// 시작/중지 Flag
        /// </summary>
        public bool IsStart
        {
            get => _isStart;
            set
            {
                if(_isStart != value)
                {
                    _isStart = value;
                    RaisePropertyChanged(nameof(IsStart));
                    if(value)
                    {
                        //동작
                        Action();
                    }
                    else
                    {
                        //중지
                        Stop();
                    }
                }
            }
        }
        private bool _isStart = true;
        /// <summary>
        /// Task 시작시간
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    RaisePropertyChanged(nameof(StartTime));
                }
            }
        }
        private DateTime _startTime;
        /// <summary>
        /// Task 시작 후 경과시간
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                if(_elapsedTime != value)
                {
                    _elapsedTime = value;
                    RaisePropertyChanged(nameof(ElapsedTime));
                }
            }
        }
        private TimeSpan _elapsedTime;
        /// <summary>
        /// 다음 Task 동작 시간
        /// </summary>
        public DateTime NextTime
        {
            get => _nextTime;
            set
            {
                if(_nextTime != value)
                {
                    _nextTime = value;
                    RaisePropertyChanged(nameof(NextTime));
                }
            }
        }
        private DateTime _nextTime;
        /// <summary>
        /// 마지막 Task 동작시간
        /// </summary>
        public DateTime LastTime
        {
            get => _lastTime;
            set
            {
                if(_lastTime != value)
                {
                    _lastTime = value;
                    RaisePropertyChanged(nameof(LastTime));
                }
            }
        }
        private DateTime _lastTime;
        /// <summary>
        /// Tag 누식 시간
        /// </summary>
        public TimeSpan LossTimeSpan
        {
            get => _lossTimeSpan;
            set
            {
                if(_lossTimeSpan != value)
                {
                    _lossTimeSpan = value;
                    RaisePropertyChanged(nameof(LossTimeSpan));
                }
            }
        }
        private TimeSpan _lossTimeSpan;
        /// <summary>
        /// Task 진행 예정시간
        /// </summary>
        public string RemainingTime
        {
            get => _remainingTime;
            set
            {
                if(_remainingTime != value)
                {
                    _remainingTime = value;
                    RaisePropertyChanged(nameof(RemainingTime));
                }
            }
        }
        private string _remainingTime;
        /// <summary>
        /// Task 동작 최대시간
        /// </summary>
        public int MaxSecond
        {
            get => _maxSecond;
            set
            {
                if(_maxSecond != value)
                {
                    _maxSecond = value;
                    RaisePropertyChanged(nameof(MaxSecond));
                }
            }
        }
        private int _maxSecond;
        /// <summary>
        /// ProgressBar 표출 시간
        /// </summary>
        public int ElapsedSecond
        {
            get => _elapsedSecond;
            set
            {
                if(_elapsedSecond != value)
                {
                    _elapsedSecond = value;
                    RaisePropertyChanged(nameof(ElapsedSecond));
                }
            }

        }
        private int _elapsedSecond;

        private int _storageInterval
        {
            get => (int)TimeSpan.FromMinutes(AppData.Instance.StorageIntervalD).TotalSeconds;
        }
        #endregion
        /// <summary>
        /// 생성자
        /// </summary>
        public HomeUCViewModel()
        {
            IsStart = false;
            BindData();
            BindCommand();
            SettingTimer();
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
            MaxSecond = _storageInterval - 1;
            DisplayServers = AppData.Instance.ServerCollection;
        }
        #endregion
        #region Timer 셋팅
        private void SettingTimer()
        {
            LastTime = FileHandler.LoadLastWorkingTime(AppData.Instance.DefaultPath, AppData.Instance.LocalNodeName);
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += Dt_Tick;
            dt.Start();
        }
        /// <summary>
        /// Timer Event Delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dt_Tick(object sender, EventArgs e)
        {
            if (IsStart)
            {
                ElapsedTime = DateTime.Now - StartTime;
                var duration = (int)(NextTime - DateTime.Now).TotalSeconds;
                if(duration < 0)
                {
                    RemainingTime = "대기중...";
                    ElapsedSecond = _maxSecond;
                }
                else
                {
                    RemainingTime = $"{duration:D2} 초";
                    ElapsedSecond = _maxSecond - duration;
                }
                
            }
            else
            {
                LossTimeSpan = DateTime.Now - LastTime;
            }
        }
        #endregion
        #region 시작/중지 버튼
        /// <summary>
        /// 시작동작
        /// </summary>
        public void Action()
        {
            StartTime = DateTime.Now;
            FirstTimeSet();
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, GetType().Name, "시작버튼 클릭");
        }
        /// <summary>
        /// 시작시간 설정
        /// </summary>
        private void FirstTimeSet()
        {
            var second = TimeSpan.FromMinutes(AppData.Instance.StorageIntervalD).TotalSeconds - StartTime.Second;
            NextTime = StartTime.AddSeconds(second);
            ElapsedSecond = _storageInterval - (int)second;
        }
        /// <summary>
        /// Task 동작 경과시간 Update
        /// </summary>
        /// <param name="name">SCADA Node Name</param>
        /// <param name="dt">동작시작시간</param>
        /// <param name="ts">동작경과시간</param>
        /// <summary>
        /// 중지동작
        /// </summary>
        public void Stop()
        {
            LastTime = FileHandler.LoadLastWorkingTime(AppData.Instance.DefaultPath, AppData.Instance.LocalNodeName);
            AppData.Instance.MsgIRDC.Info(AppData.AppLog, GetType().Name, "중지버튼 클릭");
        }
        #endregion
        #region User Function
        public void UpdateTimeline(string name, DateTime dt, TimeSpan ts, DateTime nowDT)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
              DispatcherPriority.Background,
              new Action(() =>
              {
                  var info = new TimelineModel()
                  {
                      Name = name,
                      StartTime = dt,
                      RunningTime = ts,
                      LoggingTime = nowDT
                  };

                  SortTempList.Add(info);
                  //20줄만 남김
                  if (SortTempList.Count > 20)
                  {
                      SortTempList.RemoveAt(0);
                  }

                  var sorted = SortTempList.OrderByDescending(t => t.LoggingTime).ToList();

                  DisplayList.Clear();
                  foreach (var item in sorted)
                  {
                      DisplayList.Add(item);
                  }
                  RaisePropertyChanged(nameof(DisplayList));
              }));
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, GetType().Name, $"{ name } DataList Event 처리 오류", ex);
            }
        }

        #endregion
        #region 명령등록
        /// <summary>
        /// 화면에서 Inboke될 Event Command 등록
        /// </summary>
        private void BindCommand()
        {
            SelectionChangedCommand = new RelayCommand<object>(SelectionChangedCommand_Handler);
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
            var args = obj as SelectionChangedEventArgs;
            var dg = args.Source as DataGrid;

            if (dg.SelectedIndex >= 0)
                dg.UnselectAllCells();
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
