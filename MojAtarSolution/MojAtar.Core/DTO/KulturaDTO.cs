using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class KulturaDTO
    {
        public Guid? Id { get; set; }

        [Display(Name = "Naziv kulture")]
        [Required(ErrorMessage = "Polje 'Naziv' je obavezno.")]
        [StringLength(40, ErrorMessage = "Naziv mora imati manje od 40 karaktera.")]
        public string Naziv { get; set; }

        [Display(Name = "Aktuelna cena (RSD/kg)")]
        [Required(ErrorMessage = "Polje 'Aktuelna cena' je obavezno.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena mora biti veća od 0.")]
        public double? AktuelnaCena { get; set; }

        [Display(Name = "Datum važenja cene")]
        [Required(ErrorMessage = "Datum važenja cene je obavezan.")]
        public DateTime DatumVaznostiCene { get; set; } = DateTime.Now;
        [Range(0, 999999999.99, ErrorMessage = "Količina raspoloživa za prodaju mora biti veća od 0.")]
        public decimal RaspolozivoZaProdaju { get; set; }

        public Guid IdKorisnik { get; set; }

        public Kultura ToKultura() => new Kultura()
        {
            Id = Id,
            Naziv = Naziv,
            AktuelnaCena = AktuelnaCena ?? 0,
            IdKorisnik = IdKorisnik,
            RaspolozivoZaProdaju = RaspolozivoZaProdaju
        };
    }
}
