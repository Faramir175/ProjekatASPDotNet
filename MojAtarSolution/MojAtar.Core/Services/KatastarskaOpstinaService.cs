using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.ExtensionKlase;
using MojAtar.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Services
{
    public class KatastarskaOpstinaService:IKatastarskaOpstinaService
    {
        private readonly IKatastarskaOpstinaRepository _katastarskaOpstinaRepository;

        public KatastarskaOpstinaService(IKatastarskaOpstinaRepository katastarskaOpstinaRepository)
        {
            _katastarskaOpstinaRepository = katastarskaOpstinaRepository;
        }

        public async Task<KatastarskaOpstinaDTO> Add(KatastarskaOpstinaDTO katastarskaAdd)
        {
            if (katastarskaAdd == null)
            {
                throw new ArgumentNullException(nameof(katastarskaAdd));
            }

            if (katastarskaAdd.Naziv == null)
            {
                throw new ArgumentException(nameof(katastarskaAdd.Naziv));
            }

            KatastarskaOpstina katastarskaOpstina = katastarskaAdd.ToKatastarskaOpstina();
            katastarskaOpstina.Id = Guid.NewGuid();

            await _katastarskaOpstinaRepository.Add(katastarskaOpstina);
            katastarskaAdd.Id = katastarskaOpstina.Id;

            return katastarskaAdd;
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            KatastarskaOpstina? katastarksaOpstina = await _katastarskaOpstinaRepository.GetById(id.Value);
            if (katastarksaOpstina == null)
                return false;

            await _katastarskaOpstinaRepository.Delete(katastarksaOpstina);

            return true;
        }

        public async Task<List<KatastarskaOpstinaDTO>> GetAll()
        {
            var katastarskeOpstine = await _katastarskaOpstinaRepository.GetAll();

            // Koristimo LINQ za čistiji kod umesto foreach petlje
            return katastarskeOpstine
                .Select(ko => ko.ToKatastarskaOpstinaDTO())
                .ToList();
        }

        public async Task<KatastarskaOpstinaDTO> GetById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            KatastarskaOpstina? katastarskaOpstina = await _katastarskaOpstinaRepository.GetById(id.Value);

            if (katastarskaOpstina == null) return null;

            return katastarskaOpstina.ToKatastarskaOpstinaDTO();
        }

        public async Task<KatastarskaOpstinaDTO> Update(KatastarskaOpstinaDTO dto)
        {
            if (dto.Id == null)
            {
                throw new ArgumentNullException(nameof(dto.Id));
            }
            KatastarskaOpstina? existingOpstina = await _katastarskaOpstinaRepository.GetById(dto.Id.Value);

            if (existingOpstina == null)
                return null;

            existingOpstina.Naziv = dto.Naziv;
            existingOpstina.GradskaOpstina = dto.GradskaOpstina;

            if (dto.Parcele != null)
            {
                existingOpstina.Parcele = dto.Parcele;
            }

            await _katastarskaOpstinaRepository.Update(existingOpstina);

            return existingOpstina.ToKatastarskaOpstinaDTO();
        }

    }
}
