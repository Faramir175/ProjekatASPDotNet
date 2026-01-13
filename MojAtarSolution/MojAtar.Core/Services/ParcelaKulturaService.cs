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

            var updatedEntity = await _parcelaKulturaRepository.Update(dto.ToParcelaKultura());
            return updatedEntity?.ToParcelaKulturaDTO();
        }


        public async Task<ParcelaKulturaDTO?> UpdateNezavrsena(Guid idParcela, Guid idKultura, decimal novaPovrsina)
        {
            var entity = await _parcelaKulturaRepository.UpdateNezavrsena(idParcela, idKultura, novaPovrsina);
            return entity?.ToParcelaKulturaDTO();
        }

        public async Task<int> DeleteIfNotCompleted(Guid idParcela, Guid idKultura, Guid idSetvaRadnja)
        {
            return await _parcelaKulturaRepository.DeleteAddedForParcelaKultura(idParcela, idKultura, idSetvaRadnja);
        }
        public async Task<ParcelaKulturaDTO?> GetNezavrsenaSetva(Guid idParcela, Guid idKultura)
        {
            var entity = await _parcelaKulturaRepository.GetNezavrsenaSetva(idParcela, idKultura);
            return entity?.ToParcelaKulturaDTO();
        }
        public async Task<List<ParcelaKulturaDTO>> GetSveNezavrseneSetve(Guid idParcela, Guid idKultura)
        {
            var list = await _parcelaKulturaRepository.GetSveNezavrseneSetve(idParcela, idKultura);
            return list.Select(x => x.ToParcelaKulturaDTO()).ToList();
        }

        public async Task<ParcelaKulturaDTO?> GetBySetvaRadnjaId(Guid idSetvaRadnja)
        {
            var entity = await _parcelaKulturaRepository.GetBySetvaRadnjaId(idSetvaRadnja);
            return entity?.ToParcelaKulturaDTO(); 
        }
        public async Task<List<ParcelaKulturaDTO>> GetAllBySetvaRadnjaId(Guid idSetvaRadnja)
        {
            var list = await _parcelaKulturaRepository.GetAllBySetvaRadnjaId(idSetvaRadnja);
            return list.Select(x => x.ToParcelaKulturaDTO()).ToList();
        }

        public async Task<List<ParcelaKulturaDTO>> GetSveZaZetvu(Guid idZetvaRadnja)
        {
            var list = await _parcelaKulturaRepository.GetSveZaZetvu(idZetvaRadnja);
            return list.Select(x => x.ToParcelaKulturaDTO()).ToList();
        }

    }
}
