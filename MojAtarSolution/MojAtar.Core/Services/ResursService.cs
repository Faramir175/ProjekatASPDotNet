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

        public ResursService(IResursRepository resursRepository)
        {
            _resursRepository = resursRepository;
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

            if (await _resursRepository.GetByNaziv(resursAdd.Naziv) != null)
            {
                throw new ArgumentException("Uneti naziv kulture vec postoji");
            }

            Resurs rersurs = resursAdd.ToResurs();

            rersurs.Id = Guid.NewGuid();

            await _resursRepository.Add(rersurs);

            return rersurs.ToResursDTO();
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Resurs? resurs = await _resursRepository.GetById(id.Value);
            if (resurs == null)
                return false;

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
        public async Task<ResursDTO> GetByNaziv(string? naziv)
        {
            if (naziv == null)
            {
                throw new ArgumentNullException(nameof(naziv));
            }

            Resurs? resurs = await _resursRepository.GetByNaziv(naziv);

            if (resurs == null) return null;

            return resurs.ToResursDTO();
        }

        public async Task<ResursDTO> Update(Guid? id, ResursDTO dto)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            Resurs? resurs = new Resurs()
            {
                Id = id.Value,
                Naziv = dto.Naziv,
                Vrsta = dto.Vrsta,
                AktuelnaCena = dto.AktuelnaCena,
                IdKorisnik = dto.IdKorisnik
            };

            await _resursRepository.Update(resurs);

            if (resurs == null) return null;
            return resurs.ToResursDTO();
        }

    }
}
