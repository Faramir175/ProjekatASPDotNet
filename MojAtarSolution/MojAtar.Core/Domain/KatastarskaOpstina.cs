using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class KatastarskaOpstina
    {
        [Key]
        public Guid? Id { get; set; }
        [StringLength(40,ErrorMessage =$"Duzina stringa mora biti manja od 40 karaktera")]
        public string Naziv { get; set; }
        [StringLength(100,ErrorMessage =$"Duzina stringa mora biti manja od 40 karaktera")]
        public string GradskaOpstina { get; set; }

        // Veza 1:N sa Parcelama
        public ICollection<Parcela> Parcele { get; set; }
    }
}
