using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Infrastructure.MojAtar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MojAtar.Infrastructure.Repositories
{
    public class RadnjaParcelaRepository : IRadnjaParcelaRepository
    {
        private readonly MojAtarDbContext _dbContext;

        public RadnjaParcelaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // --- CRUD METODE (Implementirane ručno kao u tvom primeru) ---

        public async Task<RadnjaParcela> Add(RadnjaParcela entity)
        {
            _dbContext.RadnjeParcele.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<RadnjaParcela> Update(RadnjaParcela entity)
        {
            // Prvo dohvatamo postojeći entitet da bismo ga ažurirali
            var postojeci = await _dbContext.RadnjeParcele.FindAsync(entity.Id);

            if (postojeci == null)
                return entity;

            // Ručno mapiranje polja koja smeju da se menjaju
            postojeci.IdParcela = entity.IdParcela;
            postojeci.IdRadnja = entity.IdRadnja;
            postojeci.Povrsina = entity.Povrsina;

            await _dbContext.SaveChangesAsync();
            return postojeci;
        }

        public async Task<bool> Delete(RadnjaParcela entity)
        {
            _dbContext.RadnjeParcele.Remove(entity);
            int rowsDeleted = await _dbContext.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<List<RadnjaParcela>> GetAll()
        {
            return await _dbContext.RadnjeParcele.ToListAsync();
        }

        public async Task<RadnjaParcela> GetById(Guid? id)
        {
            return await _dbContext.RadnjeParcele.FindAsync(id);
        }

        // --- SPECIFIČNE METODE ---

        public async Task<List<RadnjaParcela>> GetAllByRadnjaId(Guid idRadnja)
        {
            return await _dbContext.RadnjeParcele
                .Include(rp => rp.Parcela) // Uključujemo Parcelu da bismo imali Naziv
                .Where(rp => rp.IdRadnja == idRadnja)
                .ToListAsync();
        }

        public async Task<RadnjaParcela?> GetByRadnjaAndParcela(Guid idRadnja, Guid idParcela)
        {
            return await _dbContext.RadnjeParcele
                .FirstOrDefaultAsync(rp => rp.IdRadnja == idRadnja && rp.IdParcela == idParcela);
        }
    }
}