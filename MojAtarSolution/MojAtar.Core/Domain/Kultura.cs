using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Kultura
    {
        [Key]
        public Guid? Id { get; set; }

        public string Naziv { get; set; }
        public double AktuelnaCena { get; set; }
        [Precision(18, 4)]
        public decimal RaspolozivoZaProdaju { get; set; }

        public ICollection<Radnja> Radnje { get; set; }
        public ICollection<CenaKulture> CeneKulture { get; set; }
        public ICollection<Parcela_Kultura> ParceleKulture { get; set; }
        public ICollection<Prodaja>? Prodaje { get; set; }

        // Filtriranje
        public Guid IdKorisnik { get; set; }
    }
}
