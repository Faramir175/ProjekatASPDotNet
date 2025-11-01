using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class ParcelaDTO
    {
        public Guid? Id { get; set; }
        public string BrojParcele { get; set; }
        public string Naziv { get; set; }
        public decimal Povrsina { get; set; }
        public string? Napomena { get; set; }
        public Guid IdKatastarskaOpstina { get; set; }
        public Guid IdKorisnik { get; set; }
        public string? KatastarskaOpstinaNaziv { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<(string NazivKulture, decimal Povrsina)> AktivneKulture { get; set; } = new();

        public Parcela ToParcela() => new Parcela()
        {
            Id = Id,
            BrojParcele = BrojParcele,
            Naziv = Naziv,
            Povrsina = Povrsina,
            Napomena = Napomena,
            IdKatastarskaOpstina = IdKatastarskaOpstina,
            IdKorisnik = IdKorisnik,
            Latitude = Latitude,
            Longitude = Longitude
        };
    }
}
