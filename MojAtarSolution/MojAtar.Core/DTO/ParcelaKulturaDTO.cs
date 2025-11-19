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
        public Guid? Id { get; set; }
        public Guid? IdParcela { get; set; }
        public Guid? IdKultura { get; set; }
        public decimal Povrsina { get; set; }
        public DateTime DatumSetve { get; set; }
        public DateTime? DatumZetve { get; set; }

        public Guid? IdSetvaRadnja { get; set; }
        public Guid? IdZetvaRadnja { get; set; }

        //Za filtriranje
        public Guid IdKorisnik { get; set; }

        public Parcela_Kultura ToParcelaKultura()
        {
            var entity = new Parcela_Kultura()
            {
                IdKultura = IdKultura!.Value,
                IdParcela = IdParcela!.Value,
                Povrsina = Povrsina,
                DatumSetve = DatumSetve,
                DatumZetve = DatumZetve,
                IdSetvaRadnja = IdSetvaRadnja,
                IdZetvaRadnja = IdZetvaRadnja
            };

            if (Id.HasValue)
                entity.Id = Id.Value;

            return entity;
        }

    }
}
