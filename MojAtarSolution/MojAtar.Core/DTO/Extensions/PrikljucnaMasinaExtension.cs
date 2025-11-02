using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.Extensions
{
    public static class PrikljucnaMasinaExtension
    {
        public static PrikljucnaMasinaDTO? ToPrikljucnaMasinaDTO(this PrikljucnaMasina prikljucnaMasina)
        {
            return new PrikljucnaMasinaDTO()
            {
                Id = (Guid)prikljucnaMasina.Id,
                Naziv = prikljucnaMasina.Naziv,
                TipMasine = prikljucnaMasina.TipMasine,
                SirinaObrade = prikljucnaMasina.SirinaObrade,
                PoslednjiServis = prikljucnaMasina.PoslednjiServis,
                OpisServisa = prikljucnaMasina.OpisServisa,
                IdKorisnik = (Guid)prikljucnaMasina.IdKorisnik
            };
        }
    }
}
