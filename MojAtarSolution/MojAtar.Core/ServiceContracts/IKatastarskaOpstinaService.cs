using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IKatastarskaOpstinaService
    {
        public Task<KatastarskaOpstinaDTO> Add(KatastarskaOpstinaDTO dto);
        public Task<List<KatastarskaOpstinaDTO>> GetAll();
        public Task<KatastarskaOpstinaDTO> GetById(Guid? id);
        public Task<KatastarskaOpstinaDTO> Update(KatastarskaOpstinaDTO dto);
        public Task<bool> DeleteById(Guid? id);


    }
}
