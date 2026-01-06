using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class RadnjaParcelaDTO
    {
        public Guid IdParcela { get; set; }
        public string? NazivParcele { get; set; } // Korisno za prikaz

        [Range(0.0001, 10000, ErrorMessage = "Površina mora biti veća od 0.")]
        public decimal Povrsina { get; set; }
    }
}
