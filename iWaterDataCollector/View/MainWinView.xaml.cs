using iWaterDataCollector.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace iWaterDataCollector.View
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWinView : MetroWindow
    {
        // NotifyIcon 생성
        private NotifyIcon _tray = new NotifyIcon();

        public MainWinView()
        {
            var proc = Process.GetProcessesByName("iWaterDataCollector");

            if (1 < proc.Length)
            {
                System.Windows.Forms.MessageBox.Show("iWater 데이터 수집기 프로그램이 이미 실행중입니다.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Windows.Application.Current.Shutdown();
                return;
            }

            InitializeComponent();
            SetNotification();
            var vm = new MainWinViewModel();
            vm.OnAction += Vm_OnAction;
            vm.OnLoaded += Vm_OnLoaded; 
            _tray.ContextMenuStrip = vm.Menu;
            vm.CheckAutoAction();
            this.DataContext = vm;
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            this.Dispatcher.UnhandledExceptionFilter += Dispatcher_UnhandledExceptionFilter;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var vm = (MainWinViewModel)DataContext;
            if (vm.SystemShutdown)
            {
                base.OnClosing(e);
                // 프로그램 종료 후 NotifyIcoy 리소스를 해제합니다.
                // 해제하지 않을 경우 프로그램이 완전히 종료되지 않는 경우도 발생합니다.
                _tray.Dispose();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                this.Hide();
                _tray.Visible = true;
            }
        }


        #region 비정상 종료 방지 코드
        private void Dispatcher_UnhandledExceptionFilter(object sender, System.Windows.Threading.DispatcherUnhandledExceptionFilterEventArgs e)
        {
            var vm = (MainWinViewModel)DataContext;
            System.Windows.Forms.MessageBox.Show("비정상적인 종료신호가 포착되었습니다.", "종료안내", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            vm.ErrorLogWrite(e.Exception);
            _tray.Visible = false;
            _tray.Dispose();
            e.RequestCatch = true; 
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var vm = (MainWinViewModel)DataContext;
            System.Windows.Forms.MessageBox.Show("비정상적인 종료신호가 포착되었습니다.", "종료안내", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            vm.ErrorLogWrite(e.Exception);
            _tray.Visible = false;
            _tray.Dispose();
            e.Handled = true;
        }
        #endregion

        private void Vm_OnLoaded()
        {
            // 화면을 최소화 상태에서 다시 보여줍니다.
            this.Show();
            // 화면 상태를 Normal로 설정합니다.
            this.WindowState = WindowState.Normal;
            _tray.Visible = false;
        }

        private void Vm_OnAction(bool isStart)
        {
            //프로세스 시작/중지 여부 확인용
            // NotifyIcon에 마우스를 올릴 경우 나타나는 글입니다.
            // 설정 안해주셔도됩니다.
            string stat;
            if (isStart)
            {
                stat = "Start";
                _tray.Icon = Properties.Resources.MainWindow;
            }
            else
            {
                stat = "Stop";
                _tray.Icon = Properties.Resources.MainWindow_2;
            }
            _tray.Text = $"iWaterDataCollector[State:{stat}]";
        }

        private void SetNotification()
        {
            this.WindowState = WindowState.Minimized;
            //this.ShowInTaskbar = false;
            this.Hide();

            //Explorer 프로세스 재시작 ****** trayicon 잔상 제거 : 최후의 수단
            //using (var tempIcon = new NotifyIcon())
            //{
            //    tempIcon.Icon = _tray.Icon = Properties.Resources.MainWindow_2;
            //    tempIcon.Visible = true;

            //    Thread.Sleep(200);
            //    tempIcon.Visible = false;
            //    tempIcon.Dispose();
            //}

            // NotifyIcon에 더블 클릭 이벤트 추가
            _tray.DoubleClick += _tray_DoubleClick;
            // NotifyIcon에 사용할 ico 파일 경로입니다. 절대 경로로 설정해 주시면 됩니다.
            // 설정하지 않을 경우 아이콘이 보이지 않습니다.
            _tray.Icon = Properties.Resources.MainWindow_2;
            // NotifyIcon이 보일 수 있도록 Visible 항목을 true로 설정합니다.
            // 기본설정이 false이기 때문에 반드시 true로 설정해주셔야합니다.
            _tray.Visible = true;
            _tray.Text = "iWaterDataCollector[State:Stop]";
        }

        private void _tray_DoubleClick(object sender, EventArgs e)
        {
            // 화면을 최소화 상태에서 다시 보여줍니다.
            this.Show();
            // 화면 상태를 Normal로 설정합니다.
            this.WindowState = WindowState.Normal;
            _tray.Visible = false;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (MainWinViewModel)DataContext;
            if(vm.SystemShutdown)
            {
                // 프로그램 종료 후 NotifyIcoy 리소스를 해제합니다.
                // 해제하지 않을 경우 프로그램이 완전히 종료되지 않는 경우도 발생합니다.
                _tray.Dispose();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                this.Hide();
                _tray.Visible = true;
            }
        }

    }
}
