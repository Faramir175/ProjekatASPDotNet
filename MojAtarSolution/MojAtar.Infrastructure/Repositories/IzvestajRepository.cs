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
                        .ThenInclude(k => k.CeneKulture)
                .Include(p => p.Radnje)
                    .ThenInclude(r => r.RadnjeRadneMasine)
                        .ThenInclude(rm => rm.RadnaMasina)
                .Include(p => p.Radnje)
                    .ThenInclude(r => r.RadnjeResursi)
                        .ThenInclude(rr => rr.Resurs)
                            .ThenInclude(res => res.CeneResursa)
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
                    // --- Cena kulture (najnovija do datuma radnje)
                    double cenaKulture = 0;
                    if (r.Kultura?.CeneKulture != null && r.Kultura.CeneKulture.Any())
                    {
                        cenaKulture = r.Kultura.CeneKulture
                            .Where(c => c.DatumVaznosti <= r.DatumIzvrsenja)
                            .OrderByDescending(c => c.DatumVaznosti)
                            .Select(c => c.CenaPojedinici)
                            .FirstOrDefault();
                    }

                    // --- Trošak i prihod
                    double trosak = r.UkupanTrosak;
                    double prihod = 0;
                    if (r.TipRadnje == RadnjaTip.Zetva && r is Zetva zetva)
                        prihod = zetva.Prinos * 1000 * cenaKulture;

                    var radnjaDto = new RadnjaIzvestajDTO
                    {
                        Id = (Guid)r.Id,
                        NazivRadnje = r.TipRadnje.ToString(),
                        Datum = r.DatumIzvrsenja,
                        Kultura = r.Kultura?.Naziv ?? string.Empty,
                        Trosak = trosak,
                        Prihod = prihod
                    };

                    // --- Radne mašine
                    radnjaDto.RadneMasine = r.RadnjeRadneMasine
                        .Select(m => new RadnjaRadnaMasinaDTO
                        {
                            IdRadnja = m.IdRadnja,
                            IdRadnaMasina = m.IdRadnaMasina,
                            NazivRadneMasine = m.RadnaMasina?.Naziv ?? "(nepoznata)",
                            BrojRadnihSati = m.BrojRadnihSati
                        }).ToList();

                    // --- Resursi
                    radnjaDto.Resursi = r.RadnjeResursi
                            .Select(rr =>
                            {
                                double cenaResursa = rr.Resurs?.CeneResursa?
                                    .Where(c => c.DatumVaznosti <= rr.DatumKoriscenja)
                                    .OrderByDescending(c => c.DatumVaznosti)
                                    .Select(c => c.CenaPojedinici)
                                    .FirstOrDefault() ?? 0;

                                return new RadnjaResursDTO
                                {
                                    IdRadnja = rr.IdRadnja,
                                    IdResurs = rr.IdResurs,
                                    NazivResursa = rr.Resurs?.Naziv ?? "Nepoznat",
                                    Kolicina = rr.Kolicina,
                                    DatumKoriscenja = rr.DatumKoriscenja,
                                    JedinicnaCena = cenaResursa
                                };
                            })
                            .ToList();

                    parcelaDto.Radnje.Add(radnjaDto);
                }

                rezultat.Add(parcelaDto);
            }

            return rezultat;
        }
    }
}
