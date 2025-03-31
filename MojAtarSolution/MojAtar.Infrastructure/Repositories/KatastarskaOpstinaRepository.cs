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
    public class KatastarskaOpstinaRepository : IKatastarskaOpstinaRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public KatastarskaOpstinaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<KatastarskaOpstina>> GetAll()
        {
            return await _dbContext.KatastarskeOpstine.ToListAsync();
        }

        public async Task<KatastarskaOpstina> GetById(Guid? id)
        {
            return await _dbContext.KatastarskeOpstine.FindAsync(id);
        }

        public async Task<KatastarskaOpstina> Add(KatastarskaOpstina entity)
        {
            _dbContext.KatastarskeOpstine.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<KatastarskaOpstina> Update(KatastarskaOpstina entity)
        {
            KatastarskaOpstina? katastarskaOpstinaZaUpdata = await _dbContext.KatastarskeOpstine.FirstOrDefaultAsync(temp => temp.Id == entity.Id);

            if (katastarskaOpstinaZaUpdata == null)
                return entity;

            katastarskaOpstinaZaUpdata.Id = entity.Id;
            katastarskaOpstinaZaUpdata.Naziv = entity.Naziv;
            katastarskaOpstinaZaUpdata.GradskaOpstina = entity.GradskaOpstina;

            await _dbContext.SaveChangesAsync();

            return katastarskaOpstinaZaUpdata;
        }

        public async Task<bool> Delete(KatastarskaOpstina entity)
        {
            _dbContext.KatastarskeOpstine.RemoveRange(_dbContext.KatastarskeOpstine.Where(temp => temp == entity));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }
    }
}
