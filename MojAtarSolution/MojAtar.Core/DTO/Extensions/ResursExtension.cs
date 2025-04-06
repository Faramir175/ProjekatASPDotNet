using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class ResursExtension
    {
        public static ResursDTO? ToResursDTO(this Resurs resurs)
        {
            return new ResursDTO()
            {
                Id = resurs.Id,
                Naziv = resurs.Naziv,
                Vrsta = resurs.Vrsta,
                AktuelnaCena = resurs.AktuelnaCena,
                IdKorisnik = (Guid)resurs.IdKorisnik
            };
        }
    }
}
