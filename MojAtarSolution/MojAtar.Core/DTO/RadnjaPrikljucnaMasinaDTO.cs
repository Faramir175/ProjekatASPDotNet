using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class RadnjaPrikljucnaMasinaDTO
    {
        public Guid IdRadnja { get; set; }
        public Guid IdPrikljucnaMasina { get; set; }

        public Radnja_PrikljucnaMasina ToPrikljucnaMasina() => new Radnja_PrikljucnaMasina()
        {
            IdRadnja = IdRadnja,
            IdPrikljucnaMasina = IdPrikljucnaMasina,
        };
    }
}
