using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Infrastructure.MojAtar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Infrastructure.Repositories
{
    public class RadnjaRepository : IRadnjaRepository
    {
        private readonly MojAtarDbContext _dbContext;

        public RadnjaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Radnja>> GetAll()
        {
            return await _dbContext.Radnje
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .ToListAsync();
        }

        public async Task<Radnja> GetById(Guid? id)
        {
            var radnja = await _dbContext.Radnje
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (radnja?.TipRadnje == RadnjaTip.Zetva)
            {
                return radnja as Zetva;
            }

            return radnja;
        }


        public async Task<Radnja> GetByTipRadnje(RadnjaTip tipRadnje)
        {
            return await _dbContext.Radnje
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .FirstOrDefaultAsync(r => r.TipRadnje == tipRadnje);
        }

        public async Task<Radnja> Add(Radnja entity)
        {
            _dbContext.Radnje.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Radnja> Update(Radnja entity)
        {
            var radnjaIzBaze = await _dbContext.Radnje
                .FirstOrDefaultAsync(r => r.Id == entity.Id);

            if (radnjaIzBaze == null)
                return entity;

            if (entity.TipRadnje == RadnjaTip.Zetva && radnjaIzBaze is Zetva zetvaIzBaze && entity is Zetva novaZetva)
            {
                zetvaIzBaze.Prinos = novaZetva.Prinos;
            }

            radnjaIzBaze.IdParcela = entity.IdParcela;
            radnjaIzBaze.DatumIzvrsenja = entity.DatumIzvrsenja;
            radnjaIzBaze.Napomena = entity.Napomena;
            radnjaIzBaze.UkupanTrosak = entity.UkupanTrosak;
            radnjaIzBaze.TipRadnje = entity.TipRadnje;
            radnjaIzBaze.IdKultura = entity.IdKultura;

            await _dbContext.SaveChangesAsync();
            return radnjaIzBaze;
        }

        public async Task<bool> Delete(Radnja entity)
        {
            _dbContext.Radnje.Remove(entity);
            int rowsDeleted = await _dbContext.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<bool> DeleteRadnjaById(Guid? id)
        {
            var radnja = await _dbContext.Radnje.FindAsync(id);
            if (radnja == null) return false;

            _dbContext.Radnje.Remove(radnja);
            int rowsDeleted = await _dbContext.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<List<Radnja>> GetAllByParcela(Guid idParcela)
        {
            return await _dbContext.Radnje
                .Where(r => r.IdParcela == idParcela)
                .Include(r => r.Kultura)
                .ToListAsync();
        }

        public async Task<List<Radnja>> GetAllByKultura(Guid idKultura)
        {
            return await _dbContext.Radnje
                .Where(r => r.IdKultura == idKultura)
                .Include(r => r.Parcela)
                .ToListAsync();
        }

        public async Task<decimal> GetUkupanPrinosZaParcelu(Guid idParcela)
        {
            var ukupno = await _dbContext.Radnje
                .OfType<Zetva>()
                .Where(z => z.IdParcela == idParcela)
                .SumAsync(z => z.Prinos);

            return (decimal)ukupno;
        }

        public async Task<int> GetCountByParcela(Guid idParcela)
        {
            return await _dbContext.Radnje.Where(x => x.IdParcela == idParcela).CountAsync();
        }

        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _dbContext.Radnje.Where(x => x.Parcela.IdKorisnik == idKorisnik).CountAsync();
        }

        public async Task<List<Radnja>> GetAllByParcelaPaged(Guid idParcela, int skip, int take)
        {
            return await _dbContext.Radnje
                .Where(x => x.IdParcela == idParcela)
                .Include(r => r.Parcela) 
                .Include(r => r.Kultura) 
                .OrderByDescending(x => x.DatumIzvrsenja)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Radnja>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            return await _dbContext.Radnje
                .Where(x => x.Parcela.IdKorisnik == idKorisnik)
                .Include(r => r.Parcela) 
                .Include(r => r.Kultura)
                .OrderByDescending(x => x.DatumIzvrsenja)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        public async Task<List<Radnja>> GetLastRadnjeByKorisnik(Guid korisnikId, int broj)
        {
            return await _dbContext.Radnje
                .Where(r => r.Parcela.IdKorisnik == korisnikId)
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .OrderByDescending(r => r.DatumIzvrsenja)
                .Take(broj)
                .ToListAsync();
        }
        public Task<int> CountByKorisnikId(Guid korisnikId)
        {
            return _dbContext.Radnje.CountAsync(p => p.Parcela.IdKorisnik == korisnikId);
        }
        // Dohvata parcelu sa svim vezama na kulture (za proveru zauzetosti)
        public async Task<Parcela> GetParcelaSaSetvama(Guid idParcela)
        {
            return await _dbContext.Parcele
                .Include(p => p.ParceleKulture.Where(pk => pk.DatumZetve == null))
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == idParcela);
        }
        public async Task UpdateUkupanTrosak(Guid idRadnja)
        {
            var radnja = await _dbContext.Radnje
                .Include(r => r.RadnjeResursi)
                    .ThenInclude(rr => rr.Resurs)
                .FirstOrDefaultAsync(r => r.Id == idRadnja);

            if (radnja == null)
                return;

            double noviTrosak = 0;
            foreach (var rr in radnja.RadnjeResursi)
            {
                if (rr.Resurs != null)
                    noviTrosak += (double)(rr.Kolicina * rr.Resurs.AktuelnaCena);
            }

            radnja.UkupanTrosak = noviTrosak;
            await _dbContext.SaveChangesAsync();
        }

    }
}
