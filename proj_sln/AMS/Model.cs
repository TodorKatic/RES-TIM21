using AMS.DataAdapters;
using Common;
using Common.DeviceDataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AMS
{
    public class Model:INotifyPropertyChanged,IMainWindowModel
    {
        private bool _IsDatesAnnotationEnabled = false;
        private bool _IsSelectedDeviceAnnotationEnabled = false;

        private IList<DeviceModel> _DeviceModels;
        private DeviceModel _Selected;

        private int _StartDateHourSlider;
        private int _EndDateHourSlider;

        private DateTime _StartDate = DateTime.Now;
        private DateTime _EndDate = DateTime.Now;

        private BaseToViewDataAdapter DataAdapter;

        public Model(BaseToViewDataAdapter adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("Adapter podataka ne sme biti null");

            this.DataAdapter = adapter;
        }


        public BindingList<DeviceModel> DeviceModels
        {

            get => new BindingList<DeviceModel>(this._DeviceModels);
            set
            {
                this._DeviceModels = value;
                NotifyPropertyChanged("DeviceModels");
            }

        }

        public int StartDateHourSlider
        {
            get => _StartDateHourSlider;
            set
            {
                this._StartDateHourSlider = value;
                NotifyPropertyChanged("StartDateHourSlider");
            }
        }

        public int EndDateHourSlider
        {
            get => _EndDateHourSlider;
            set
            {
                this._EndDateHourSlider= value;
                NotifyPropertyChanged("EndDateHourSlider");
            }
        }

        public DateTime StartDate
        {
            get => _StartDate;
            set
            {
                this._StartDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }

        public DateTime EndDate
        {
            get => _EndDate;
            set
            {
                this._EndDate = value;
                NotifyPropertyChanged("EndDate");
            }
        }

        public bool IsDatesAnnotationEnabled
        {
            get => _IsDatesAnnotationEnabled;
            set
            {
                this._IsDatesAnnotationEnabled = value;
                NotifyPropertyChanged("IsDatesAnnotationEnabled");

            }
        }

        public bool IsSelectedDeviceAnnotationEnabled
        {
            get => _IsSelectedDeviceAnnotationEnabled;
            set
            {
                this._IsSelectedDeviceAnnotationEnabled = value;
                NotifyPropertyChanged("IsSelectedDeviceAnnotationEnabled");
            }
        }

        public DeviceModel Selected
        {
            get => _Selected;
            set
            {
                this._Selected = value;
                NotifyPropertyChanged("Selected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddDevice(int id, int uptime, DeviceType type)
        {
            DeviceModel newDevice = new DeviceModel() { DeviceCode = id, DeviceType = type, UpTime = uptime };

            this.DeviceModels.Add(newDevice);
            NotifyPropertyChanged("DeviceModels");
        }

        public void GetAllDevices()
        {
            this.DeviceModels = new BindingList<DeviceModel>(DataAdapter.GetAllDevices());
        }

        public void Reset()
        {
            this.IsDatesAnnotationEnabled = false;
            this.IsSelectedDeviceAnnotationEnabled = false;
        }

        public void UpdateDevice(int id,int newNumberOfChanges,int newUpTime)
        {
            DeviceModel model = this.DeviceModels.FirstOrDefault<DeviceModel>(o => o.DeviceCode == id);
            if(model != null)
            {
                model.NumberOfChanges = newNumberOfChanges;
                model.UpTime = newUpTime;
                model.RowColor = this.DataAdapter.ChangeColorIfNeeded(model);

                NotifyPropertyChanged("DeviceModels");
            }
        }

        public bool Validate()
        {
            bool retval = true;


            if ((StartDate.Year > EndDate.Year) || (EndDate.Year == StartDate.Year && StartDate.Month > EndDate.Month) ||
                (EndDate.Year == StartDate.Year && EndDate.Month == StartDate.Month && StartDate.Day > EndDate.Day) || 
                (EndDate.Year == StartDate.Year && EndDate.Month == StartDate.Month && StartDate.Day == EndDate.Day && StartDateHourSlider >= EndDateHourSlider))
            {
                this.IsDatesAnnotationEnabled = true;
                retval = false;
                Selected = null;
            }
            else if(Selected == null)
            {
                this.IsSelectedDeviceAnnotationEnabled = true;
                retval = false;
                Selected = null;
            }
            else
            {
                this.StartDate = this.StartDate.AddHours((double)this.StartDateHourSlider);
                this.EndDate = this.EndDate.AddHours((double)this.EndDateHourSlider);
            }


            return retval;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
