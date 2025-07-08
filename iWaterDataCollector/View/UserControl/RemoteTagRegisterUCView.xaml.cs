using iWaterDataCollector.ViewModel.UserControl;
using System;

namespace iWaterDataCollector.View.UserControl
{
    /// <summary>
    /// RemoteTagRegisterUCView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RemoteTagRegisterUCView : System.Windows.Controls.UserControl, IDisposable
    {
        public RemoteTagRegisterUCView()
        {
            InitializeComponent();
            var vm = new RemoteTagRegisterUCViewModel();
            DataContext = vm;
            vm.OnFilter += Vm_OnFilter;
            vm.InitContextMenu();
        }

        public void Dispose()
        {

        }

        private void Vm_OnFilter(bool isVisible)
        {
            var vm = (RemoteTagRegisterUCViewModel)DataContext;
            if (isVisible)
            {
                grdTag.ContextMenu = vm.Menu;
            }
            else
            {
                grdTag.ContextMenu = null;
            }
        }
    }
}
