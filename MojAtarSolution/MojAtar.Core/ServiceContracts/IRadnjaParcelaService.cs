using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IRadnjaParcelaService
    {
        // Додавање једне везе (користи се код креирања нове радње)
        Task Add(Guid idRadnja, RadnjaParcelaDTO dto);

        // Главна метода за ажурирање:
        // Прима ID радње и НОВУ листу парцела како треба да изгледа
        Task UpdateParceleZaRadnju(Guid idRadnja, List<RadnjaParcelaDTO> noveParceleDto);

        // Брисање једне конкретне (ако затреба)
        Task Delete(Guid idRadnja, Guid idParcela);

        // Дохватање свих за приказ (Edit форма)
        Task<List<RadnjaParcelaDTO>> GetAllByRadnjaId(Guid idRadnja);
    }
}
