using MojAtar.Core.Domain;
using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IParcelaKulturaService
    {
        Task<ParcelaKulturaDTO> Add(ParcelaKulturaDTO dto);
        Task<bool> Delete(Guid id);
        Task<List<ParcelaKulturaDTO>> GetAll();
        Task<List<ParcelaKulturaDTO>> GetAllByParcelaId(Guid idParcela);
        Task<ParcelaKulturaDTO?> GetByParcelaAndKulturaId(Guid idParcela, Guid idKultura);
        Task<ParcelaKulturaDTO?> Update(ParcelaKulturaDTO dto);
        Task<int> DeleteIfNotCompleted(Guid idParcela, Guid idKultura, Guid idSetvaRadnja);
        Task<ParcelaKulturaDTO?> GetNezavrsenaSetva(Guid idParcela, Guid idKultura);
        Task<ParcelaKulturaDTO?> UpdateNezavrsena(Guid idParcela, Guid idKultura, decimal novaPovrsina);
        Task<List<ParcelaKulturaDTO>> GetSveNezavrseneSetve(Guid idParcela, Guid idKultura);
        Task<ParcelaKulturaDTO?> GetBySetvaRadnjaId(Guid idSetvaRadnja);
        Task<List<ParcelaKulturaDTO>> GetSveZaZetvu(Guid idZetvaRadnja);
    }
}
