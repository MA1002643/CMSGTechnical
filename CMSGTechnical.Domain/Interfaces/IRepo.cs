using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSGTechnical.Domain.Interfaces
{
    public interface IRepo<TEntity> : IQueryable<TEntity> where TEntity : IEntity
    {

        public Task<TEntity?> Get(int id, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TEntity>> Get(IEnumerable<int> ids, CancellationToken cancellationToken = default);
        public IQueryable<TEntity> GetAll();
        public IQueryable<TEntity> GetAll(IEnumerable<int> ids);

        public IQueryable<TEntity> GetAll(params int[] ids) => GetAll((IEnumerable<int>)ids);
        public IQueryable<TEntity> this[int id] => GetAll(new[] { id });
        public IQueryable<TEntity> this[params int[] ids] => GetAll(ids);
        public IQueryable<TEntity> this[IEnumerable<int> ids] => GetAll(ids);

        public Task Add(TEntity entity, CancellationToken cancellationToken = default);
        public Task Add(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        public Task Update(TEntity entity, CancellationToken cancellationToken = default);
        public Task Update(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        public Task Modify(TEntity entity, CancellationToken cancellationToken = default);
        public Task Modify(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);



        public Task Delete(int id, CancellationToken cancellationToken = default);
        public Task Delete(TEntity entity, CancellationToken cancellationToken = default);
        public Task Delete(IEnumerable<int> ids, CancellationToken cancellationToken = default);
        public Task Delete(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    }
}
