using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBusiness.RelationData;
using MyBusiness.UserMicroservice.Models;

namespace MyBusiness.UserMicroservice.Services
{
    public class UserService : IUserService
    {
    private readonly MySQLDataContext _mysqlcontext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IConfiguration _configuration;

    public UserService(MySQLDataContext mysqlcontext, MongoDBDataContext mongodbcontext, IConfiguration configuration)
    {
         _mysqlcontext = mysqlcontext;
         _mongodbcontext = mongodbcontext;
         _configuration = configuration;
    }

    public async Task<string> RegistrationAsync(User user)
    {
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        _mysqlcontext.Users.Add(user);
        await _mysqlcontext.SaveChangesAsync();

        await _mongodbcontext.Users.InsertOneAsync(user);

        return "User registered successfully";
    }

    public async Task<string> LoginAsync(string email, string password)
     {
         var dbUser = await _mysqlcontext.Users.FirstOrDefaultAsync(a => a.Email == email);

         if (dbUser == null || !BCrypt.Net.BCrypt.Verify(password, dbUser.Password))
         {
             throw new UnauthorizedAccessException("Invalid email or password.");
         }

         return GenerateJwtToken(dbUser);
     }

    public string GenerateJwtToken(User user)
     {
         var key = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

         var credentials = new SigningCredentials(
             key, SecurityAlgorithms.HmacSha256);

         var claims = new[]
         {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
         };

         var token = new JwtSecurityToken(
             _configuration["Jwt:Issuer"],
             _configuration["Jwt:Audience"],
             claims,
             expires: DateTime.UtcNow.AddMinutes(
                 Convert.ToInt32(_configuration["Jwt:ExpirationInMinutes"])),
                  
                 signingCredentials : credentials
             );
             return new JwtSecurityTokenHandler().WriteToken(token);
     }

    }
}
