using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class KorisnikRequestDTO
    {
        public string? Ime { get; set; } 
        public string? Prezime { get; set; } 
        public string Email { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KorisnikTip? TipKorisnika { get; set; }
        public DateTime? DatumRegistracije { get; set; }
        public string Lozinka { get; set; }
        public ICollection<Parcela>? Parcele { get; set; }

        public Korisnik ToKorisnik() => new Korisnik()
        {
            Ime = Ime,
            Prezime = Prezime,
            Email = Email,
            TipKorisnika = (KorisnikTip)TipKorisnika,
            DatumRegistracije = (DateTime)DatumRegistracije,
            Lozinka = Lozinka,
        };
    }
}
