using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GetTeacherServer.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace GetTeacherServer.Services.Generators;

public class JwtTokenGenerator
{
    private readonly IConfiguration configuration;
    private readonly UserManager<DbUser> userManager;

    public JwtTokenGenerator(IConfiguration configuration, UserManager<DbUser> userManager)
    {
        this.configuration = configuration;
        this.userManager = userManager;

        // Check if the required configuration is set
        string? key = configuration["JwtSettings:Key"];
        string? issuer = configuration["JwtSettings:Issuer"];
        string? audience = configuration["JwtSettings:Audience"];

        if (key == null || issuer == null || audience == null)
        {
            // Something wrong in the configuration, JwtSettings is not set up correctly
            // TODO: Change to a [critical] using logger
            Console.WriteLine("JwtSettings is not set up correctly");
        }
    }

    public async Task<string> GenerateUserToken(DbUser user)
    {
        // Add basic email username and JwtId claims
        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(await userManager.GetClaimsAsync(user));
        // TODO: Add ClaimTypes.Role for roles

        return GenerateToken(claims, TimeSpan.FromMinutes(60));
    }

    private string GenerateToken(IEnumerable<Claim> claims, TimeSpan tokenLifetime)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!));
        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(tokenLifetime),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}