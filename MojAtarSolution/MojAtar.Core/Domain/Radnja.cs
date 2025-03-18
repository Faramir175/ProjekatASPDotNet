using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Radnja
    {
        [Key]
        public Guid? Id { get; set; }

        // Veza sa Parcelom
        public Guid? IdParcela { get; set; }
        public Parcela Parcela { get; set; }

        [StringLength(40,ErrorMessage =$"Duzina stringa mora biti manja od 40 karaktera")]
        public string TipRadnje { get; set; }
        public DateTime DatumIzvrsenja { get; set; }
        [StringLength(40,ErrorMessage =$"Duzina stringa mora biti manja od 40 karaktera")]
        public string VremenskiUslovi { get; set; }
        [StringLength(175,ErrorMessage =$"Duzina stringa mora biti manja od 175 karaktera")]
        public string Napomena { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Vrednost mora biti veca od 0.")]
        public double UkupanTrosak { get; set; }
    }
}
