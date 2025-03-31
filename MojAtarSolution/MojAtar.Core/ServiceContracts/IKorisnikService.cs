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
        public Task<KorisnikResponse> Add(KorisnikRequest dto);
        public Task<List<KorisnikResponse>> GetAll();
        public Task<KorisnikResponse> GetById(Guid? id);
        public Task<KorisnikResponse> GetByEmail(string? email);
        public Task<KorisnikResponse> Update(Guid? id, KorisnikRequest dto);
        public Task<bool> DeleteById(Guid? id);

    }
}
