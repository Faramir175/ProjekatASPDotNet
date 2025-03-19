using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Kultura
    {
        [Key]
        public Guid? Id { get; set; }
        [StringLength(40,ErrorMessage =$"Duzina stringa mora biti manja od 40 karaktera")]
        public string Naziv { get; set; }
        [StringLength(100,ErrorMessage =$"Duzina stringa mora biti manja od 100 karaktera")]
        public string Hibrid { get; set; }
        [Range(0.01,double.MaxValue,ErrorMessage = "Cena po jedinici mora biti veca od 0.")]
        public double AktuelnaCena { get; set; }
        public ICollection<Zetva> Zetve { get; set; }
        public ICollection<CenaKulture> CeneKulture { get; set; }

        public ICollection<Parcela_Kultura> ParceleKulture { get; set; }
    }
}
