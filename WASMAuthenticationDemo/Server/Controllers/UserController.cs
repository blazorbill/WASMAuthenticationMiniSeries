using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WASMAuthenticationDemo.Server.Services;
using WASMAuthenticationDemo.Shared;

namespace WASMAuthenticationDemo.Server.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserAuthService _userAuthService;
        private readonly IUserService _userService;

        public UserController(IUserAuthService userAuthService,
            IUserService userService)
        {
            _userAuthService = userAuthService;
            _userService = userService;
        }
        [AllowAnonymous]
        [HttpPost("api/login")]
        public async Task<LoginResult> Login(Credentials credentials)
        {
            LoginResult result = await _userAuthService.LoginUserAsync(credentials);
            return result;
        }
        [AllowAnonymous]
        [HttpPost("api/register")]
        public async Task<LoginResult> Register(UserRegistration user)
        {
            LoginResult result = await _userAuthService.RegisterUserAsync(user);
            return result;
        }
        [HttpGet("api/user/{Id}")]
        public UserDTO GetUser(string Id)
        {
            return _userService.GetUser(Id);
        }
        [HttpGet("api/users")]
        public IEnumerable<UserDTO> GetUsers()
        {
            return _userService.GetUsers();
        }

    }
}
