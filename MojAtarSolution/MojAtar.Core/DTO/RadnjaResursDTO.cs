using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class RadnjaResursDTO
    {
        public Guid IdRadnja { get; set; }
        public Guid IdResurs { get; set; }
        public string NazivResursa { get; set; } = string.Empty;
        public double Kolicina { get; set; }
        public DateTime DatumKoriscenja { get; set; } = DateTime.Now;
        public double JedinicnaCena { get; set; } = 0;

        public Radnja_Resurs ToResurs() => new Radnja_Resurs()
        {
            IdRadnja = IdRadnja,
            IdResurs = IdResurs,
            Kolicina = Kolicina,
            DatumKoriscenja = DatumKoriscenja
        };
    }
}
