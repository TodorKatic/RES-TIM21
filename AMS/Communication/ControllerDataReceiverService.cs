using System;
using System.Linq;
using System.ServiceModel;
using AMSDatabaseAccess.DatabaseCRUD;
using Common;
using Common.ControllerDataModel;

namespace AMS.Communication
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    public class ControllerDataReceiverService : ISendControllerData
    {
        public ICrudDaoDeviceData dataTableHandle { get; private set; }
        public ICrudDaoDevice deviceTableHandle { get; private set; }
        public uint AppToRealSecondRation { get; private set; }
        public IMainWindowModel UIModel { get; private set; }


        public ControllerDataReceiverService(ICrudDaoDeviceData dataTableHandle, ICrudDaoDevice deviceTableHandle, uint secondRatio, IMainWindowModel UIModel)
        {
            if (dataTableHandle == null)
                throw new ArgumentNullException("Handle za objekat koji radi sa tabelom podataka ne sme biti null");

            if (deviceTableHandle == null)
                throw new ArgumentNullException("Handle za objekat koji radi sa tabelom uredjaja ne sme biti null");

            if (UIModel == null)
                throw new ArgumentNullException("Prosledjen model ne sme biti null");


            this.deviceTableHandle = deviceTableHandle;
            this.dataTableHandle = dataTableHandle;
            this.AppToRealSecondRation = secondRatio;
            this.UIModel = UIModel;
        }



        /// <summary>
        /// Prima i obradjuje podatke koje je kontroler poslao
        /// Updejtuje broj radnih sati kontrolera tako sto oduzme timestamp u kom je kontroler poslao podatke i timestamp u kom se kontroler javio na ams
        /// i podeli razliku sa 3600 da bi bio dobijen broj sati jer timestampovi su sekunde od epohe
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendControllerData(ControllerData data)
        {
            if (data == null)
                return false;

            try
            {
                Console.WriteLine("RECEIVED CONTROLLER DATA");
                int upTime;
                var controller = this.deviceTableHandle.Get(data.LocalControllerCode);
                if (controller != null)
                {
                    upTime = (int)(this.AppToRealSecondRation * (data.UnixTimestamp - controller.WakeUpTime)) / 3600;
                    controller.UpTime = upTime;

                    this.deviceTableHandle.Update(controller);

                    Console.WriteLine($"UPDATE:\tDEVICE ID:{controller.Id}\tUP_TIME:{controller.UpTime}");
                }
                else
                {
                    Console.WriteLine("Referenca na kontroler je null");
                }

                foreach (var deviceData in data.DevicesDataList)
                {
                    int id = deviceData.DeviceCode;
                    foreach (var dataNode in deviceData.DataList)
                    {
                        if (dataNode != null)
                            dataTableHandle.Insert(new AMSDatabaseAccess.DevicesData() { Id = id, Data = (float)dataNode.Value, Timestamp = dataNode.Timestamp });
                    }

                    var device = this.deviceTableHandle.Get(deviceData.DeviceCode);

                    var lastDataNode = deviceData.DataList.OrderBy(dev => dev.Timestamp).Last();

                    if (device != null)
                    {
                        upTime = (int)(this.AppToRealSecondRation * (lastDataNode.Timestamp - device.WakeUpTime) / 3600);
                        device.UpTime = upTime;
                        this.deviceTableHandle.Update(device);
                        Console.WriteLine($"UPDATE:\tDEVICE ID:{device.Id}\tUP TIME:{device.UpTime}\tNUM OF CHANGES:{device.NumberOfChanges}");
                    }
                    else
                    {
                        Console.WriteLine("REFERENCA NA DEVICE JE NULL");
                    }
                }

                this.UIModel.GetAllDevices();

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }

        }
    }
}
