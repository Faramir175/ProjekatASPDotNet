using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class RadnjaPrikljucnaMasinaExtension
    {
        public static RadnjaPrikljucnaMasinaDTO? ToPrikljucnaMasinaDTO(this Radnja_PrikljucnaMasina radnjaPrikljucnaMasina)
        {
            return new RadnjaPrikljucnaMasinaDTO()
            {
                IdRadnja = (Guid)radnjaPrikljucnaMasina.IdRadnja,
                IdPrikljucnaMasina = (Guid)radnjaPrikljucnaMasina.IdPrikljucnaMasina,
                BrojRadnihSati = radnjaPrikljucnaMasina.BrojRadnihSati,
            };
        }
    }
}
