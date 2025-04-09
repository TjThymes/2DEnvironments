using Microsoft.AspNetCore.Identity;
using ObjectEnvironmentPlacer.Objects;

namespace ObjectEnvironmentPlacer.Other
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(IdentityUser user);
    }
}
