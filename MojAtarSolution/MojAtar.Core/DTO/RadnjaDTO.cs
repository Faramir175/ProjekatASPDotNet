using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class RadnjaDTO
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Morate izabrati parcelu.")]
        //public Guid? IdParcela { get; set; }
        public List<RadnjaParcelaDTO> Parcele { get; set; } = new();
        public Guid? IdKultura { get; set; }

        [Required(ErrorMessage = "Morate uneti datum izvršenja.")]
        [DataType(DataType.Date)]
        public DateTime DatumIzvrsenja { get; set; } = DateTime.Now;

        [StringLength(500, ErrorMessage = "Napomena može sadržati najviše 500 karaktera.")]
        public string? Napomena { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ukupan trošak ne može biti negativan.")]
        public double UkupanTrosak { get; set; }

        [Required(ErrorMessage = "Morate izabrati tip radnje.")]
        public RadnjaTip TipRadnje { get; set; }

        public double? Prinos { get; set; }

        [Range(0.0001, 10000, ErrorMessage = "Površina mora biti veća od 0.")]
        public decimal? UkupnaPovrsina { get; set; }
        public Kultura? Kultura { get; set; }

        public List<RadnjaRadnaMasinaDTO> RadneMasine { get; set; } = new();
        public List<RadnjaPrikljucnaMasinaDTO> PrikljucneMasine { get; set; } = new();
        public List<RadnjaResursDTO> Resursi { get; set; } = new();
        public List<Guid>? ObrisaneRadneMasineId { get; set; }
        public List<Guid>? ObrisanePrikljucneMasineId { get; set; }
        public List<Guid>? ObrisaniResursiId { get; set; }


        public Radnja ToRadnja()
        {
            // Napomena: Ovde više ne mapiramo Parcele jer se one moraju dodati
            // kroz servise kao child entiteti u tabelu RadnjeParcele.

            Radnja radnja;

            if (this.TipRadnje == RadnjaTip.Zetva)
            {
                radnja = new Zetva()
                {
                    Id = this.Id ?? Guid.NewGuid(),
                    Prinos = this.Prinos ?? 0
                };
            }
            else
            {
                radnja = new Radnja()
                {
                    Id = this.Id ?? Guid.NewGuid()
                };
            }

            // Zajednička polja
            radnja.IdKultura = this.IdKultura;
            radnja.DatumIzvrsenja = this.DatumIzvrsenja;
            radnja.Napomena = this.Napomena;
            radnja.UkupanTrosak = this.UkupanTrosak;
            radnja.TipRadnje = this.TipRadnje;
            radnja.Kultura = this.Kultura;

            return radnja;
        }
    }
}
