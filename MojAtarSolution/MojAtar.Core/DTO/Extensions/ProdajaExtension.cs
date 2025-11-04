using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class ProdajaExtension
    {
        public static ProdajaDTO? ToProdajaDTO(this Prodaja prodaja)
        {
            if (prodaja == null) return null;

            return new ProdajaDTO
            {
                Id = prodaja.Id,
                IdKultura = prodaja.IdKultura,
                NazivKulture = prodaja.Kultura?.Naziv,
                Kolicina = prodaja.Kolicina,
                CenaPoJedinici = prodaja.CenaPoJedinici,
                DatumProdaje = prodaja.DatumProdaje,
                Napomena = prodaja.Napomena
            };
        }
    }
}
