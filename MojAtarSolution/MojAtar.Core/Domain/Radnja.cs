using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Radnja
    {
        [Key]
        public Guid? Id { get; set; }
        // Veza sa Parcelom
        public Guid? IdParcela { get; set; }
        public Parcela Parcela { get; set; }
        public DateTime DatumIzvrsenja { get; set; }
        public string? Napomena { get; set; }
        public double UkupanTrosak { get; set; }
        public RadnjaTip TipRadnje { get; set; }
        public Guid? IdKultura { get; set; } 
        public Kultura? Kultura { get; set; }

        public virtual ICollection<Radnja_RadnaMasina> RadnjeRadneMasine { get; set; } = new List<Radnja_RadnaMasina>();
        public virtual ICollection<Radnja_PrikljucnaMasina> RadnjePrikljucneMasine { get; set; } = new List<Radnja_PrikljucnaMasina>();
        public virtual ICollection<Radnja_Resurs> RadnjeResursi { get; set; } = new List<Radnja_Resurs>();
    }
}
