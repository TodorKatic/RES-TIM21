
namespace DeviceComponent
{

    public class AnalogDevice : AbstractDevice
    {
        public int limitingValue { get; private set; }

        public AnalogDevice(int dcode,int limitingValue) : base(dcode)
        {
            this.limitingValue = limitingValue;
        }

        public AnalogDevice(int limitingValue)
        {
            this.limitingValue = limitingValue;
        }

        public override void ChangeState()
        {
            this.Value = (float)(valueGenerator.NextDouble() * valueGenerator.Next()) % this.limitingValue;
        }
    }
}
