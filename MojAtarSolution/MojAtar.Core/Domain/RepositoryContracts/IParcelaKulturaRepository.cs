using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IParcelaKulturaRepository: IRepository<Parcela_Kultura>
    {
        Task<Parcela_Kultura> GetByParcelaAndKulturaId(Guid idParcela, Guid idKultura);
        Task<List<Parcela_Kultura>> GetAllByParcelaId(Guid idParcela);
        Task<bool> DeleteById(Guid id);
        Task<int> DeleteAddedForParcelaKultura(Guid idParcela, Guid idKultura, Guid idSetvaRadnja);
        Task<Parcela_Kultura> GetNezavrsenaSetva(Guid idParcela, Guid idKultura);
        Task<Parcela_Kultura?> UpdateNezavrsena(Guid idParcela, Guid idKultura, decimal novaPovrsina);
        Task<List<Parcela_Kultura>> GetSveNezavrseneSetve(Guid idParcela, Guid idKultura);
        Task<Parcela_Kultura?> GetBySetvaRadnjaId(Guid idSetvaRadnja);
        Task<List<Parcela_Kultura>> GetSveZaZetvu(Guid idZetvaRadnja);
        Task<List<Parcela_Kultura>> GetAllBySetvaRadnjaId(Guid idSetvaRadnja);


    }
}
