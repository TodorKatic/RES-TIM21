using Common.DeviceDataModel;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Common
{
    [DataContract]
    public enum DeviceType
    {
        [EnumMember]
        Digital,
        [EnumMember]
        Analog,
        [EnumMember]
        Controller
    }

    [Serializable]
    [DataContract]

    public class DeviceData
    {
        [DataMember]
        [XmlAttribute]
        public int DeviceCode { get; set; }

        [DataMember]
        public InnerDeviceData Data { get; set; }

        public DeviceData(int dcode, long timestamp,float value)
        {
            this.DeviceCode = dcode;
            Data = new InnerDeviceData(value, timestamp);
        }


        public override string ToString()
        {
            return $"Device:{DeviceCode}\tTimestamp:{Data.Timestamp}\tValue:{Data.Value}";
        }

        public DeviceData() { }
    }
}
