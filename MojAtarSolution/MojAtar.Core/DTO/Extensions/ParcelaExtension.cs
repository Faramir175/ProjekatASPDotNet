using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class ParcelaExtension
    {
        public static ParcelaDTO? ToParcelaDTO(this Parcela parcela)
        {
            return new ParcelaDTO()
            {
                Id = parcela.Id,
                BrojParcele = parcela.BrojParcele,
                Naziv = parcela.Naziv,
                Povrsina = parcela.Povrsina,
                Napomena = parcela.Napomena,
                IdKatastarskaOpstina = (Guid)parcela.IdKatastarskaOpstina,
                IdKorisnik = (Guid)parcela.IdKorisnik,
                Latitude = parcela.Latitude,
                Longitude = parcela.Longitude
            };
        }
    }
}
