using iWaterDataCollector.Model;
using iWaterDataCollector.ViewModel.UserControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iWaterDataCollector.View.UserControl
{
    /// <summary>
    /// KafkaTagCheckerUCView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KafkaTagCheckerUCView : System.Windows.Controls.UserControl, IDisposable
    {
        public KafkaTagCheckerUCView()
        {
            InitializeComponent();
            var vm = new KafkaTagCheckerUCViewModel();
            vm.OnFilter += Vm_OnFilter;
            DataContext = vm;
            vm.InitContextMenu();
        }

        public void Dispose()
        {
            
        }

        private void Vm_OnFilter(bool isVisible)
        {
            var vm = (KafkaTagCheckerUCViewModel)DataContext;
            if(isVisible)
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
