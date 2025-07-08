using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model
{
    /// <summary>
    /// View Action용 Property Change Event & Delegate Class
    /// </summary>
    public class ModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged 멤버
        protected void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
