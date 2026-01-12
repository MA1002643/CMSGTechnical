using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using CMSGTechnical.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Repository
{
    public class Repo<TEntity> : IRepo<TEntity> where TEntity : class, IEntity
    {


        private ApplicationDbContext Context { get; }

        protected IQueryable<TEntity> Query => _query??=Context.Set<TEntity>();
        private IQueryable<TEntity>? _query;

        public Repo(ApplicationDbContext context)
        {
            Context = context;
        }


        #region Add

        public virtual async Task Add(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await Context.AddRangeAsync(entities, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Context.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Update

        public virtual async Task Update(TEntity entity, CancellationToken cancellationToken = default)
        {
            //if an entity has been updated, the tracking will spot it. The Update call here forces the modified state onto an entity.
            Context.Update(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }
        public virtual async Task Update(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            //if an entity has been updated, the tracking will spot it. The Update call here forces the modified state onto an entity.
            Context.UpdateRange(entities);
            await Context.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Delete

        public virtual async Task Delete(int id, CancellationToken cancellationToken = default)
        {
            var entity = GetAll(new[] { id });
            await Delete(entity, cancellationToken);
        }

        public virtual async Task Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Delete(new[] { entity }, cancellationToken);
        }

        public virtual async Task Delete(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            var entities = GetAll(ids);
            await Delete(entities, cancellationToken);
        }

        public virtual async Task Delete(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            Context.RemoveRange(entities);
            await Context.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Modify

        public virtual async Task Modify(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
                await Add(entity, cancellationToken);
            else
                await Update(entity, cancellationToken);
        }

        public virtual async Task Modify(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {

            var update = new List<TEntity>();
            var add = new List<TEntity>();

            foreach (var entity in entities)
                switch (Context.Entry(entity).State)
                {
                    case EntityState.Added:
                        add.Add(entity);
                        break;
                    case EntityState.Modified:
                        update.Add(entity);
                        break;
                }

            await Context.AddRangeAsync(add, cancellationToken);
            //if an entity has been updated, the tracking will spot it. The Update call here forces the modified state onto an entity.
            //Context.UpdateRange(update);
            await Context.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Get

        public IQueryable<TEntity> GetAll()
        {
            return Query;
        }

        public virtual async Task<IEnumerable<TEntity>> Get(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            var query = GetAll(ids);
            var entities = await query.ToListAsync(cancellationToken);
            return entities;
        }

        public virtual async Task<TEntity?> Get(int id, CancellationToken cancellationToken = default)
        {
            var r = await this.SingleOrDefaultAsync(i => Equals(i.Id, id), cancellationToken);
            return r;
        }

        public virtual IQueryable<TEntity> GetAll(IEnumerable<int> ids)
        {
            var query = GetAll().Where(i => ids.Contains(i.Id));
            return query;
        }


        #endregion

        #region IQueryableStuff

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType => Query.ElementType;
        public Expression Expression => Query.Expression;
        public IQueryProvider Provider => Query.Provider;

        #endregion

        
    }
}
