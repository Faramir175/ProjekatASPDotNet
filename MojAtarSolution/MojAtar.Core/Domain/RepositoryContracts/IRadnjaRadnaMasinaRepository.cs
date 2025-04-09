using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IRadnjaRadnaMasinaRepository
    {
        public  Task<Radnja_RadnaMasina> Add(Radnja_RadnaMasina entity);
        public  Task<bool> Delete(Radnja_RadnaMasina entity);
        public  Task<Radnja_RadnaMasina> Update(Radnja_RadnaMasina entity);
        public Task<Radnja_RadnaMasina> GetById(Guid idRadnja, Guid idRadnaMasina);
        public Task<List<Radnja_RadnaMasina>> GetAllByRadnjaId(Guid idRadnja);
        public Task<List<Radnja_RadnaMasina>> GetAllByKorisnikId(Guid idKorisnik);

    }
}
