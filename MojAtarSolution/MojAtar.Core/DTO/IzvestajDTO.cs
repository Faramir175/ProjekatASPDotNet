namespace MojAtar.Core.DTO
{
    public class IzvestajDTO
    {
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; } = DateTime.Now;
        public List<ParcelaIzvestajDTO> Parcele { get; set; } = new();

        public decimal UkupanPrinos => Parcele.Sum(p => p.Prinos);
        public decimal UkupanTrosak => Parcele.Sum(p => p.Trosak);
        public decimal UkupanPrihodIzProdaja { get; set; } = 0;
        public decimal UkupanPrihod => UkupanPrihodIzProdaja;
        public decimal Profit => UkupanPrihodIzProdaja - UkupanTrosak;
    }

    public class ParcelaIzvestajDTO
    {
        public Guid Id { get; set; }
        public string NazivParcele { get; set; } = string.Empty;
        public List<RadnjaIzvestajDTO> Radnje { get; set; } = new();

        public decimal Trosak => Radnje.Sum(r => r.Trosak);
        public decimal Prinos => Radnje.Sum(r => r.Prinos);
    }

    public class RadnjaIzvestajDTO
    {
        public Guid Id { get; set; }
        public string NazivRadnje { get; set; } = string.Empty;
        public DateTime Datum { get; set; } = DateTime.Now;
        public string Kultura { get; set; } = string.Empty;
        public Guid? IdKultura { get; set; }
        public decimal Trosak { get; set; }
        public decimal Prihod { get; set; }
        public decimal Prinos { get; set; }
        public decimal UkupanPrinosKulture { get; set; }
        public decimal Ostalo => UkupanPrinosKulture - Prinos;


        public List<RadnjaRadnaMasinaDTO> RadneMasine { get; set; } = new();
        public List<RadnjaResursDTO> Resursi { get; set; } = new();
    }

    public class IzvestajProdajeResult
    {
        public ParcelaIzvestajDTO ParcelaProdaje { get; set; } = new();
        public decimal UkupanPrihod { get; set; }
    }
}
