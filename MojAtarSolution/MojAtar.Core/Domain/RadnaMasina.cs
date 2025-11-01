using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class RadnaMasina
    {
        [Key]
        public Guid? Id { get; set; }
        [StringLength(40,ErrorMessage =$"Duzina stringa mora biti manja od 40 karaktera")]
        public string Naziv { get; set; }
        [StringLength(40,ErrorMessage =$"Duzina stringa mora biti manja od 40 karaktera")]
        public string TipUlja { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Vrednost mora biti veca od 0.")]
        public int RadniSatiServis { get; set; }
        public DateTime PoslednjiServis { get; set; }
        [StringLength(500,ErrorMessage =$"Duzina stringa mora biti manja od 175 karaktera")]
        public string OpisServisa { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Vrednost mora biti veca od 0.")]
        public int UkupanBrojRadnihSati { get; set; }

        //Za filtriranje
        public Guid IdKorisnik { get; set; }

    }
}
