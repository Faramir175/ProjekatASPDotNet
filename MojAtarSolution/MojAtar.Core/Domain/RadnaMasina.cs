using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class RadnaMasina
    {
        [Key]
        public Guid Id { get; set; }
        public string Naziv { get; set; }
        public string TipUlja { get; set; }
        public int RadniSatiServis { get; set; }
        public DateTime PoslednjiServis { get; set; }
        public string OpisServisa { get; set; }
        public int UkupanBrojRadnihSati { get; set; }

        //Za filtriranje
        public Guid IdKorisnik { get; set; }

    }
}
