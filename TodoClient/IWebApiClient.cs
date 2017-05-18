using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoClient
{
    public interface IWebApiClient<TEntity, in TKey> where TEntity : IEntityWithKey<TKey>
    {
        Task<Uri> Add(TEntity todo);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Find(TKey id);
        Task<bool> Remove(TKey id);
        Task<bool> Update(TKey id, TEntity todo);
    }

    public interface IEntityWithKey<TKey>
    {
        TKey Key { get; set; }
    }
}