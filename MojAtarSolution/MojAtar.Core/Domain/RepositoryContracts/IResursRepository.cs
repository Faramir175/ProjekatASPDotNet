using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IResursRepository : IRepository<Resurs>
    {
        public Task<Resurs> GetByNaziv(string naziv);
        public Task<bool> DeleteResursById(Guid? id);
        public Task<List<Resurs>> GetAllByKorisnik(Guid idKorisnik);

    }
}
