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
                           .FirstOrDefaultAsync(pk => pk.IdParcela == idParcela && pk.IdKultura == idKultura );
        }

        public async Task<Parcela_Kultura> GetNezavrsenaSetva(Guid idParcela, Guid idKultura)
        {
            return await _dbContext.ParceleKulture
                .Include(pk => pk.Parcela)
                .Include(pk => pk.Kultura)
                .FirstOrDefaultAsync(pk =>
                    pk.IdParcela == idParcela &&
                    pk.IdKultura == idKultura &&
                    pk.IdZetvaRadnja == null);
        }


        public async Task<Parcela_Kultura> Update(Parcela_Kultura entity)
        {
            var existing = await _dbContext.ParceleKulture.FirstOrDefaultAsync(pk => pk.Id == entity.Id);

            if (existing == null)
                return null;

            existing.Povrsina = entity.Povrsina;
            existing.DatumSetve = entity.DatumSetve;
            existing.DatumZetve = entity.DatumZetve;
            existing.IdSetvaRadnja = entity.IdSetvaRadnja;
            existing.IdZetvaRadnja = entity.IdZetvaRadnja;

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<Parcela_Kultura?> UpdateNezavrsena(Guid idParcela, Guid idKultura, decimal novaPovrsina)
        {
            var existing = await _dbContext.ParceleKulture
                .FirstOrDefaultAsync(pk =>
                    pk.IdParcela == idParcela &&
                    pk.IdKultura == idKultura &&
                    pk.IdZetvaRadnja == null);

            if (existing == null)
                return null;

            existing.Povrsina = novaPovrsina;
            await _dbContext.SaveChangesAsync();
            return existing;
        }


        public async Task<bool> DeleteById(Guid id)
        {
            var entity = await GetById(id);
            if (entity == null) return false;

            _dbContext.ParceleKulture.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Parcela_Kultura> GetById(Guid? id)
        {
            return await _dbContext.ParceleKulture
            .Include(pk => pk.Kultura)
            .FirstOrDefaultAsync(pk => pk.Id == id);
        }
        public async Task<int> DeleteAddedForParcelaKultura(Guid idParcela, Guid idKultura, Guid idSetvaRadnja)
        {
            var entities = await _dbContext.ParceleKulture
                .Where(pk => pk.IdParcela == idParcela
                          && pk.IdKultura == idKultura
                          && pk.IdSetvaRadnja == idSetvaRadnja)
                .ToListAsync();

            if (entities.Any())
                _dbContext.ParceleKulture.RemoveRange(entities);

            return await _dbContext.SaveChangesAsync();
        }

    }
}
