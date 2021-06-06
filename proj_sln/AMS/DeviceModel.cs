using Common;
using System.Windows.Media;

namespace AMS
{
    public class DeviceModel
    {
        public int DeviceCode { get; set; }
        public int UpTime { get; set; }
        public DeviceType DeviceType { get; set; }
        public int NumberOfChanges { get; set; }
        public Brush RowColor { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
                return false;

            DeviceModel model = obj as DeviceModel;

            return this.DeviceCode == model.DeviceCode;
        }
    }
}
