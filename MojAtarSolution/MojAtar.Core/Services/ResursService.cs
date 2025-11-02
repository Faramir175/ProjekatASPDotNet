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
    public class ResursService : IResursService
    {
        private readonly IResursRepository _resursRepository;
        private readonly IRadnjaService _radnjaService;
        private readonly IRadnjaResursService _radnjaResursService;

        public ResursService(
            IResursRepository resursRepository,
            IRadnjaService radnjaService,
            IRadnjaResursService radnjaResursService)
        {
            _resursRepository = resursRepository;
            _radnjaService = radnjaService;
            _radnjaResursService = radnjaResursService;
        }


        public async Task<ResursDTO> Add(ResursDTO resursAdd)
        {
            if (resursAdd == null)
            {
                throw new ArgumentNullException(nameof(resursAdd));
            }

            if (resursAdd.Naziv == null)
            {
                throw new ArgumentException(nameof(resursAdd.Naziv));
            }

            var existing = await _resursRepository.GetByNazivIKorisnik(resursAdd.Naziv, resursAdd.IdKorisnik);
            if (existing != null)
                throw new ArgumentException("Već postoji resurs sa ovim nazivom za vaš nalog.");

            Resurs resurs = resursAdd.ToResurs();

            resurs.Id = Guid.NewGuid();

            await _resursRepository.Add(resurs);

            CenaResursa cena = new CenaResursa
            {
                Id = Guid.NewGuid(),
                IdResurs = resurs.Id.Value,
                CenaPojedinici = resurs.AktuelnaCena,
                DatumVaznosti = resursAdd.DatumVaznostiCene != DateTime.MinValue ? resursAdd.DatumVaznostiCene : DateTime.Now
            };
            await _resursRepository.DodajCenu(cena);

            return resurs.ToResursDTO();
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var resurs = await _resursRepository.GetById(id.Value);
            if (resurs == null)
                return false;

            // 1️⃣ Nađi sve radnje koje koriste ovaj resurs
            var radnjeResursi = await _radnjaResursService.GetAllByUser(resurs.IdKorisnik);
            var vezaneRadnje = radnjeResursi
                .Where(rr => rr.IdResurs == id.Value)
                .Select(rr => rr.IdRadnja)
                .Distinct()
                .ToList();

            // 2️⃣ Obriši veze resursa i radnji
            foreach (var idRadnja in vezaneRadnje)
            {
                await _radnjaResursService.Delete(idRadnja, id.Value);
            }

            // 3️⃣ Ažuriraj ukupne troškove svake radnje koja je koristila resurs
            foreach (var idRadnja in vezaneRadnje)
            {
                await _radnjaService.UpdateUkupanTrosak(idRadnja);
            }

            // 4️⃣ Obriši sam resurs
            await _resursRepository.DeleteResursById(id.Value);
            return true;
        }


        public async Task<List<ResursDTO>> GetAllForUser(Guid idKorisnika)
        {
            List<Resurs> resursi = await _resursRepository.GetAllByKorisnik(idKorisnika);
            return resursi.Select(r => r.ToResursDTO()).ToList();
        }



        public async Task<ResursDTO> GetById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Resurs? resurs = await _resursRepository.GetById(id.Value);

            if (resurs == null) return null;

            return resurs.ToResursDTO();
        }
        public async Task<ResursDTO> GetByNaziv(string? naziv, Guid idKorisnik)
        {
            if (naziv == null)
            {
                throw new ArgumentNullException(nameof(naziv));
            }

            Resurs? resurs = await _resursRepository.GetByNazivIKorisnik(naziv, idKorisnik);

            if (resurs == null) return null;

            return resurs.ToResursDTO();
        }

        public async Task<ResursDTO> Update(Guid? id, ResursDTO dto)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var stariResurs = await _resursRepository.GetById(id.Value);
            if (stariResurs == null)
                return null;

            if (stariResurs.AktuelnaCena != dto.AktuelnaCena)
            {
                CenaResursa novaCena = new CenaResursa
                {
                    Id = Guid.NewGuid(),
                    IdResurs = id.Value,
                    CenaPojedinici = dto.AktuelnaCena,
                    DatumVaznosti = dto.DatumVaznostiCene != DateTime.MinValue ? dto.DatumVaznostiCene : DateTime.Now
                };

                await _resursRepository.DodajCenu(novaCena);
            }

            stariResurs.Naziv = dto.Naziv;
            stariResurs.Vrsta = dto.Vrsta;
            stariResurs.AktuelnaCena = dto.AktuelnaCena;
            stariResurs.IdKorisnik = dto.IdKorisnik;

            await _resursRepository.Update(stariResurs);

            return stariResurs.ToResursDTO();
        }
        public async Task<List<ResursDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            var resursi = await _resursRepository.GetAllByKorisnikPaged(idKorisnik, skip, take);
            return resursi.Select(r => r.ToResursDTO()).ToList();
        }

        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _resursRepository.GetCountByKorisnik(idKorisnik);
        }

    }
}
