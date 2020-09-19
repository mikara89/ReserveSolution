using Microsoft.EntityFrameworkCore;
using Player.Data.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Player.Service.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected PlayerDBContext RepositoryContext { get; set; }

        public RepositoryBase(PlayerDBContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        public IQueryable<T> FindAll(bool readOnly = true)
        {
            if (readOnly)
                return this.RepositoryContext.Set<T>().AsNoTracking();
            return this.RepositoryContext.Set<T>();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,bool readOnly=true)
        {
            if(readOnly)
                return this.RepositoryContext.Set<T>().Where(expression).AsNoTracking();
            return this.RepositoryContext.Set<T>().Where(expression);
        }

        public void Create(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
        }

        public async Task SaveAsync()
        {
            await this.RepositoryContext.SaveChangesAsync();
        }
    }
}
