using MojAtar.Core.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace MojAtar.Core.DTO
{
    public class ResursDTO
    {
        public Guid? Id { get; set; }

        [Display(Name = "Naziv resursa")]
        [Required(ErrorMessage = "Polje 'Naziv' je obavezno.")]
        [StringLength(40, ErrorMessage = "Naziv mora imati manje od 40 karaktera.")]
        public string Naziv { get; set; }

        [Display(Name = "Vrsta resursa")]
        [Required(ErrorMessage = "Polje 'Vrsta' je obavezno.")]
        [StringLength(40, ErrorMessage = "Vrsta mora imati manje od 40 karaktera.")]
        public string Vrsta { get; set; }

        [Display(Name = "Aktuelna cena po jedinici")]
        [Required(ErrorMessage = "Polje 'Aktuelna cena' je obavezno.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena mora biti veća od 0.")]
        public double? AktuelnaCena { get; set; }

        [Display(Name = "Datum važenja cene")]
        [Required(ErrorMessage = "Datum važenja cene je obavezan.")]
        public DateTime DatumVaznostiCene { get; set; } = DateTime.Now;

        public Guid IdKorisnik { get; set; }

        public Resurs ToResurs() => new Resurs()
        {
            Id = Id,
            Naziv = Naziv,
            Vrsta = Vrsta,
            AktuelnaCena = AktuelnaCena ?? 0,
            IdKorisnik = IdKorisnik
        };
    }
}
