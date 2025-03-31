using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.ExtensionKlase
{
    public static class KatastarskaOpstinaExtension
    {
        public static KatastarskaOpstinaDTO? ToKatastarskaOpstinaDTO(this KatastarskaOpstina katastarskaOpstina)
        {
            if (katastarskaOpstina == null)
            {
                return null; 
            }

            return new KatastarskaOpstinaDTO()
            {
                Id = katastarskaOpstina.Id,
                Naziv = katastarskaOpstina.Naziv,
                GradskaOpstina = katastarskaOpstina.GradskaOpstina,
                Parcele = katastarskaOpstina.Parcele
            };
        }
    }
}
