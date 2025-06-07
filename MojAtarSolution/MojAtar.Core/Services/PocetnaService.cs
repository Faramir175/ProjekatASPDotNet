using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.ExtensionKlase;
using MojAtar.Core.DTO.Extensions;
using MojAtar.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Services
{
    public class PocetnaService : IPocetnaService
    {
        private readonly IParcelaRepository _parcelaRepo;
        private readonly IRadnjaRepository _radnjaRepo;
        private readonly IResursRepository _resursRepo;
        private readonly IRadnaMasinaRepository _radnaRepo;
        private readonly IPrikljucnaMasinaRepository _prikljucnaRepo;
        private readonly IKulturaRepository _kulturaRepo;

        public PocetnaService(
            IParcelaRepository parcelaRepo,
            IRadnjaRepository radnjaRepo,
            IResursRepository resursRepo,
            IRadnaMasinaRepository radnaRepo,
            IPrikljucnaMasinaRepository prikljucnaRepo,
            IKulturaRepository kulturaRepo)
        {
            _parcelaRepo = parcelaRepo;
            _radnjaRepo = radnjaRepo;
            _resursRepo = resursRepo;
            _radnaRepo = radnaRepo;
            _prikljucnaRepo = prikljucnaRepo;
            _kulturaRepo = kulturaRepo;
        }

        public Task<int> GetBrojParcelaAsync(Guid korisnikId) =>
            _parcelaRepo.CountByKorisnikId(korisnikId);

        public Task<int> GetBrojRadnjiAsync(Guid korisnikId) =>
            _radnjaRepo.CountByKorisnikId(korisnikId);

        public Task<int> GetBrojResursaAsync(Guid korisnikId) =>
            _resursRepo.CountByKorisnikId(korisnikId);

        public Task<int> GetBrojRadnihMasinaAsync(Guid korisnikId) =>
            _radnaRepo.CountByKorisnikId(korisnikId);

        public Task<int> GetBrojPrikljucnihMasinaAsync(Guid korisnikId) =>
            _prikljucnaRepo.CountByKorisnikId(korisnikId);

        public Task<int> GetBrojKulturaAsync(Guid korisnikId) =>
            _kulturaRepo.CountByKorisnikId(korisnikId);

        public Task<List<Radnja>> GetPoslednjeRadnjeAsync(Guid korisnikId, int broj = 5) =>
            _radnjaRepo.GetLastRadnjeByKorisnik(korisnikId, broj);
    }

}
