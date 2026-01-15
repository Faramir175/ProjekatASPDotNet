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
            // 1. Osnovni upit
            var query = _dbContext.Parcele
                .AsNoTracking()
                .Where(p => p.IdKorisnik == korisnikId);

            // 2. Filtriranje po parceli
            if (!sveParcele && idParcele.HasValue)
            {
                query = query.Where(p => p.Id == idParcele);
            }

            // 3. Filtriranje parcela preko nove vezne tabele RadnjeParcele
            query = query.Where(p => p.RadnjeParcele.Any(rp =>
                (!odDatuma.HasValue || rp.Radnja.DatumIzvrsenja >= odDatuma) &&
                (!doDatuma.HasValue || rp.Radnja.DatumIzvrsenja <= doDatuma)));

            // 4. Glavna projekcija
            var rezultat = await query.Select(p => new ParcelaIzvestajDTO
            {
                Id = p.Id.Value,
                NazivParcele = p.Naziv,

                // Pristupamo radnjama preko vezne tabele rp.Radnja
                Radnje = p.RadnjeParcele
                    .Where(rp => (!odDatuma.HasValue || rp.Radnja.DatumIzvrsenja >= odDatuma) &&
                                 (!doDatuma.HasValue || rp.Radnja.DatumIzvrsenja <= doDatuma))
                    .Select(rp => new RadnjaIzvestajDTO
                    {
                        Id = rp.Radnja.Id.Value,
                        NazivRadnje = rp.Radnja.TipRadnje.ToString(),
                        Datum = rp.Radnja.DatumIzvrsenja,
                        Kultura = rp.Radnja.Kultura != null ? rp.Radnja.Kultura.Naziv : string.Empty,
                        IdKultura = rp.Radnja.IdKultura ?? Guid.Empty,

                        // BITNO: Ovde uzimamo UkupanTrosak radnje, ili možeš računati proporcionalno površini
                        Trosak = (decimal)rp.Radnja.UkupanTrosak,

                        // Casting za Žetvu preko navigacije rp.Radnja
                        Prinos = (decimal?)((rp.Radnja as Zetva).Prinos) ?? 0,

                        RadneMasine = rp.Radnja.RadnjeRadneMasine.Select(rm => new RadnjaRadnaMasinaDTO
                        {
                            IdRadnja = rm.IdRadnja,
                            IdRadnaMasina = rm.IdRadnaMasina,
                            NazivRadneMasine = rm.RadnaMasina != null ? rm.RadnaMasina.Naziv : "(nepoznata)",
                            BrojRadnihSati = rm.BrojRadnihSati
                        }).ToList(),

                        Resursi = rp.Radnja.RadnjeResursi.Select(rr => new RadnjaResursDTO
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
