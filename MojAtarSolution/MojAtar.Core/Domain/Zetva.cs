﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain
{
    public class Zetva:Radnja
    {
        // Veza sa Kulturom
        public Guid? IdKultura { get; set; }
        public Kultura Kultura { get; set; }

        [Range(0.01,double.MaxValue,ErrorMessage = "Cena po jedinici mora biti veca od 0.")]
        public double Prinos { get; set; } // u tonama

    }
}
