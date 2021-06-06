
namespace DeviceComponent
{
    public class DigitalDevice : AbstractDevice
    {
        public DigitalDevice(int dcode) : base(dcode) { }

        public DigitalDevice() { }

        public override void ChangeState()
        {
            this.Value = valueGenerator.Next() % 2;
        }
    }
}
