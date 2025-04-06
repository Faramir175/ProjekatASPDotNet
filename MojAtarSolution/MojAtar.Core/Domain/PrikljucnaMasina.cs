using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class PrikljucnaMasina
    {
        [Key]
        public Guid? Id { get; set; }
        [StringLength(50,ErrorMessage =$"Duzina stringa mora biti manja od 50 karaktera")]
        public string Naziv { get; set; }
        public string TipMasine { get; set; }
        [Range(0.01,double.MaxValue,ErrorMessage = "Vrednost mora biti veca od 0.")]
        public double SirinaObrade { get; set; }
        public DateTime PoslednjiServis { get; set; }
        [StringLength(175,ErrorMessage =$"Duzina stringa mora biti manja od 175 karaktera")]
        public string OpisServisa { get; set; }

        //Za filtriranje
        public Guid IdKorisnik { get; set; }

    }
}
