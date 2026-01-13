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
        public static RadnjaDTO ToRadnjaDTO(this Radnja radnja)
        {
            var dto = new RadnjaDTO()
            {
                Id = radnja.Id,
                // IZBAČENO: IdParcela = radnja.IdParcela, 
                IdKultura = radnja.IdKultura,
                DatumIzvrsenja = radnja.DatumIzvrsenja,
                Napomena = radnja.Napomena,
                UkupanTrosak = radnja.UkupanTrosak,
                TipRadnje = radnja.TipRadnje,
                // IZBAČENO: Parcela = radnja.Parcela, (jer Radnja nema jednu parcelu)
                Kultura = radnja.Kultura
            };

            // 1. Mapiranje Žetve (Prinos)
            if (radnja is Zetva zetva)
            {
                dto.Prinos = zetva.Prinos;
            }

            // 2. Mapiranje Veza: RadnjeParcele -> ParceleDTO
            if (radnja.RadnjeParcele != null && radnja.RadnjeParcele.Any())
            {
                dto.Parcele = radnja.RadnjeParcele.Select(rp => new RadnjaParcelaDTO
                {
                    IdParcela = rp.IdParcela,
                    Povrsina = rp.Povrsina,
                    NazivParcele = rp.Parcela?.Naziv // Ako je Parcela učitana (Include)
                }).ToList();

                // Automatski sračunaj ukupnu površinu na osnovu liste
                dto.UkupnaPovrsina = dto.Parcele.Sum(p => p.Povrsina);
            }

            // 3. Mapiranje mašina (ako su učitane)
            if (radnja.RadnjeRadneMasine != null)
            {
                dto.RadneMasine = radnja.RadnjeRadneMasine.Select(rm => new RadnjaRadnaMasinaDTO
                {
                    IdRadnja = rm.IdRadnja,
                    IdRadnaMasina = rm.IdRadnaMasina,
                    // Dodaj ostala polja ako ih imaš u DTO
                }).ToList();
            }

            if (radnja.RadnjePrikljucneMasine != null)
            {
                dto.PrikljucneMasine = radnja.RadnjePrikljucneMasine.Select(pm => new RadnjaPrikljucnaMasinaDTO
                {
                    IdRadnja = pm.IdRadnja,
                    IdPrikljucnaMasina = (Guid)pm.IdPrikljucnaMasina
                }).ToList();
            }

            if (radnja.RadnjeResursi != null)
            {
                dto.Resursi = radnja.RadnjeResursi.Select(r => new RadnjaResursDTO
                {
                    IdRadnja = r.IdRadnja,
                    IdResurs = r.IdResurs,
                    Kolicina = r.Kolicina,
                    DatumKoriscenja = r.DatumKoriscenja
                }).ToList();
            }

            return dto;
        }
    }
}
