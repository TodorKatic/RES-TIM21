using Common;
using Common.DeviceDataModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;

namespace ControllerComponent
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single,ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DeviceDataSaver : ISendDeviceData
    {

        public ConcurrentDictionary<int, IList<InnerDeviceData>> LocalBufferReference { get; private set; } 
        
        public DeviceDataSaver() { }
  
        public DeviceDataSaver(ConcurrentDictionary<int,IList<InnerDeviceData>> buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("Prosledjeni lokalni bafer ne sme biti null!");

            this.LocalBufferReference = buffer;
        }

        public bool SendDeviceData(DeviceData data)
        {
            bool retval = false;
            if (data == null)
                throw new ArgumentNullException("Prosledjeni podaci ne smeju biti null!");


            if (LocalBufferReference.ContainsKey(data.DeviceCode))
            {
                LocalBufferReference[data.DeviceCode].Add(new InnerDeviceData(data.Data.Value, data.Data.Timestamp));
                Console.WriteLine(data);
                retval = true;
            }
            else
            {
                LocalBufferReference[data.DeviceCode] = new List<InnerDeviceData>() { new InnerDeviceData(data.Data.Value, data.Data.Timestamp) };
                retval = true;
            }

            return retval;
        }
    }
}
