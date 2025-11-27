using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IPasswordHasherService
    {
        string HashPassword(string lozinka);
        bool VerifyPassword(string heshovanaLozinka, string unetaLozinka);
    }
}
