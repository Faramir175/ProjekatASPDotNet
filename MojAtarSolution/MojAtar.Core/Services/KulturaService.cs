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
    public class KulturaService : IKulturaService
    {
        private readonly IKulturaRepository _kulturaRepository;

        public KulturaService(IKulturaRepository kulturaRepository)
        {
            _kulturaRepository = kulturaRepository;
        }

        public async Task<KulturaDTO> Add(KulturaDTO kulturaAdd)
        {
            if (kulturaAdd == null)
            {
                throw new ArgumentNullException(nameof(kulturaAdd));
            }

            if (kulturaAdd.Naziv == null)
            {
                throw new ArgumentException(nameof(kulturaAdd.Naziv));
            }

            if (await _kulturaRepository.GetByNaziv(kulturaAdd.Naziv) != null)
            {
                throw new ArgumentException("Uneti naziv kulture vec postoji");
            }

            Kultura kultura = kulturaAdd.ToKultura();

            kultura.Id = Guid.NewGuid();

            await _kulturaRepository.Add(kultura);

            // Dodavanje cene
            CenaKulture cena = new CenaKulture
            {
                Id = Guid.NewGuid(),
                IdKultura = kultura.Id.Value,
                CenaPojedinici = kultura.AktuelnaCena,
                DatumVaznosti = DateTime.Now
            };
            await _kulturaRepository.DodajCenu(cena); // ovo trebaš da dodaš u interfejs i implementaciju


            return kultura.ToKulturaDTO();
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Kultura? kultura = await _kulturaRepository.GetById(id.Value);
            if (kultura == null)
                return false;

            await _kulturaRepository.DeleteKulturaById(id.Value);

            return true;
        }

        public async Task<List<KulturaDTO>> GetAllForUser(Guid idKorisnika)
        {
            List<Kultura> parcele = await _kulturaRepository.GetAllByKorisnik(idKorisnika);
            return parcele.Select(k => k.ToKulturaDTO()).ToList();
        }



        public async Task<KulturaDTO> GetById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Kultura? kultura = await _kulturaRepository.GetById(id.Value);

            if (kultura == null) return null;

            return kultura.ToKulturaDTO();
        }
        public async Task<KulturaDTO> GetByNaziv(string? naziv)
        {
            if (naziv == null)
            {
                throw new ArgumentNullException(nameof(naziv));
            }

            Kultura? kultura = await _kulturaRepository.GetByNaziv(naziv);

            if (kultura == null) return null;

            return kultura.ToKulturaDTO();
        }

        public async Task<KulturaDTO> Update(Guid? id, KulturaDTO dto)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var staraKultura = await _kulturaRepository.GetById(id.Value);
            if (staraKultura == null)
                return null;

            if (staraKultura.AktuelnaCena != dto.AktuelnaCena)
            {
                CenaKulture novaCena = new CenaKulture
                {
                    Id = Guid.NewGuid(),
                    IdKultura = id.Value,
                    CenaPojedinici = dto.AktuelnaCena,
                    DatumVaznosti = DateTime.Now
                };

                await _kulturaRepository.DodajCenu(novaCena);
            }

            staraKultura.Naziv = dto.Naziv;
            staraKultura.Hibrid = dto.Hibrid;
            staraKultura.AktuelnaCena = dto.AktuelnaCena;
            staraKultura.IdKorisnik = dto.IdKorisnik;

            await _kulturaRepository.Update(staraKultura);

            return staraKultura.ToKulturaDTO();
        }

    }
}
