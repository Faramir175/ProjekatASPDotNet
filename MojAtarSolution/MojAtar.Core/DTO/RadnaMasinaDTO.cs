using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class RadnaMasinaDTO
    {
        public Guid Id { get; set; }

        [Display(Name = "Naziv radne mašine")]
        [Required(ErrorMessage = "Polje 'Naziv radne mašine' je obavezno.")]
        [StringLength(40, ErrorMessage = "Naziv mora imati manje od 40 karaktera.")]
        public string Naziv { get; set; }

        [Display(Name = "Tip ulja")]
        [Required(ErrorMessage = "Polje 'Tip ulja' je obavezno.")]
        [StringLength(40, ErrorMessage = "Tip ulja mora imati manje od 40 karaktera.")]
        public string TipUlja { get; set; }

        [Display(Name = "Radni sati do servisa")]
        [Required(ErrorMessage = "Polje 'Radni sati do servisa' je obavezno.")]
        [Range(1, int.MaxValue, ErrorMessage = "Broj sati mora biti veći od 0.")]
        public int? RadniSatiServis { get; set; }

        [Display(Name = "Poslednji servis")]
        [Required(ErrorMessage = "Polje 'Poslednji servis' je obavezno.")]
        public DateTime PoslednjiServis { get; set; } = DateTime.Now;

        [Display(Name = "Opis servisa")]
        [Required(ErrorMessage = "Polje 'Opis servisa' je obavezno.")]
        [StringLength(500, ErrorMessage = "Opis servisa mora imati manje od 500 karaktera.")]
        public string OpisServisa { get; set; }

        [Display(Name = "Ukupno radnih sati")]
        [Required(ErrorMessage = "Polje 'Ukupno radnih sati' je obavezno.")]
        [Range(1, int.MaxValue, ErrorMessage = "Broj sati mora biti veći od 0.")]
        public int? UkupanBrojRadnihSati { get; set; }

        public Guid IdKorisnik { get; set; }

        public RadnaMasina ToRadnaMasina() => new RadnaMasina()
        {
            Naziv = Naziv,
            TipUlja = TipUlja,
            RadniSatiServis = RadniSatiServis ?? 0,
            PoslednjiServis = PoslednjiServis,
            OpisServisa = OpisServisa,
            UkupanBrojRadnihSati = UkupanBrojRadnihSati ?? 0,
            IdKorisnik = IdKorisnik
        };
    }
}
