using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.Enums
{
    public enum RadnjaTip
    {
        [Display(Name = "Oranje")]
        Oranje,

        [Display(Name = "Priprema")]
        Priprema,

        [Display(Name = "Đubrenje")]
        Djubrenje,

        [Display(Name = "Setva")]
        Setva,

        [Display(Name = "Prihrana")]
        Prehrana,

        [Display(Name = "Zaštita od bolesti")]
        ZastitaOdBolesti,

        [Display(Name = "Zaštita od korova")]
        ZastitaOdKorova,

        [Display(Name = "Žetva")]
        Zetva
    }
}
