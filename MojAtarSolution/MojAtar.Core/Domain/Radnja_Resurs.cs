using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Radnja_Resurs
    {
        [Required]
        public Guid IdRadnja { get; set; }
        public Radnja Radnja { get; set; }

        [Required]
        public Guid IdResurs { get; set; }
        public Resurs Resurs { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Vrednost mora biti veca od 0.")]
        public double Kolicina { get; set; }
        public DateTime DatumKoriscenja { get; set; }
    }
}
