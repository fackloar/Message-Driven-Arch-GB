using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages.Interfaces
{
    internal interface IRepository<T> where T : class
    {
        void AddOrUpdate(T entity);
        IEnumerable<T> Get();
    }
}
