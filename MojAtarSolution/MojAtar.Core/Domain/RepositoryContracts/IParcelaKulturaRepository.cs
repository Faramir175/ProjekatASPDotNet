using MojAtar.Core.Domain.Enums;
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

    }
}
