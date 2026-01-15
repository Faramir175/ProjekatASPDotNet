using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Parcela
    {
        [Key]
        public Guid? Id { get; set; }
        public string BrojParcele { get; set; }
        public string Naziv { get; set; }
        [Precision(18, 4)]
        public decimal Povrsina { get; set; }
        public string? Napomena { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Foreign Key
        public Guid IdKatastarskaOpstina { get; set; }
        public KatastarskaOpstina KatastarskaOpstina { get; set; }

        public Guid IdKorisnik { get; set; }
        public Korisnik Korisnik { get; set; }

        // Veze sa drugim tabelama
        public ICollection<Parcela_Kultura> ParceleKulture { get; set; }
        public virtual ICollection<RadnjaParcela> RadnjeParcele { get; set; } = new List<RadnjaParcela>();
    }
}
