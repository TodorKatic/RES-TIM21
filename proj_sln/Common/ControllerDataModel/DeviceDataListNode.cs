using Common.DeviceDataModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Common.ControllerDataModel
{
    [DataContract]
    [Serializable]
    public class DeviceDataListNode
    {
        [DataMember]
        public int DeviceCode { get; set; }

        [DataMember]
        public List<InnerDeviceData> DataList { get; set; }

        public DeviceDataListNode() { }

        public DeviceDataListNode(int dcode,List<InnerDeviceData> dlist)
        {
            if (dlist == null)
                throw new ArgumentNullException("Lista podataka uredjaja ne sme biti null");

            this.DeviceCode = dcode;
            this.DataList = dlist;
        }
    }
}
