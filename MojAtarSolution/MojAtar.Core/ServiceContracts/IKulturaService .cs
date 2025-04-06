﻿using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IResursService
    {
        public Task<ResursDTO> Add(ResursDTO dto);
        public Task<List<ResursDTO>> GetAllForUser(Guid idKorisnika);
        public Task<ResursDTO> GetById(Guid? id);
        public Task<ResursDTO> GetByNaziv(string? naziv);
        public Task<ResursDTO> Update(Guid? id, ResursDTO dto);
        public Task<bool> DeleteById(Guid? id);

    }
}
