using DeviceComponent.DeviceModel;
using System;


namespace DeviceComponent
{
    public abstract class AbstractDevice : IDeviceSim,IDevice
    {
        public int DeviceCode { get; set; }
        public float Value { get; protected set; }
        public Random valueGenerator { get; private set; }

        public AbstractDevice(int dcode)
        {
            this.DeviceCode = dcode;
            valueGenerator = new Random(DateTime.Now.Second);
        }

        public AbstractDevice()
        {
            valueGenerator = new Random(DateTime.Now.Second);
        }

        public abstract void ChangeState();
    }
}
