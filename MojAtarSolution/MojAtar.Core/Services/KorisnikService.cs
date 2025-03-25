using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.ExtensionKlase;
using MojAtar.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Services
{
    public class KorisnikService:IKorisnikService
    {
        private readonly IKorisnikRepository _korisnikRepository;

        public KorisnikService(IKorisnikRepository korisnikRepository)
        {
            _korisnikRepository = korisnikRepository;
        }

        public async Task<KorisnikResponse> Add(KorisnikRequest korisnikAdd)
        {
            if (korisnikAdd == null)
            {
                throw new ArgumentNullException(nameof(korisnikAdd));
            }

            if (korisnikAdd.Email == null)
            {
                throw new ArgumentException(nameof(korisnikAdd.Email));
            }

            if (await _korisnikRepository.GetByEmail(korisnikAdd.Email) != null)
            {
                throw new ArgumentException("Given email already exists");
            }

            Korisnik korisnik = korisnikAdd.ToKorisnik();

            korisnik.Id = Guid.NewGuid();

            await _korisnikRepository.Add(korisnik);

            return korisnik.ToKorisnikResponse();
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Korisnik? korisnik = await _korisnikRepository.GetById(id.Value);
            if (korisnik == null)
                return false;

            await _korisnikRepository.DeleteKorisnikById(id.Value);

            return true;
        }

        public Task<List<KorisnikResponse>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<KorisnikResponse> GetById(Guid? id)
        {
            throw new NotImplementedException();
        }


        public Task<KorisnikResponse> UpdateById(Guid? id, KorisnikRequest dto)
        {
            throw new NotImplementedException();
        }
    }
}
