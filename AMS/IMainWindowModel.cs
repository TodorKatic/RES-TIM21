using Common;
using Common.DeviceDataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS
{
    public interface IMainWindowModel
    {
        bool IsDatesAnnotationEnabled { get; set; }
        bool IsSelectedDeviceAnnotationEnabled { get; set; }

        BindingList<DeviceModel> DeviceModels { get; set; }
        DeviceModel Selected { get; set; }

        int StartDateHourSlider { get; set; }
        int EndDateHourSlider { get; set; }

        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }

        void GetAllDevices();
        void AddDevice(int id,int uptime,DeviceType type);
        void UpdateDevice(int id,int newNumberOfChanges,int newUpTime);
        bool Validate();
        void Reset();
    }
}
