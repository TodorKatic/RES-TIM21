using System;
using System.Runtime.Serialization;

namespace Common.DeviceDataModel
{
    [DataContract]
    [Serializable]
    public class InnerDeviceData
    {
        [DataMember]
        public float Value { get; set; }

        [DataMember]
        public long Timestamp { get; set; }

        public InnerDeviceData(float val, long timestamp)
        {
            this.Value = val;
            this.Timestamp = timestamp;
        }

        public InnerDeviceData() { }
    }
}
