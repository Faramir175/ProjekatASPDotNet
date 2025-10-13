using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using MojAtar.Core.Domain.RepositoryContracts;

namespace MojAtar.Core.Services
{
    public class IzvestajService : IIzvestajService
    {
        private readonly IIzvestajRepository _izvestajRepository;

        public IzvestajService(IIzvestajRepository izvestajRepository)
        {
            _izvestajRepository = izvestajRepository;
        }

        public async Task<IzvestajDTO> GenerisiIzvestaj(Guid userId, Guid? parcelaId, DateTime? odDatuma, DateTime? doDatuma, bool sveParcele)
        {
            var parcele = await _izvestajRepository.GetIzvestaj(userId, odDatuma, doDatuma, parcelaId, sveParcele);

            return new IzvestajDTO
            {
                DatumOd = odDatuma ?? DateTime.MinValue,
                DatumDo = doDatuma ?? DateTime.MaxValue,
                Parcele = parcele
            };
        }
    }
}
