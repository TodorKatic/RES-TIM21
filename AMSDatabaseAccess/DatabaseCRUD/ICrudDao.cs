using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMSDatabaseAccess.DatabaseCRUD
{
    public interface ICrudDao<T>
    {
        void Insert(T entity);
    }
}
