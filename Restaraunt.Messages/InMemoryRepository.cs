using Restaraunt.Messages.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Messages
{
    internal class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly ConcurrentBag<T> _repo = new();

        public void AddOrUpdate(T entity)
        {
            _repo.Add(entity);
        }

        public IEnumerable<T> Get()
        {
            return _repo;
        }
    }
}
