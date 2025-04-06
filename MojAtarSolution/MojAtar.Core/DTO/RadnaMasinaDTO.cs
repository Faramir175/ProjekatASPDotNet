﻿using MojAtar.Core.Domain;
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
        public Guid? Id { get; set; }
        public string Naziv { get; set; }
        public string TipUlja { get; set; }
        public int RadniSatiServis { get; set; }
        public DateTime PoslednjiServis { get; set; }
        public string OpisServisa { get; set; }
        public int UkupanBrojRadnihSati { get; set; }
        public Guid IdKorisnik { get; set; }

        public RadnaMasina ToRadnaMasina() => new RadnaMasina()
        {
            Id = Id,
            Naziv = Naziv,
            TipUlja = TipUlja,
            RadniSatiServis = RadniSatiServis,
            PoslednjiServis = PoslednjiServis,
            OpisServisa = OpisServisa,
            UkupanBrojRadnihSati = UkupanBrojRadnihSati,
            IdKorisnik = IdKorisnik
        };
    }
}
