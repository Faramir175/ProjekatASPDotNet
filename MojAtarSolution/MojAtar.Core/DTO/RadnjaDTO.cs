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
        public Guid? IdParcela { get; set; }

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
        public decimal? Povrsina { get; set; }
        public Parcela? Parcela { get; set; }
        public Kultura? Kultura { get; set; }

        public List<RadnjaRadnaMasinaDTO> RadneMasine { get; set; } = new();
        public List<RadnjaPrikljucnaMasinaDTO> PrikljucneMasine { get; set; } = new();
        public List<RadnjaResursDTO> Resursi { get; set; } = new();
        public List<Guid>? ObrisaneRadneMasineId { get; set; }
        public List<Guid>? ObrisanePrikljucneMasineId { get; set; }
        public List<Guid>? ObrisaniResursiId { get; set; }


        public Radnja ToRadnja()
        {
            if (this.TipRadnje == RadnjaTip.Zetva)
            {
                return new Zetva()
                {
                    Id = this.Id ?? Guid.NewGuid(),
                    IdParcela = (Guid)this.IdParcela,
                    IdKultura = this.IdKultura,
                    DatumIzvrsenja = this.DatumIzvrsenja,
                    Napomena = this.Napomena,
                    UkupanTrosak = this.UkupanTrosak,
                    TipRadnje = this.TipRadnje,
                    Prinos = this.Prinos ?? 0,
                    Parcela = this.Parcela,
                    Kultura = this.Kultura
                };
            }
            else
            {
                return new Radnja()
                {
                    Id = this.Id ?? Guid.NewGuid(),
                    IdParcela = (Guid)this.IdParcela,
                    IdKultura = this.IdKultura,
                    DatumIzvrsenja = this.DatumIzvrsenja,
                    Napomena = this.Napomena,
                    UkupanTrosak = this.UkupanTrosak,
                    TipRadnje = this.TipRadnje,
                    Parcela = this.Parcela,
                    Kultura = this.Kultura
                };
            }
        }
    }
}
