using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WASMAuthenticationDemo.Server.Models;
using WASMAuthenticationDemo.Shared;

namespace WASMAuthenticationDemo.Server.Services
{
    public class UserAuthService : IUserAuthService
    {
        private readonly UserManager<WASMUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserAuthService(UserManager<WASMUser> userManager, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<LoginResult> RegisterUserAsync(UserRegistration user)
        {
            try
            {

            var userId = new WASMUser()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            var result = await _userManager.CreateAsync(userId, user.Password);

            if (result.Succeeded)
            {
                return GetLoginResult(userId);
            }

            return new LoginResult();
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        public async Task<LoginResult> LoginUserAsync(Credentials user)
        {
            var userId = await _userManager.FindByNameAsync(user.UserName);

            if(userId != null)
            {
                var validCredentials = await _userManager.CheckPasswordAsync(userId, user.Password);
                if (validCredentials)
                {
                    return GetLoginResult(userId);
                }
            }
            return new LoginResult();
        }
        public WASMUser GetUser(string Id)
        {
            var userId = _userManager.Users.FirstOrDefault(x => x.Id == Id);
            if(userId != null)
            {
                return userId;
            }
            return null;
        }
        public IEnumerable<WASMUser> GetUsers()
        {
            return _userManager.Users.ToList();
        }


        private string GenerateJwtToken(string userId, DateTime expry)
        {
            try
            {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"],
                Subject = new ClaimsIdentity(claims),
                Expires = expry,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return string.Empty;
            }
        }

        private LoginResult GetLoginResult(WASMUser user)
        {
            DateTime dtExpire = DateTime.UtcNow.AddMinutes(10);
            return new LoginResult()
            {
                UserId = user.Id,
                ExpirationDate = dtExpire,
                Token = GenerateJwtToken(user.Id, dtExpire)
            };
        }

    }
}
