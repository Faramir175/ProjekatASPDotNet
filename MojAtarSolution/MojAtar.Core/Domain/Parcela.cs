using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Parcela
    {
        [Key]
        public Guid? Id { get; set; }
        [StringLength(20, ErrorMessage = $"Duzina stringa mora biti manja od 20 karaktera")]
        public string BrojParcele { get; set; }
        [StringLength(40, ErrorMessage = $"Duzina stringa mora biti manja od 40 karaktera")]
        public string Naziv { get; set; }
        [Range(0.0001, double.MaxValue, ErrorMessage = "Cena po jedinici mora biti veca od 0.")]
        [Precision(18, 4)]
        public decimal Povrsina { get; set; }
        [StringLength(175, ErrorMessage = $"Duzina stringa mora biti manja od 175 karaktera")]
        public string Napomena { get; set; }
        [Range(-90, 90)]
        public double? Latitude { get; set; }
        [Range(-180, 180)]
        public double? Longitude { get; set; }

        // Foreign Key
        public Guid? IdKatastarskaOpstina { get; set; }
        public KatastarskaOpstina KatastarskaOpstina { get; set; }

        public Guid? IdKorisnik { get; set; }
        public Korisnik Korisnik { get; set; }

        // Veze sa drugim tabelama
        public ICollection<Parcela_Kultura> ParceleKulture { get; set; }
        public ICollection<Radnja> Radnje { get; set; }
    }
}
