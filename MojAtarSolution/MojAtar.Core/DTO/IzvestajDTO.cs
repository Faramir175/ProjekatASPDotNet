namespace MojAtar.Core.DTO
{
    public class IzvestajDTO
    {
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; } = DateTime.Now;
        public List<ParcelaIzvestajDTO> Parcele { get; set; } = new();

        public double UkupanTrosak => Parcele.Sum(p => p.Trosak);
        public double UkupanPrihod => Parcele.Sum(p => p.Prihod);
        public double Profit => UkupanPrihod - UkupanTrosak;
    }

    public class ParcelaIzvestajDTO
    {
        public Guid Id { get; set; }
        public string NazivParcele { get; set; } = string.Empty;
        public List<RadnjaIzvestajDTO> Radnje { get; set; } = new();

        public double Trosak => Radnje.Sum(r => r.Trosak);
        public double Prihod => Radnje.Sum(r => r.Prihod);
    }

    public class RadnjaIzvestajDTO
    {
        public Guid Id { get; set; }
        public string NazivRadnje { get; set; } = string.Empty;
        public DateTime Datum { get; set; } = DateTime.Now;
        public string Kultura { get; set; } = string.Empty;
        public double Trosak { get; set; }
        public double Prihod { get; set; }

        public List<RadnjaRadnaMasinaDTO> RadneMasine { get; set; } = new();
        public List<RadnjaResursDTO> Resursi { get; set; } = new();
    }
}
