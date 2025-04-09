using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class RadnjaExtension
    {
        public static RadnjaDTO ToRadnjaDTO(this Radnja radnja, List<RadnjaRadnaMasinaDTO>? radneMasine = null)
        {
            var dto = new RadnjaDTO()
            {
                Id = radnja.Id,
                IdParcela = radnja.IdParcela,
                IdKultura = radnja.IdKultura,
                DatumIzvrsenja = radnja.DatumIzvrsenja,
                VremenskiUslovi = radnja.VremenskiUslovi,
                Napomena = radnja.Napomena,
                UkupanTrosak = radnja.UkupanTrosak,
                TipRadnje = radnja.TipRadnje,
                Parcela = radnja.Parcela,
                Kultura = radnja.Kultura,
                RadneMasine = radneMasine ?? new List<RadnjaRadnaMasinaDTO>()
            };

            if (radnja is Zetva zetva)
            {
                dto.Prinos = zetva.Prinos;
            }

            return dto;
        }
    }
}
