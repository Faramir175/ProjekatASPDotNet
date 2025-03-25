using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public Task<List<TEntity>> GetAll();
        public Task<TEntity> GetById(Guid id);
        public Task<TEntity> Add(TEntity entity);
        public Task<TEntity> Update(TEntity entity);
        public Task<bool> Delete(TEntity entity);

    }
}
