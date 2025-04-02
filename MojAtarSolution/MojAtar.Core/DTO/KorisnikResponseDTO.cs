using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class KorisnikResponseDTO
    {
        public Guid? Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public KorisnikTip TipKorisnika { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public string Lozinka { get; set; }
        public ICollection<Parcela> Parcele { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj == null || obj.GetType()!=typeof(Korisnik)) return false;

            Korisnik? korisnikZaPoredjenje = obj as Korisnik;

            return Id == korisnikZaPoredjenje.Id && Email == korisnikZaPoredjenje.Email;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
