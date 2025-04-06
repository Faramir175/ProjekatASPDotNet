using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class ResursDTO
    {
        public Guid? Id { get; set; }
        public string Naziv { get; set; }
        public string Vrsta { get; set; }
        public double AktuelnaCena { get; set; }

        //Za filtriranje
        public Guid IdKorisnik { get; set; }

        public Resurs ToResurs() => new Resurs()
        {
            Id = Id,
            Naziv = Naziv,
            Vrsta = Vrsta,
            AktuelnaCena = AktuelnaCena,
            IdKorisnik = IdKorisnik
        };
    }
}
