using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class RadnjaParcela
    {
        [Key]
        public Guid Id { get; set; }

        public Guid IdRadnja { get; set; }

        [ForeignKey("IdRadnja")] // Ovde treba da stoji, pokazuje na IdRadnja
        public virtual Radnja Radnja { get; set; }

        public Guid IdParcela { get; set; }

        [ForeignKey("IdParcela")] // Ovde treba da stoji, pokazuje na IdParcela
        public virtual Parcela Parcela { get; set; }

        public decimal Povrsina { get; set; }
    }
}
