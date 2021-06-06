using System.Data.Entity;

namespace AMSDatabaseAccess.DatabaseWrapper
{
    public interface IContext
    {
        DbSet<Device> Devices { get; }
        DbSet<DevicesData> DevicesDatas { get; }
        int SaveChanges();

    }
}
