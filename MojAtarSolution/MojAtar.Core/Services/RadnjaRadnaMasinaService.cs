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
    public class RadnjaRadnaMasinaService : IRadnjaRadnaMasinaService
    {
        private readonly IRadnjaRadnaMasinaRepository _radnjaRadnaMasinaRepository;

        public RadnjaRadnaMasinaService(IRadnjaRadnaMasinaRepository radnjaRadnaMasinaRepository)
        {
            _radnjaRadnaMasinaRepository = radnjaRadnaMasinaRepository;
        }

        public async Task<RadnjaRadnaMasinaDTO> Add(RadnjaRadnaMasinaDTO dto)
        {
            var entity = await _radnjaRadnaMasinaRepository.Add(dto.ToRadnaMasina());
            return entity.ToRadnaMasinaDTO();
        }

        public async Task<RadnjaRadnaMasinaDTO> Update(RadnjaRadnaMasinaDTO dto)
        {
            var entity = await _radnjaRadnaMasinaRepository.Update(dto.ToRadnaMasina());
            return entity.ToRadnaMasinaDTO();
        }

        public async Task<bool> Delete(Guid idRadnja, Guid idRadnaMasina)
        {
            var entity = await _radnjaRadnaMasinaRepository.GetById(idRadnja, idRadnaMasina);
            if (entity == null) return false;

            return await _radnjaRadnaMasinaRepository.Delete(entity);
        }

        public async Task<RadnjaRadnaMasinaDTO?> GetById(Guid idRadnja, Guid idRadnaMasina)
        {
            var entity = await _radnjaRadnaMasinaRepository.GetById(idRadnja, idRadnaMasina);
            return entity?.ToRadnaMasinaDTO();
        }

        public async Task<List<RadnjaRadnaMasinaDTO>> GetAllByRadnjaId(Guid idRadnja)
        {
            var entities = await _radnjaRadnaMasinaRepository.GetAllByRadnjaId(idRadnja);
            return entities.Select(e => e.ToRadnaMasinaDTO()).ToList();
        }
        public async Task<List<RadnjaRadnaMasinaDTO>> GetAllByUser(Guid idKorisnik)
        {
            var entities = await _radnjaRadnaMasinaRepository.GetAllByKorisnikId(idKorisnik);
            return entities.Select(e => e.ToRadnaMasinaDTO()).ToList();
        }

    }
}
