using AirJourney_Blog.BLL.Helper;
using AirJourney_Blog.BLL.Interfaces;
using AirJourney_Blog.BLL.Ordering;
using AirJourney_Blog.DAL.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.BLL.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDBContext context;
        internal DbSet<T> dbset;

        public GenericRepository(AppDBContext _context)
        {
            context = _context;
            dbset = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await dbset.AddAsync(entity);
        }

        public async Task<int> CountAsync()
        {
            return await dbset.CountAsync();
        }

        public void Delete(T entity)
        {
            dbset.Remove(entity);
        }

        public async Task<PaginatedResult<T>> GetAllAsync(
            int? take = 10,
            int? skip = 0,
            Expression<Func<T, bool>> criteria = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = dbset.AsQueryable();

            // Filtering
            if (criteria != null)
                query = query.Where(criteria);

            int totalCount = await query.CountAsync();

            // Sorting
            if (orderBy != null)
            {
                query = orderByDirection == OrderBy.Ascending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            // Pagination (safeguard for take = 0 or null)
            int actualTake = take.GetValueOrDefault(10);
            if (actualTake <= 0)
                actualTake = 10; // or throw BadRequest

            int actualSkip = skip.GetValueOrDefault(0);

            query = query.Skip(actualSkip).Take(actualTake);

            var items = await query.ToListAsync();

            int totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / actualTake);
            int currentPage = totalCount == 0 ? 0 : (actualSkip / actualTake) + 1;

            return new PaginatedResult<T>
            {
                Items = items,
                CurrentPage = currentPage,
                PageSize = actualTake,
                TotalPages = totalPages
            };
        }





        public async Task<T> GetByIdAsync(int id)
        {
            var res = await dbset.FindAsync(id);
            return res;
        }

        public void Update(T entity)
        {
            dbset.Update(entity); 
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbset.AnyAsync(predicate);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria)
        {
            IQueryable<T> query = context.Set<T>();


            return await query.SingleOrDefaultAsync(criteria);
        }

    }
}
