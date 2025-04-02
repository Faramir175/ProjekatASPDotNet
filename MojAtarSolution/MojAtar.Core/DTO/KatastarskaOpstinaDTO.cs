using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class KatastarskaOpstinaDTO
    {
        public Guid? Id { get; set; } // Opcioni Id, može biti null kod kreiranja
        [Required]
        [StringLength(40, ErrorMessage = "Dužina stringa mora biti manja od 40 karaktera")]
        public string Naziv { get; set; }

        [StringLength(100, ErrorMessage = "Dužina stringa mora biti manja od 100 karaktera")]
        public string GradskaOpstina { get; set; }
        public ICollection<Parcela>? Parcele { get; set; }


        public KatastarskaOpstina ToKatastarskaOpstina() => new KatastarskaOpstina()
        {
            Id = Id,
            GradskaOpstina = GradskaOpstina,
            Naziv = Naziv,
            Parcele = Parcele,
        };
    }

}
