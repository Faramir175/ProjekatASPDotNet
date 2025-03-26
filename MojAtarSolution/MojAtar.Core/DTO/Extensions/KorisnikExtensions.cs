using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO.ExtensionKlase
{
    public static class KorisnikExtensions
    {
        public static KorisnikResponse? ToKorisnikResponse(this Korisnik korisnik)
        {
            if (korisnik == null)
            {
                return null; 
            }

            return new KorisnikResponse()
            {
                Id = korisnik.Id,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email,
                TipKorisnika = korisnik.TipKorisnika,
                DatumRegistracije = korisnik.DatumRegistracije,
                Lozinka = korisnik.Lozinka,
                Parcele = korisnik.Parcele
            };
        }
    }
}
