using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class PrikljucnaMasinaDTO
    {
        public Guid? Id { get; set; }
        public string Naziv { get; set; }
        public string TipMasine { get; set; }
        public double SirinaObrade { get; set; }
        public DateTime PoslednjiServis { get; set; }
        public string OpisServisa { get; set; }
        public Guid IdKorisnik { get; set; }

        public PrikljucnaMasina ToPrikljucnaMasina() => new PrikljucnaMasina()
        {
            Id = Id,
            Naziv = Naziv,
            TipMasine = TipMasine,
            SirinaObrade = SirinaObrade,
            PoslednjiServis = PoslednjiServis,
            OpisServisa = OpisServisa,
            IdKorisnik = IdKorisnik
        };
    }
}
