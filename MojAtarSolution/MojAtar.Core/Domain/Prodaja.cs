using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Prodaja
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Kultura))]
        public Guid IdKultura { get; set; }
        public Kultura Kultura { get; set; }
        [Precision(18, 4)]
        public decimal Kolicina { get; set; }
        [Precision(18, 4)]
        public decimal CenaPoJedinici { get; set; }
        [Required]
        public DateTime DatumProdaje { get; set; } = DateTime.Now;
        public string? Napomena { get; set; }
    }
}
