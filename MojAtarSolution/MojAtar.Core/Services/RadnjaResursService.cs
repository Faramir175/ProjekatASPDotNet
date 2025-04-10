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
    public class RadnjaResursService : IRadnjaResursService
    {
        private readonly IRadnjaResursRepository _radnjaResursRepository;

        public RadnjaResursService(IRadnjaResursRepository radnjaResursRepository)
        {
            _radnjaResursRepository = radnjaResursRepository;
        }

        public async Task<RadnjaResursDTO> Add(RadnjaResursDTO dto)
        {
            var entity = await _radnjaResursRepository.Add(dto.ToResurs());
            return entity.ToResursDTO();
        }

        public async Task<RadnjaResursDTO> Update(RadnjaResursDTO dto)
        {
            var entity = await _radnjaResursRepository.Update(dto.ToResurs());
            return entity.ToResursDTO();
        }

        public async Task<bool> Delete(Guid idRadnja, Guid idResurs)
        {
            var entity = await _radnjaResursRepository.GetById(idRadnja, idResurs);
            if (entity == null) return false;

            return await _radnjaResursRepository.Delete(entity);
        }

        public async Task<RadnjaResursDTO?> GetById(Guid idRadnja, Guid idResurs)
        {
            var entity = await _radnjaResursRepository.GetById(idRadnja, idResurs);
            return entity?.ToResursDTO();
        }

        public async Task<List<RadnjaResursDTO>> GetAllByRadnjaId(Guid idRadnja)
        {
            var entities = await _radnjaResursRepository.GetAllByRadnjaId(idRadnja);
            return entities.Select(e => e.ToResursDTO()).ToList();
        }
        public async Task<List<RadnjaResursDTO>> GetAllByUser(Guid idKorisnik)
        {
            var entities = await _radnjaResursRepository.GetAllByKorisnikId(idKorisnik);
            return entities.Select(e => e.ToResursDTO()).ToList();
        }

    }
}
