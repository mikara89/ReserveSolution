using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Player.Service.Repository
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(bool readOnly=true);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool readOnly = true);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task SaveAsync();
    }
}
