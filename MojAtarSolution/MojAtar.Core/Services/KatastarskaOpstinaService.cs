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
            List<KatastarskaOpstina> katastarskaOpstina = await _katastarskaOpstinaRepository.GetAll();
            List<KatastarskaOpstinaDTO> dto = new List<KatastarskaOpstinaDTO>();
            foreach(KatastarskaOpstina ko in katastarskaOpstina)
            {
                dto.Add(ko.ToKatastarskaOpstinaDTO());
            }
            return dto;
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
            KatastarskaOpstina? katastarskaOpstina = new KatastarskaOpstina()
            {
                Id = dto.Id,
                Naziv = dto.Naziv,
                GradskaOpstina = dto.GradskaOpstina,
                Parcele = dto.Parcele,
            };

            await _katastarskaOpstinaRepository.Update(katastarskaOpstina);

            if (katastarskaOpstina == null) return null;
            return katastarskaOpstina.ToKatastarskaOpstinaDTO();
        }

    }
}
