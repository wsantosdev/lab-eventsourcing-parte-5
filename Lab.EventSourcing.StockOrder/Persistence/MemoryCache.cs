using System;
using System.Collections.Concurrent;

namespace Lab.EventSourcing.StockOrder
{
    public class MemoryCache
    {
        private readonly ConcurrentDictionary<Guid, dynamic> _readModels = new ConcurrentDictionary<Guid, dynamic>();

        private MemoryCache() { }

        private static readonly MemoryCache _instance = new MemoryCache();
        public static MemoryCache Create() => _instance;

        public T Get<T>(Guid id) where T : class => 
            _readModels[id] as T;

        public void AddOrUpdate<T>(Guid id, T readModel) =>
            _readModels.AddOrUpdate(id, readModel, (id, stored) => readModel);
    }
}
