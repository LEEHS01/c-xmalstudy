using MahApps.Metro.Controls;
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
    /// SettingUCView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingUCView : System.Windows.Controls.UserControl
    {
        public SettingUCView()
        {
            InitializeComponent();
        }

        private void HamburgerMenuControl_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            HamburgerMenuControl.Content = e.InvokedItem;
        }
    }
}
