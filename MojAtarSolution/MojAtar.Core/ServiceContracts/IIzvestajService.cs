using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IIzvestajService
    {
        Task<IzvestajDTO> GenerisiIzvestaj(Guid userId, Guid? parcelaId, DateTime? odDatuma, DateTime? doDatuma, bool sveParcele);
    }
}
