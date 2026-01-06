using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MojAtar.Core.Services
{
    public class RadnjaParcelaService : IRadnjaParcelaService
    {
        private readonly IRadnjaParcelaRepository _repo;

        public RadnjaParcelaService(IRadnjaParcelaRepository repo)
        {
            _repo = repo;
        }

        public async Task Add(Guid idRadnja, RadnjaParcelaDTO dto)
        {
            var entity = new RadnjaParcela
            {
                IdRadnja = idRadnja,
                IdParcela = dto.IdParcela,
                Povrsina = dto.Povrsina
            };

            // Ovde možeš dodati validaciju da li već postoji, ali repo će verovatno baciti grešku
            await _repo.Add(entity);
        }

        public async Task<List<RadnjaParcelaDTO>> GetAllByRadnjaId(Guid idRadnja)
        {
            var entities = await _repo.GetAllByRadnjaId(idRadnja);

            // Mapiranje u DTO
            return entities.Select(e => new RadnjaParcelaDTO
            {
                IdParcela = e.IdParcela,
                NazivParcele = e.Parcela?.Naziv, // Pretpostavka da Parcela ima property Naziv
                Povrsina = e.Povrsina
            }).ToList();
        }

        public async Task Delete(Guid idRadnja, Guid idParcela)
        {
            var entity = await _repo.GetByRadnjaAndParcela(idRadnja, idParcela);
            if (entity != null)
            {
                await _repo.Delete(entity);
            }
        }

        // *** OVO JE KLJUČNA METODA ZA UPDATE ***
        public async Task UpdateParceleZaRadnju(Guid idRadnja, List<RadnjaParcelaDTO> dolazneParceleDto)
        {
            // 1. Izvuci trenutno stanje iz baze
            var postojeceUdB = await _repo.GetAllByRadnjaId(idRadnja);

            // Ako je lista null, inicijalizuj je da ne pukne foreach
            if (dolazneParceleDto == null) dolazneParceleDto = new List<RadnjaParcelaDTO>();

            // 2. BRISANJE: Nađi one koje su u bazi, a NEMA ih u novoj listi
            foreach (var postojeca in postojeceUdB)
            {
                // Da li dolazna lista sadrži ovu parcelu?
                bool iDaljePostoji = dolazneParceleDto.Any(d => d.IdParcela == postojeca.IdParcela);

                if (!iDaljePostoji)
                {
                    // Ako je nema u novoj listi, brišemo je iz baze
                    await _repo.Delete(postojeca);
                }
            }

            // 3. DODAVANJE I AŽURIRANJE
            foreach (var dolazna in dolazneParceleDto)
            {
                // Pokušamo da nađemo tu parcelu u onima koje su već bile u bazi
                var postojeca = postojeceUdB.FirstOrDefault(p => p.IdParcela == dolazna.IdParcela);

                if (postojeca != null)
                {
                    // --- UPDATE ---
                    // Parcela je već tu, samo proveravamo da li se promenila površina
                    if (postojeca.Povrsina != dolazna.Povrsina)
                    {
                        postojeca.Povrsina = dolazna.Povrsina;
                        await _repo.Update(postojeca);
                    }
                }
                else
                {
                    // --- INSERT ---
                    // Parcela nije postojala u bazi za ovu radnju, dodajemo je
                    var nova = new RadnjaParcela
                    {
                        IdRadnja = idRadnja,
                        IdParcela = dolazna.IdParcela,
                        Povrsina = dolazna.Povrsina
                    };
                    await _repo.Add(nova);
                }
            }
        }
    }
}