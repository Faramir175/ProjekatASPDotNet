using Microsoft.AspNetCore.Identity;
using MojAtar.Core.ServiceContracts;
using MojAtar.Core.DTO; 

public class IdentityPasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<KorisnikRequestDTO> _passwordHasher;

    public IdentityPasswordHasherService()
    {
        _passwordHasher = new PasswordHasher<KorisnikRequestDTO>();
    }

    public string HashPassword(string lozinka)
    {
        var dummyUser = new KorisnikRequestDTO();
        return _passwordHasher.HashPassword(dummyUser, lozinka);
    }

    public bool VerifyPassword(string heshovanaLozinka, string unetaLozinka)
    {
        var dummyUser = new KorisnikRequestDTO();
        var result = _passwordHasher.VerifyHashedPassword(dummyUser, heshovanaLozinka, unetaLozinka);
        return result == PasswordVerificationResult.Success;
    }
}