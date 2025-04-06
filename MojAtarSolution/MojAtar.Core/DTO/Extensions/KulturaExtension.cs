using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class KulturaExtension
    {
        public static KulturaDTO? ToKulturaDTO(this Kultura kultura)
        {
            return new KulturaDTO()
            {
                Id = kultura.Id,
                Naziv = kultura.Naziv,
                Hibrid = kultura.Hibrid,
                AktuelnaCena = kultura.AktuelnaCena,
                IdKorisnik = (Guid)kultura.IdKorisnik
            };
        }
    }
}
