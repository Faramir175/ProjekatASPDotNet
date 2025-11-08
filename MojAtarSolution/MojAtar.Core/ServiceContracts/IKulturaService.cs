using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IKulturaService
    {
        public Task<KulturaDTO> Add(KulturaDTO dto);
        public Task<List<KulturaDTO>> GetAllForUser(Guid idKorisnika);
        public Task<KulturaDTO> GetById(Guid? id);
        public Task<KulturaDTO> GetByNaziv(string? naziv, Guid idKorisnik);
        public Task<KulturaDTO> Update(Guid? id, KulturaDTO dto);
        public Task<bool> DeleteById(Guid? id);
        Task<List<KulturaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);
        Task<int> GetCountByKorisnik(Guid idKorisnik);
        Task<KulturaDTO> GetWithAktuelnaCena(Guid idKorisnik, Guid idKultura);
        Task AzurirajPosleZetve(Guid idKultura, decimal dodatiPrinos);
        Task AzurirajPosleProdaje(Guid idKultura, decimal kolicina);
        Task VratiPosleBrisanjaProdaje(Guid idKultura, decimal kolicina);
        Task AzurirajPosleIzmeneZetve(Guid idKultura, decimal stariPrinos, decimal noviPrinos);
        Task<bool> MozeSmanjenje(Guid idKultura, decimal razlika);
        Task<bool> MozeBrisanjeZetve(Guid idKultura, decimal prinos);

    }
}
