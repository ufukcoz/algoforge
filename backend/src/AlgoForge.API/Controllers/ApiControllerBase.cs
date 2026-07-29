using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlgoForge.API.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    // JWT'deki "sub" claim'i, kullanilan token handler'a gore ClaimTypes.NameIdentifier'a
    // otomatik map edilebilir ya da edilmeyebilir; ikisini de kontrol ederek garantiye aliyoruz.
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Token icinde gecerli bir kullanici kimligi bulunamadi.");

        return userId;
    }
}
