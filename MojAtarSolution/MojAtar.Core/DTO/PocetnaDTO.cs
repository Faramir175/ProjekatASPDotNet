using MojAtar.Core.DTO;
namespace MojAtar.Core.DTO
{
    public class PocetnaDTO
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Uloga { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public int BrojParcela { get; set; }
        public int BrojRadnji { get; set; }
        public int BrojResursa { get; set; }
        public int BrojRadnihMasina { get; set; }
        public int BrojPrikljucnihMasina { get; set; }
        public int BrojKultura { get; set; }
        public List<RadnjaDTO> PoslednjeRadnje { get; set; } = new();

    }
}
