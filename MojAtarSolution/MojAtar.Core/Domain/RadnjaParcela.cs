using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class RadnjaParcela
    {
        [Key]
        public Guid Id { get; set; }

        // Veza ka Radnji
        public Guid IdRadnja { get; set; }
        public Radnja Radnja { get; set; }

        // Veza ka Parceli
        public Guid IdParcela { get; set; }
        public Parcela Parcela { get; set; }

        // Koliko je TE parcele obrađeno u ovoj radnji?
        // (Npr. Parcela ima 5ha, ali smo u ovoj radnji posejali samo 3ha)
        public decimal Povrsina { get; set; }
    }
}
