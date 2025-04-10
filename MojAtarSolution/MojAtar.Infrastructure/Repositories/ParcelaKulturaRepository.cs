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
    public class ParcelaKulturaRepository : IParcelaKulturaRepository
    {
        private readonly MojAtarDbContext _dbContext;

        public ParcelaKulturaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Parcela_Kultura> Add(Parcela_Kultura entity)
        {
            _dbContext.ParceleKulture.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(Parcela_Kultura entity)
        {
            _dbContext.ParceleKulture.Remove(entity);
            int rowsDeleted = await _dbContext.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<List<Parcela_Kultura>> GetAll()
        {
            return await _dbContext.ParceleKulture
                .Include(pk => pk.Parcela)
                .Include(pk => pk.Kultura)
                .ToListAsync();
        }

        public async Task<List<Parcela_Kultura>> GetAllByParcelaId(Guid idParcela)
        {
            return await _dbContext.ParceleKulture
                .Where(pk => pk.IdParcela == idParcela)
                .Include(pk => pk.Kultura)
                .ToListAsync();
        }

        public async Task<Parcela_Kultura> GetByParcelaAndKulturaId(Guid idParcela, Guid idKultura)
        {
            return await _dbContext.ParceleKulture
                           .Include(p => p.Parcela)
                           .Include(k => k.Kultura)
                           .FirstOrDefaultAsync(pk => pk.IdParcela == idParcela && pk.IdKultura == idKultura);
        }

        public async Task<Parcela_Kultura> Update(Parcela_Kultura entity)
        {
            var existing = await GetByParcelaAndKulturaId(entity.IdParcela.Value, entity.IdKultura.Value);
            if (existing == null) return null;

            existing.Povrsina = entity.Povrsina;
            existing.DatumSetve = entity.DatumSetve;
            existing.DatumZetve = entity.DatumZetve;

            await _dbContext.SaveChangesAsync();
            return existing;
        }
        public async Task<bool> DeleteByParcelaAndKulturaId(Guid idParcela, Guid idKultura)
        {
            var entity = await GetByParcelaAndKulturaId(idParcela, idKultura);
            if (entity == null) return false;

            _dbContext.ParceleKulture.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

    }
}
