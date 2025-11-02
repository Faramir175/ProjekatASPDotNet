using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Resurs
    {
        [Key]
        public Guid? Id { get; set; }

        public string Naziv { get; set; }
        public string Vrsta { get; set; }
        public double AktuelnaCena { get; set; }

        public ICollection<CenaResursa> CeneResursa { get; set; }

        // Filtriranje
        public Guid IdKorisnik { get; set; }
    }
}
