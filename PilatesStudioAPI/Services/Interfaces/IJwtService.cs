using PilatesStudioAPI.Models.Entities;
using System.Security.Claims;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    bool ValidateToken(string token);
    DateTime GetTokenExpiration(string token);
}