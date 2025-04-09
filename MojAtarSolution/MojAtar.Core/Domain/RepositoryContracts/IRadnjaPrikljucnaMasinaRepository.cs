using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IRadnjaPrikljucnaMasinaRepository
    {
        public  Task<Radnja_PrikljucnaMasina> Add(Radnja_PrikljucnaMasina entity);
        public  Task<bool> Delete(Radnja_PrikljucnaMasina entity);
        public  Task<Radnja_PrikljucnaMasina> Update(Radnja_PrikljucnaMasina entity);
        public Task<Radnja_PrikljucnaMasina> GetById(Guid idRadnja, Guid idPrikljucnaMasina);
        public Task<List<Radnja_PrikljucnaMasina>> GetAllByRadnjaId(Guid idRadnja);
        public Task<List<Radnja_PrikljucnaMasina>> GetAllByKorisnikId(Guid idKorisnik);

    }
}
