using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IParcelaKulturaRepository
    {
        Task<Parcela_Kultura> GetByParcelaAndKulturaId(Guid idParcela, Guid idKultura);
        Task<bool> DeleteByParcelaAndKulturaId(Guid idParcela, Guid idKultura);
        Task<List<Parcela_Kultura>> GetAllByParcelaId(Guid idParcela);
        Task<List<Parcela_Kultura>> GetAll();
        Task<Parcela_Kultura> Add(Parcela_Kultura entity);
        Task<Parcela_Kultura> Update(Parcela_Kultura entity);
        Task<bool> Delete(Parcela_Kultura entity);

    }
}
