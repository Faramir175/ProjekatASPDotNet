using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class RadnjaResursExtension
    {
        public static RadnjaResursDTO? ToResursDTO(this Radnja_Resurs radnjaResurs)
        {
            return new RadnjaResursDTO()
            {
                IdRadnja = (Guid)radnjaResurs.IdRadnja,
                IdResurs = (Guid)radnjaResurs.IdResurs,
                Kolicina = radnjaResurs.Kolicina,
                DatumKoriscenja = radnjaResurs.DatumKoriscenja,
            };
        }
    }
}
