

using System.Collections.Generic;

namespace AMSDatabaseAccess.DatabaseCRUD
{
    public interface ICrudDaoDevice:ICrudDao<Device>
    {
        Device Get(int key);
        IEnumerable<Device> GetAll();

        void Update(Device entity);
        bool Exists(int key);
    }
}
