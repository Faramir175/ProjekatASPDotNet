using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IIzvestajRepository
    {
        Task<List<ParcelaIzvestajDTO>> GetIzvestaj(Guid korisnikId, DateTime? odDatuma, DateTime? doDatuma, Guid? idParcele, bool sveParcele);
    }
}
