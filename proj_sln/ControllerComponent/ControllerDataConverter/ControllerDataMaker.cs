using System;
using System.Collections.Generic;
using Common;
using Common.ControllerDataModel;
using System.Linq;
using Common.ControllerDataModel.DataModelInterfaces;
using Common.DeviceDataModel;

namespace ControllerComponent
{
    public class ControllerDataMaker:IMakeControllerData
    {
        public ControllerData MakeControllerData(IController controller,IGetUnixTimestamp TimeKeep)
        {
            if (controller == null)
                throw new ArgumentNullException("Prosledjeni kontroler ne sme biti null!");

            if (TimeKeep == null)
                throw new ArgumentNullException("Instanca TimeKeep klase ne sme biti null");

            List<DeviceDataListNode> EverySignedUpDeviceData = new List<DeviceDataListNode>();

            foreach(var kvp in controller.LocalBuffer.ToList())
            {
                EverySignedUpDeviceData.Add(new DeviceDataListNode(kvp.Key, kvp.Value.ToList<InnerDeviceData>()));
            }
            ControllerData data = new ControllerData(controller.ControllerCode, TimeKeep.GetUnixTimestamp(),EverySignedUpDeviceData);

            return data;
        }
    }
}
