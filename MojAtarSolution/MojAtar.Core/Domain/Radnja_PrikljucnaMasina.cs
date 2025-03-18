using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Radnja_PrikljucnaMasina
    {
        public Guid? IdRadnja { get; set; }
        public Radnja Radnja { get; set; }

        public Guid? IdPrikljucnaMasina { get; set; }
        public PrikljucnaMasina PrikljucnaMasina { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Vrednost mora biti veca od 0.")]
        public int BrojRadnihSati { get; set; }

    }
}
