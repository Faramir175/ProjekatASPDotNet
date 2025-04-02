using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IParcelaService
    {
        public Task<ParcelaDTO> Add(ParcelaDTO dto);
        public Task<List<ParcelaDTO>> GetAll();
        public Task<ParcelaDTO> GetById(Guid? id);
        public Task<ParcelaDTO> GetByNaziv(string? naziv);
        public Task<ParcelaDTO> Update(Guid? id, ParcelaDTO dto);
        public Task<bool> DeleteById(Guid? id);

    }
}
