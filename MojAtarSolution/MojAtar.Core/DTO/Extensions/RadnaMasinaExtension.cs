using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class RadnaMasinaExtension
    {
        public static RadnaMasinaDTO? ToRadnaMasinaDTO(this RadnaMasina radnaMasina)
        {
            return new RadnaMasinaDTO()
            {
                Id = radnaMasina.Id,
                Naziv = radnaMasina.Naziv,
                TipUlja = radnaMasina.TipUlja,
                RadniSatiServis = radnaMasina.RadniSatiServis,
                PoslednjiServis = radnaMasina.PoslednjiServis,
                OpisServisa = radnaMasina.OpisServisa,
                UkupanBrojRadnihSati = radnaMasina.UkupanBrojRadnihSati,
                IdKorisnik = (Guid)radnaMasina.IdKorisnik
            };
        }
    }
}
