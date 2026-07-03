using DAL.Data;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly OfficeMonitorContext Context;
        protected readonly DbSet<T> DbSet;

        public GenericRepository(OfficeMonitorContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id) => await DbSet.FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllAsync() =>
            await DbSet.AsNoTracking().ToListAsync();

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await DbSet.AsNoTracking().Where(predicate).ToListAsync();

        public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

        public void Update(T entity) => DbSet.Update(entity);

        public void Remove(T entity) => DbSet.Remove(entity);
    }
}
