using AMSDatabaseAccess;
using Common;
using System;
using System.Collections.Generic;
using AMSDatabaseAccess.DatabaseCRUD;
using System.Windows.Media;
using System.ComponentModel;

namespace AMS.DataAdapters
{
    public class BaseToViewDataAdapter
    {
        public ICrudDaoDevice DeviceTableAccess { get; private set; }
        public ICrudDaoDeviceData DataTableAccess { get; private set; }

        public uint AnalogMaxUpTime { get; private set; }
        public uint DigitalMaxUpTime { get; private set; }
        public uint AnalogMaxChanges { get; private set; }
        public uint DigitalMaxChanges { get; private set; }
        public uint ControllerMaxUpTime { get; private set; }
        public uint AppToRealSecondRatio { get; private set; }


        public BaseToViewDataAdapter(ICrudDaoDeviceData devicesData,ICrudDaoDevice deviceCrud,uint AnalogMaxChanges,uint DigitalMaxChanges,uint DigitalUpTime,uint AnalogUpTime,uint ControllerUpTime,uint AppToRealSecondRatio)
        {
            if (devicesData == null)
                throw new ArgumentNullException("Referenca na tabelu podataka ne sme biti null");

            if (deviceCrud == null)
                throw new ArgumentNullException("Referenca na tablelu uredjaja ne sme biti null");



            this.DataTableAccess = devicesData;
            this.DeviceTableAccess = deviceCrud;

            this.AnalogMaxUpTime = AnalogUpTime;
            this.DigitalMaxUpTime = DigitalUpTime;
            this.AnalogMaxChanges = AnalogMaxChanges;
            this.DigitalMaxChanges = DigitalMaxChanges;
            this.ControllerMaxUpTime = ControllerUpTime;
            this.AppToRealSecondRatio = AppToRealSecondRatio;

        }

        public IList<KeyValuePair<DateTime,float>> GetGraphData(int key,long lowerTimestamp,long upperTimestamp)
        {

            IEnumerable<DevicesData> databaseGraphData = this.DataTableAccess.GetGraphData(key, lowerTimestamp, upperTimestamp);

            IList<KeyValuePair<DateTime, float>> GraphData = new List<KeyValuePair<DateTime, float>>();

            DateTimeOffset sourceTime;
            DateTime target;


            foreach(var device in databaseGraphData)
            {

                sourceTime = DateTimeOffset.FromUnixTimeSeconds(device.Timestamp);
                target = sourceTime.DateTime;

                GraphData.Add(new KeyValuePair<DateTime, float>(target,device.Data));
            }

            return GraphData;
        }

        public BindingList<DeviceModel> GetAllDevices()
        {
            IEnumerable<Device> databaseDevices = this.DeviceTableAccess.GetAll();

            BindingList<DeviceModel> devices = new BindingList<DeviceModel>();

            foreach(var device in databaseDevices)
            {
                DeviceModel model = new DeviceModel() { DeviceCode = device.Id, DeviceType = (DeviceType)device.Type, UpTime = device.UpTime, NumberOfChanges = device.NumberOfChanges };
                model.RowColor = ChangeColorIfNeeded(model);

                devices.Add(model);
            }

            return devices;
        }


        public SolidColorBrush ChangeColorIfNeeded(DeviceModel model)
        {
            SolidColorBrush color = new SolidColorBrush(Colors.White);

            switch ((DeviceType)model.DeviceType)
            {
                case DeviceType.Analog:
                    if (model.UpTime >= this.AnalogMaxUpTime && model.NumberOfChanges < this.AnalogMaxChanges)
                        color = new SolidColorBrush(Colors.Red);
                    else if (model.NumberOfChanges >= this.AnalogMaxChanges && model.UpTime < this.AnalogMaxUpTime)
                        color = new SolidColorBrush(Colors.Orange);
                    else if (model.NumberOfChanges >= this.AnalogMaxChanges && model.UpTime >= this.AnalogMaxUpTime)
                        color = new SolidColorBrush(Colors.DarkViolet);

                    break;
                case DeviceType.Digital:
                    if (model.UpTime >= this.DigitalMaxUpTime && model.NumberOfChanges < this.DigitalMaxChanges)
                        color = new SolidColorBrush(Colors.Red);
                    else if (model.NumberOfChanges >= this.DigitalMaxChanges && model.UpTime < this.DigitalMaxUpTime)
                        color = new SolidColorBrush(Colors.Orange);
                    else if (model.NumberOfChanges >= this.DigitalMaxChanges && model.UpTime >= this.DigitalMaxUpTime)
                        color = new SolidColorBrush(Colors.DarkViolet);

                    break;

                case DeviceType.Controller:
                    if (model.UpTime >= this.ControllerMaxUpTime)
                        color = new SolidColorBrush(Colors.Red);
                    break;

            }

            return color;
        }


        public long ConvertToUnixTimestamp(DateTime dateTime)
        {
            return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }


        public void GetGraphDetails(int key,long lowerTimestamp,long higherTimestamp, out float MaxValue,out float MinValue,out float MeanValue,out float Diff)
        {
            MaxValue = 0;
            MinValue = 0;
            MeanValue = 0;
            Diff = 0;

            float? maxVal = this.DataTableAccess.GetMaxData(key,lowerTimestamp,higherTimestamp);
            float? minVal = this.DataTableAccess.GetMinData(key,lowerTimestamp, higherTimestamp);
            float? meanVal = this.DataTableAccess.GetMeanValue(key,lowerTimestamp, higherTimestamp);
            float? firstVal = this.DataTableAccess.GetFirstData(key,lowerTimestamp, higherTimestamp);
            float? lastVal = this.DataTableAccess.GetLastData(key,lowerTimestamp, higherTimestamp);


            if(maxVal != null)
            {
                MaxValue = (float)maxVal;
            }

            if(minVal != null)
            {
                MinValue = (float)minVal;
            }

            if(meanVal != null)
            {
                MeanValue = (float)meanVal;
            }

            if(firstVal != null && lastVal != null)
            {
                Diff = Math.Abs((float)(lastVal - firstVal));
            }
        }
    }
}
