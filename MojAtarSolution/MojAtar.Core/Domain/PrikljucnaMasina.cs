using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class PrikljucnaMasina
    {
        [Key]
        public Guid? Id { get; set; }
        public string Naziv { get; set; }
        public string TipMasine { get; set; }
        public double SirinaObrade { get; set; }
        public DateTime PoslednjiServis { get; set; }
        public string OpisServisa { get; set; }

        public Guid IdKorisnik { get; set; }
    }
}
