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
            // 1. Osnovni upit za parcele korisnika
            var query = _dbContext.Parcele
                .AsNoTracking() // Dobra praksa za read-only upite (ubrzava)
                .Where(p => p.IdKorisnik == korisnikId);

            // 2. Filtriranje po specifičnoj parceli ako nije izabrano "sve"
            if (!sveParcele && idParcele.HasValue)
            {
                query = query.Where(p => p.Id == idParcele);
            }

            // 3. Filtriranje parcela koje uopšte imaju radnje u tom periodu
            // Ovo sprečava da vučemo prazne parcele
            query = query.Where(p => p.Radnje.Any(r =>
                (!odDatuma.HasValue || r.DatumIzvrsenja >= odDatuma) &&
                (!doDatuma.HasValue || r.DatumIzvrsenja <= doDatuma)));

            // 4. Glavna projekcija (Select) - Ovde se dešava magija i rešava CS8122
            var rezultat = await query.Select(p => new ParcelaIzvestajDTO
            {
                Id = p.Id.Value,
                NazivParcele = p.Naziv,

                // Filtriramo i mapiramo radnje direktno u upitu
                Radnje = p.Radnje
                    .Where(r => (!odDatuma.HasValue || r.DatumIzvrsenja >= odDatuma) &&
                                (!doDatuma.HasValue || r.DatumIzvrsenja <= doDatuma))
                    .Select(r => new RadnjaIzvestajDTO
                    {
                        Id = r.Id.Value,
                        NazivRadnje = r.TipRadnje.ToString(),
                        Datum = r.DatumIzvrsenja,
                        Kultura = r.Kultura != null ? r.Kultura.Naziv : string.Empty, // Null check za svaki slučaj
                        IdKultura = r.IdKultura ?? Guid.Empty,
                        Trosak = (decimal)r.UkupanTrosak,

                        // FIX ZA CS8122: Koristimo 'as' casting i null-coalescing operator
                        // Ako r nije Zetva, (r as Zetva) je null.
                        // Ako je Zetva ali je Prinos null, vraća 0.
                        Prinos = (decimal?)((r as Zetva).Prinos) ?? 0,

                        // Mapiranje ugnježdenih kolekcija
                        RadneMasine = r.RadnjeRadneMasine.Select(rm => new RadnjaRadnaMasinaDTO
                        {
                            IdRadnja = rm.IdRadnja,
                            IdRadnaMasina = rm.IdRadnaMasina,
                            NazivRadneMasine = rm.RadnaMasina != null ? rm.RadnaMasina.Naziv : "(nepoznata)",
                            BrojRadnihSati = rm.BrojRadnihSati
                        }).ToList(),

                        Resursi = r.RadnjeResursi.Select(rr => new RadnjaResursDTO
                        {
                            IdRadnja = rr.IdRadnja,
                            IdResurs = rr.IdResurs,
                            NazivResursa = rr.Resurs != null ? rr.Resurs.Naziv : "(nepoznat)",
                            Kolicina = rr.Kolicina,
                            DatumKoriscenja = rr.DatumKoriscenja
                        }).ToList()
                    }).ToList()
            }).ToListAsync();

            return rezultat;
        }
    }
}
