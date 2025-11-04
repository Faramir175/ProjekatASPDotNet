using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Infrastructure.MojAtar;

namespace MojAtar.Infrastructure.Repositories
{
    public class IzvestajRepository : IIzvestajRepository
    {
        private readonly MojAtarDbContext _dbContext;

        public IzvestajRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ParcelaIzvestajDTO>> GetIzvestaj(
            Guid korisnikId,
            DateTime? odDatuma,
            DateTime? doDatuma,
            Guid? idParcele,
            bool sveParcele)
        {
            var query = _dbContext.Parcele
                .Include(p => p.Radnje)
                    .ThenInclude(r => r.Kultura)
                .Include(p => p.Radnje)
                    .ThenInclude(r => r.RadnjeRadneMasine)
                        .ThenInclude(rm => rm.RadnaMasina)
                .Include(p => p.Radnje)
                    .ThenInclude(r => r.RadnjeResursi)
                        .ThenInclude(rr => rr.Resurs)
                .Where(p => p.IdKorisnik == korisnikId);

            if (!sveParcele && idParcele.HasValue)
                query = query.Where(p => p.Id == idParcele);

            var parcele = await query.ToListAsync();
            var rezultat = new List<ParcelaIzvestajDTO>();

            foreach (var parcela in parcele)
            {
                var parcelaDto = new ParcelaIzvestajDTO
                {
                    Id = parcela.Id.Value,
                    NazivParcele = parcela.Naziv,
                };

                var radnje = parcela.Radnje
                    .Where(r => (!odDatuma.HasValue || r.DatumIzvrsenja >= odDatuma)
                             && (!doDatuma.HasValue || r.DatumIzvrsenja <= doDatuma))
                    .ToList();

                foreach (var r in radnje)
                {
                    var radnjaDto = new RadnjaIzvestajDTO
                    {
                        Id = r.Id.Value,
                        NazivRadnje = r.TipRadnje.ToString(),
                        Datum = r.DatumIzvrsenja,
                        Kultura = r.Kultura?.Naziv ?? string.Empty,
                        IdKultura = r.IdKultura ?? Guid.Empty,
                        Trosak = (decimal)r.UkupanTrosak,
                        Prinos = (r is Zetva zetva) ? (decimal)zetva.Prinos : 0
                    };

                    radnjaDto.RadneMasine = r.RadnjeRadneMasine
                        .Select(m => new RadnjaRadnaMasinaDTO
                        {
                            IdRadnja = m.IdRadnja,
                            IdRadnaMasina = m.IdRadnaMasina,
                            NazivRadneMasine = m.RadnaMasina?.Naziv ?? "(nepoznata)",
                            BrojRadnihSati = m.BrojRadnihSati
                        }).ToList();

                    radnjaDto.Resursi = r.RadnjeResursi
                        .Select(rr => new RadnjaResursDTO
                        {
                            IdRadnja = rr.IdRadnja,
                            IdResurs = rr.IdResurs,
                            NazivResursa = rr.Resurs?.Naziv ?? "(nepoznat)",
                            Kolicina = rr.Kolicina,
                            DatumKoriscenja = rr.DatumKoriscenja
                        }).ToList();

                    parcelaDto.Radnje.Add(radnjaDto);
                }

                rezultat.Add(parcelaDto);
            }

            return rezultat;
        }
    }
}
