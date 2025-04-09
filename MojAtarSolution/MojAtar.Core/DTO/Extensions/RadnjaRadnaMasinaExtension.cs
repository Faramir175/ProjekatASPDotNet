using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class RadnjaRadnaMasinaExtension
    {
        public static RadnjaRadnaMasinaDTO? ToRadnaMasinaDTO(this Radnja_RadnaMasina radnjaRadnaMasina)
        {
            return new RadnjaRadnaMasinaDTO()
            {
                IdRadnja = (Guid)radnjaRadnaMasina.IdRadnja,
                IdRadnaMasina = (Guid)radnjaRadnaMasina.IdRadnaMasina,
                BrojRadnihSati = radnjaRadnaMasina.BrojRadnihSati,
            };
        }
    }
}
