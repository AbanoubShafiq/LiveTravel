using AirJourney_Blog.BLL.Interfaces;
using AirJourney_Blog.DAL.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.BLL.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext context;
        private Hashtable repositories;

        public UnitOfWork(AppDBContext _context)
        {
            context = _context;
        }
        public async Task<int> CompleteAsync()
        {
            return await context.SaveChangesAsync();
        }

        public IGenericRepository<T> GenericRepository<T>() where T : class
        {
            if (repositories is null)
                repositories = new Hashtable();

            var entityKey = typeof(T).Name;

            if (!repositories.ContainsKey(entityKey))
            {
                var repositoryType = typeof(GenericRepository<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), context);

                repositories.Add(entityKey, repositoryInstance);
            }
            return (IGenericRepository<T>)repositories[entityKey];
        }
    }
}
