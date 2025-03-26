using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class KorisnikRequest
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public KorisnikTip TipKorisnika { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public string Lozinka { get; set; }
        public ICollection<Parcela> Parcele { get; set; }

        public Korisnik ToKorisnik()
        {
            return new Korisnik() {                 
                Ime = Ime,
                Prezime = Prezime,
                Email = Email,
                TipKorisnika = TipKorisnika,
                DatumRegistracije = DatumRegistracije,
                Lozinka = Lozinka,
                Parcele = Parcele };
        }
    }
}
