using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.DTO
{
    public class ParcelaKulturaDTO
    {
        public Guid? IdParcela { get; set; }
        public Guid? IdKultura { get; set; }
        public double Povrsina { get; set; }
        public DateTime DatumSetve { get; set; }
        public DateTime? DatumZetve { get; set; }

        //Za filtriranje
        public Guid IdKorisnik { get; set; }

        public Parcela_Kultura ToParcelaKultura() => new Parcela_Kultura()
        {
            IdKultura = IdKultura,
            IdParcela = IdParcela,
            Povrsina = Povrsina,
            DatumSetve = DatumSetve,
            DatumZetve = DatumZetve
        };
    }
}
