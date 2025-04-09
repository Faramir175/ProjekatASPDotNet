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
    public class RadnjaPrikljucnaMasinaService : IRadnjaPrikljucnaMasinaService
    {
        private readonly IRadnjaPrikljucnaMasinaRepository _radnjaPrikljucnaMasinaRepository;

        public RadnjaPrikljucnaMasinaService(IRadnjaPrikljucnaMasinaRepository radnjaPrikljucnaMasinaRepository)
        {
            _radnjaPrikljucnaMasinaRepository = radnjaPrikljucnaMasinaRepository;
        }

        public async Task<RadnjaPrikljucnaMasinaDTO> Add(RadnjaPrikljucnaMasinaDTO dto)
        {
            var entity = await _radnjaPrikljucnaMasinaRepository.Add(dto.ToPrikljucnaMasina());
            return entity.ToPrikljucnaMasinaDTO();
        }

        public async Task<RadnjaPrikljucnaMasinaDTO> Update(RadnjaPrikljucnaMasinaDTO dto)
        {
            var entity = await _radnjaPrikljucnaMasinaRepository.Update(dto.ToPrikljucnaMasina());
            return entity.ToPrikljucnaMasinaDTO();
        }

        public async Task<bool> Delete(Guid idRadnja, Guid idRadnaMasina)
        {
            var entity = await _radnjaPrikljucnaMasinaRepository.GetById(idRadnja, idRadnaMasina);
            if (entity == null) return false;

            return await _radnjaPrikljucnaMasinaRepository.Delete(entity);
        }

        public async Task<RadnjaPrikljucnaMasinaDTO?> GetById(Guid idRadnja, Guid idRadnaMasina)
        {
            var entity = await _radnjaPrikljucnaMasinaRepository.GetById(idRadnja, idRadnaMasina);
            return entity?.ToPrikljucnaMasinaDTO();
        }

        public async Task<List<RadnjaPrikljucnaMasinaDTO>> GetAllByRadnjaId(Guid idRadnja)
        {
            var entities = await _radnjaPrikljucnaMasinaRepository.GetAllByRadnjaId(idRadnja);
            return entities.Select(e => e.ToPrikljucnaMasinaDTO()).ToList();
        }
        public async Task<List<RadnjaPrikljucnaMasinaDTO>> GetAllByUser(Guid idKorisnik)
        {
            var entities = await _radnjaPrikljucnaMasinaRepository.GetAllByKorisnikId(idKorisnik);
            return entities.Select(e => e.ToPrikljucnaMasinaDTO()).ToList();
        }

    }
}
