using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class KulturaDTO
    {
        public Guid? Id { get; set; }
        public string Naziv { get; set; }
        public double AktuelnaCena { get; set; }
        public DateTime DatumVaznostiCene { get; set; }


        //Za filtriranje
        public Guid IdKorisnik { get; set; }

        public Kultura ToKultura() => new Kultura()
        {
            Id = Id,
            Naziv = Naziv,
            AktuelnaCena = AktuelnaCena,
            IdKorisnik = IdKorisnik
        };
    }
}
