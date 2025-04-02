using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IKorisnikService
    {
        public Task<KorisnikResponseDTO> Add(KorisnikRequestDTO dto);
        public Task<List<KorisnikResponseDTO>> GetAll();
        public Task<KorisnikResponseDTO> GetById(Guid? id);
        public Task<KorisnikResponseDTO> GetByEmail(string? email);
        public Task<KorisnikResponseDTO> Update(Guid? id, KorisnikRequestDTO dto);
        public Task<bool> DeleteById(Guid? id);

    }
}
