using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class PrikljucnaMasinaDTO
    {
        public Guid Id { get; set; }

        [Display(Name = "Naziv")]
        [Required(ErrorMessage = "Polje 'Naziv' je obavezno.")]
        [StringLength(50, ErrorMessage = "Naziv mora imati manje od 50 karaktera.")]
        public string Naziv { get; set; }

        [Display(Name = "Tip mašine")]
        [Required(ErrorMessage = "Polje 'Tip mašine' je obavezno.")]
        [StringLength(50, ErrorMessage = "Tip mašine mora imati manje od 50 karaktera.")]
        public string TipMasine { get; set; }

        [Display(Name = "Širina obrade (m)")]
        [Required(ErrorMessage = "Polje 'Širina obrade' je obavezno.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Vrednost mora biti veća od 0.")]
        public double? SirinaObrade { get; set; }

        [Display(Name = "Datum poslednjeg servisa")]
        [Required(ErrorMessage = "Polje 'Datum poslednjeg servisa' je obavezno.")]
        public DateTime PoslednjiServis { get; set; } = DateTime.Now;

        [Display(Name = "Opis servisa")]
        [Required(ErrorMessage = "Polje 'Opis servisa' je obavezno.")]
        [StringLength(500, ErrorMessage = "Opis mora imati manje od 500 karaktera.")]
        public string? OpisServisa { get; set; }

        public Guid IdKorisnik { get; set; }

        public PrikljucnaMasina ToPrikljucnaMasina() => new PrikljucnaMasina()
        {
            Naziv = Naziv,
            TipMasine = TipMasine,
            SirinaObrade = SirinaObrade ?? 0,
            PoslednjiServis = PoslednjiServis,
            OpisServisa = OpisServisa,
            IdKorisnik = IdKorisnik
        };
    }
}
