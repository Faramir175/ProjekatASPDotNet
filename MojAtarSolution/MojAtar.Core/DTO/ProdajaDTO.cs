using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class ProdajaDTO
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Morate izabrati kulturu.")]
        public Guid IdKultura { get; set; }

        [Display(Name = "Naziv kulture")]
        public string? NazivKulture { get; set; }

        [Display(Name = "Količina (kg)")]
        [Required(ErrorMessage = "Polje 'Količina' je obavezno.")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Količina mora biti veća od 0.")]
        public decimal? Kolicina { get; set; }

        [Display(Name = "Cena po jedinici (RSD/kg)")]
        [Required(ErrorMessage = "Polje 'Cena' je obavezno.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Cena ne može biti negativna.")]
        public decimal? CenaPoJedinici { get; set; }

        [Display(Name = "Datum prodaje")]
        [Required(ErrorMessage = "Datum prodaje je obavezan.")]
        public DateTime DatumProdaje { get; set; } = DateTime.Now;

        [Display(Name = "Napomena")]
        [StringLength(200, ErrorMessage = "Napomena može imati najviše 200 karaktera.")]
        public string? Napomena { get; set; }

        [Display(Name = "Ukupan iznos (RSD)")]
        public decimal UkupanIznos => (Kolicina ?? 0) * (CenaPoJedinici ?? 0);

        public Prodaja ToProdaja() => new Prodaja
        {
            Id = Id ?? Guid.NewGuid(),
            IdKultura = IdKultura,
            Kolicina = Kolicina ?? 0,
            CenaPoJedinici = CenaPoJedinici ?? 0,
            DatumProdaje = DatumProdaje,
            Napomena = Napomena
        };
    }
}
