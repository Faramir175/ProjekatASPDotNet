using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Korisnik
    {
        [Key]
        public Guid? Id { get; set; }
        [StringLength(40,ErrorMessage =$"Duzina stringa mora biti manja od 40 karaktera")]
        public string Ime { get; set; }
        [StringLength(40)]
        public string Prezime { get; set; }
        [StringLength(40)]
        public string Email { get; set; }
        [StringLength(40)]
        public KorisnikTip TipKorisnika { get; set; }
        public DateTime DatumRegistracije { get; set; }
        [StringLength(100)]
        public string Lozinka { get; set; }
        public ICollection<Parcela> Parcele { get; set; }
    }
}
