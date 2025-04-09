using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class RadnjaRadnaMasinaDTO
    {
        public Guid IdRadnja { get; set; }
        public Guid IdRadnaMasina { get; set; }
        public int BrojRadnihSati { get; set; }

        public Radnja_RadnaMasina ToRadnaMasina() => new Radnja_RadnaMasina()
        {
            IdRadnja = IdRadnja,
            IdRadnaMasina = IdRadnaMasina,
            BrojRadnihSati = BrojRadnihSati
        };
    }
}
