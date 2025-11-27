using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class RadnjaExtension
    {
        public static RadnjaDTO ToRadnjaDTO(this Radnja radnja, List<RadnjaRadnaMasinaDTO>? radneMasine = null, decimal? povrsina = null)
        {
            var dto = new RadnjaDTO()
            {
                Id = radnja.Id,
                IdParcela = radnja.IdParcela,
                IdKultura = radnja.IdKultura,
                DatumIzvrsenja = radnja.DatumIzvrsenja,
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
            if (radnja.TipRadnje == RadnjaTip.Setva)
            {
                dto.Povrsina = povrsina;
            }

            return dto;
        }
    }
}
