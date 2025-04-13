using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class ParcelaKulturaExtension
    {
        public static ParcelaKulturaDTO? ToParcelaKulturaDTO(this Parcela_Kultura parcelaKultura)
        {
            return new ParcelaKulturaDTO()
            {
                Id = parcelaKultura.Id,
                IdKultura = parcelaKultura.IdKultura,
                IdParcela = parcelaKultura.IdParcela,
                Povrsina = parcelaKultura.Povrsina,
                DatumSetve = parcelaKultura.DatumSetve,
                DatumZetve = parcelaKultura.DatumZetve
            };
        }
    }
}
