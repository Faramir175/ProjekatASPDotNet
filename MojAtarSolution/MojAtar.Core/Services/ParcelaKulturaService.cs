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
    public class ParcelaKulturaService : IParcelaKulturaService
    {
        private readonly IParcelaKulturaRepository _parcelaKulturaRepository;

        public ParcelaKulturaService(IParcelaKulturaRepository parcelaKulturaRepository)
        {
            _parcelaKulturaRepository = parcelaKulturaRepository;
        }

        public async Task<ParcelaKulturaDTO> Add(ParcelaKulturaDTO dto)
        {
            if (dto == null || dto.IdParcela == null || dto.IdKultura == null)
                throw new ArgumentNullException("DTO ili ID vrednosti ne smeju biti null");

            var entity = dto.ToParcelaKultura();
            var added = await _parcelaKulturaRepository.Add(entity);
            return added.ToParcelaKulturaDTO();
        }

        public async Task<bool> Delete(Guid id)
        {
            return await _parcelaKulturaRepository.DeleteById(id);
        }

        public async Task<List<ParcelaKulturaDTO>> GetAll()
        {
            var entities = await _parcelaKulturaRepository.GetAll();
            return entities.Select(pk => pk.ToParcelaKulturaDTO()).ToList();
        }

        public async Task<List<ParcelaKulturaDTO>> GetAllByParcelaId(Guid idParcela)
        {
            var entities = await _parcelaKulturaRepository.GetAllByParcelaId(idParcela);
            return entities.Select(pk => pk.ToParcelaKulturaDTO()).ToList();
        }

        public async Task<ParcelaKulturaDTO?> GetByParcelaAndKulturaId(Guid idParcela, Guid idKultura)
        {
            var entity = await _parcelaKulturaRepository.GetByParcelaAndKulturaId(idParcela, idKultura);
            return entity?.ToParcelaKulturaDTO();
        }

        public async Task<ParcelaKulturaDTO?> Update(ParcelaKulturaDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var updatedEntity = await _parcelaKulturaRepository.Update(new Parcela_Kultura
            {
                Id = dto.Id,
                IdParcela = dto.IdParcela,
                IdKultura = dto.IdKultura,
                Povrsina = dto.Povrsina,
                DatumSetve = dto.DatumSetve,
                DatumZetve = dto.DatumZetve
            });

            return updatedEntity?.ToParcelaKulturaDTO();
        }
    }
}
