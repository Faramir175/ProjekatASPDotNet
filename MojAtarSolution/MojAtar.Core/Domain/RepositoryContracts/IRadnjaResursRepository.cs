using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IRadnjaResursRepository
    {
        public  Task<Radnja_Resurs> Add(Radnja_Resurs entity);
        public  Task<bool> Delete(Radnja_Resurs entity);
        public  Task<Radnja_Resurs> Update(Radnja_Resurs entity);
        public Task<Radnja_Resurs> GetById(Guid idRadnja, Guid idResurs);
        public Task<List<Radnja_Resurs>> GetAllByRadnjaId(Guid idRadnja);
        public Task<List<Radnja_Resurs>> GetAllByKorisnikId(Guid idKorisnik);

    }
}
