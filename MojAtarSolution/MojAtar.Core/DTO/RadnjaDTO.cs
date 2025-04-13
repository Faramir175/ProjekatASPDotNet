using MojAtar.Core.Domain.Enums;
using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class RadnjaDTO
    {
        public Guid? Id { get; set; }
        public Guid? IdParcela { get; set; }
        public Guid? IdKultura { get; set; }
        public DateTime DatumIzvrsenja { get; set; }
        public string? VremenskiUslovi { get; set; }
        public string? Napomena { get; set; }
        public double UkupanTrosak { get; set; }
        public RadnjaTip TipRadnje { get; set; }
        public Parcela? Parcela { get; set; }
        public Kultura? Kultura { get; set; }

        // Dodatno ako je zetva
        public double? Prinos { get; set; }
        //Za tabelu ParceleKulture
        public double? Povrsina { get; set; }

        public List<RadnjaRadnaMasinaDTO> RadneMasine { get; set; } = new();
        public List<RadnjaPrikljucnaMasinaDTO> PrikljucneMasine { get; set; } = new();
        public List<RadnjaResursDTO> Resursi { get; set; } = new();

        public Radnja ToRadnja()
        {
            if (this.TipRadnje == RadnjaTip.Zetva)
            {
                return new Zetva()
                {
                    Id = this.Id ?? Guid.NewGuid(),
                    IdParcela = this.IdParcela,
                    IdKultura = this.IdKultura,
                    DatumIzvrsenja = this.DatumIzvrsenja,
                    VremenskiUslovi = this.VremenskiUslovi,
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
                    IdParcela = this.IdParcela,
                    IdKultura = this.IdKultura,
                    DatumIzvrsenja = this.DatumIzvrsenja,
                    VremenskiUslovi = this.VremenskiUslovi,
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
