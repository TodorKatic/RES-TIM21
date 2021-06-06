using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS
{
    public class ChartWindowModel : IChartWindowModel,INotifyPropertyChanged
    {
        private float   _MinValue;
        private float   _MaxValue;
        private float   _DiffValue;
        private float   _MeanValue;

        private int     _DeviceCode;
        private uint     _UpTime;
        private IList<KeyValuePair<DateTime, float>> _DeviceData;

        public float MinValue
        {
            get => _MinValue;
            set
            {
                this._MinValue = value;
                NotifyPropertyChanged("MinValue");

            }
        }
        public float MaxValue
        {
            get => _MaxValue;
            set
            {
                this._MaxValue = value;
                NotifyPropertyChanged("MaxValue");

            }
        }
        public float Difference
        {
            get => _DiffValue;
            set
            {
                this._DiffValue = value;
                NotifyPropertyChanged("Difference");

            }
        }
        public float MeanValue
        {
            get => _MeanValue ;
            set
            {
                this._MeanValue = value;
                NotifyPropertyChanged("MeanValue");

            }
        }
        public uint UpTime
        {
            get => _UpTime;
            set
            {
                this._UpTime = value;
                NotifyPropertyChanged("UpTime");


            }
        }
        public int DeviceCode
        {
            get => _DeviceCode;
            set
            {
                this._DeviceCode = value;
                NotifyPropertyChanged("DeviceCode");

            }

        }

        public IList<KeyValuePair<DateTime, float>> DeviceData
        {
            get => _DeviceData;
            set
            {
                this._DeviceData = value;
                NotifyPropertyChanged("DeviceData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
