using Microsoft.EntityFrameworkCore;
using Server.Data;
using Sever;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected PhoneshopIdentityContext RepositoryContext { get; set; }

        public RepositoryBase(PhoneshopIdentityContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext ?? throw new ArgumentNullException(nameof(repositoryContext));
        }

        public IQueryable<T> FindAll()
        {
            return this.RepositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
        RepositoryContext.Set<T>().Where(expression).AsNoTracking();
    }
}