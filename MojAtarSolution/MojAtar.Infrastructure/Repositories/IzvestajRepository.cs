using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
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
                    .ThenInclude(r => r.RadnjeResursi)
                        .ThenInclude(rr => rr.Resurs)
                .Include(p => p.Radnje)
                    .ThenInclude(r => r.RadnjeRadneMasine)
                        .ThenInclude(rm => rm.RadnaMasina)
                .Where(p => p.IdKorisnik == korisnikId);

            if (!sveParcele && idParcele.HasValue)
                query = query.Where(p => p.Id == idParcele);

            var parcele = await query.ToListAsync();

            var rezultat = new List<ParcelaIzvestajDTO>();

            foreach (var parcela in parcele)
            {
                var parcelaDto = new ParcelaIzvestajDTO
                {
                    Id = (Guid)parcela.Id,
                    NazivParcele = parcela.Naziv
                };

                var radnje = parcela.Radnje
                    .Where(r => (!odDatuma.HasValue || r.DatumIzvrsenja >= odDatuma)
                             && (!doDatuma.HasValue || r.DatumIzvrsenja <= doDatuma))
                    .ToList();

                foreach (var r in radnje)
                {
                    var radnjaDto = new RadnjaIzvestajDTO
                    {
                        Id = (Guid)r.Id,
                        NazivRadnje = r.TipRadnje.ToString(),
                        Datum = r.DatumIzvrsenja,
                        Kultura = r.Kultura?.Naziv ?? string.Empty,
                        Trosak = r.UkupanTrosak,
                        Prihod = r.TipRadnje == RadnjaTip.Zetva ? ((Zetva)r).Prinos * (r.Kultura?.AktuelnaCena ?? 0) : 0

                    };

                    // --- Radne mašine
                    radnjaDto.RadneMasine = r.RadnjeRadneMasine
                        .Select(m => new RadnjaRadnaMasinaDTO
                        {
                            IdRadnja = m.IdRadnja,
                            IdRadnaMasina = m.IdRadnaMasina,
                            BrojRadnihSati = m.BrojRadnihSati
                        }).ToList();


                    // --- Resursi
                    radnjaDto.Resursi = r.RadnjeResursi
                        .Select(rr => new RadnjaResursDTO
                        {
                            IdRadnja = rr.IdRadnja,
                            IdResurs = rr.IdResurs,
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
