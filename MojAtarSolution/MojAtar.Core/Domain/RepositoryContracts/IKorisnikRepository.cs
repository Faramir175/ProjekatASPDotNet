using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IKorisnikRepository:IRepository<Korisnik>
    {
        public Task<Korisnik> GetByTipKorisnika(KorisnikTip tipKorisnika);
        public Task<Korisnik> GetByEmail(string email);
        public Task<bool> DeleteKorisnikById(Guid? id);

    }
}
