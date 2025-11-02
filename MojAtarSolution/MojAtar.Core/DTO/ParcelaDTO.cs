using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MojAtar.Core.DTO
{
    public class ParcelaDTO
    {
        public Guid? Id { get; set; }

        [Display(Name = "Naziv")]
        [Required(ErrorMessage = "Polje 'Naziv' je obavezno.")]
        [StringLength(40, ErrorMessage = "Naziv mora imati manje od 40 karaktera.")]
        public string Naziv { get; set; }

        [Display(Name = "Broj parcele")]
        [Required(ErrorMessage = "Polje 'Broj parcele' je obavezno.")]
        [StringLength(20, ErrorMessage = "Broj parcele mora imati manje od 20 karaktera.")]
        public string BrojParcele { get; set; }

        [Display(Name = "Površina (ha)")]
        [Required(ErrorMessage = "Polje 'Površina' je obavezno.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Površina mora biti veća od 0.")]
        public decimal? Povrsina { get; set; }

        [Display(Name = "Napomena")]
        [StringLength(175, ErrorMessage = "Napomena mora imati manje od 175 karaktera.")]
        public string? Napomena { get; set; }

        [Display(Name = "Katastarska opština")]
        [Required(ErrorMessage = "Polje 'Katastarska opština' je obavezno.")]
        public Guid? IdKatastarskaOpstina { get; set; }

        public Guid IdKorisnik { get; set; }
        public string? KatastarskaOpstinaNaziv { get; set; }

        [Display(Name = "Latitude")]
        [Range(-90, 90, ErrorMessage = "Latitude mora biti između -90 i 90.")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        [Range(-180, 180, ErrorMessage = "Longitude mora biti između -180 i 180.")]
        public double? Longitude { get; set; }

        public List<(string NazivKulture, decimal Povrsina)> AktivneKulture { get; set; } = new();

        public Parcela ToParcela() => new Parcela()
        {
            Id = Id,
            BrojParcele = BrojParcele,
            Naziv = Naziv,
            Povrsina = (decimal)Povrsina,
            Napomena = Napomena,
            IdKatastarskaOpstina = IdKatastarskaOpstina,
            IdKorisnik = IdKorisnik,
            Latitude = Latitude,
            Longitude = Longitude
        };
    }
}
