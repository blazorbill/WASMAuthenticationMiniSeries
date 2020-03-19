using Microsoft.AspNetCore.Components.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System.Security.Claims;
using WASMAuthenticationDemo.Client.Helpers;
using System;
using WASMAuthenticationDemo.Shared;
using System.Collections.Generic;
using WASMAuthenticationDemo.Client.Repository;

namespace WASMAuthenticationDemo.Client.Services
{
    public class SavedToken
    {
        public IEnumerable<Claim> Claims { get; set; }
        public LoginResult SavedLR { get; set; } = new LoginResult();
    }
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider, IWASMAuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IGenericRepository _genericRepository;

        private bool firstTimeThrough = true;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, 
            IGenericRepository genericRepository)
        {
            _localStorage = localStorage;
            _genericRepository = genericRepository;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            SavedToken savedToken = await GetTokenAsync();

            if (string.IsNullOrWhiteSpace(savedToken.SavedLR.Token))
            {
                firstTimeThrough = false;
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            if (firstTimeThrough)
            {
                firstTimeThrough = false;
                await MarkUserAsAuthenticated(savedToken);
            }
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(savedToken.Claims, "jwt")));
        }
        //Public interface...no need for claims to be exposed
        public async Task MarkUserAsAuthenticated(LoginResult lr)
        {
            SavedToken st = await ParseToken(lr);
            await MarkUserAsAuthenticated(st);
        }
        private async Task MarkUserAsAuthenticated(SavedToken savedToken)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, savedToken.SavedLR.UserId) }, "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            await _localStorage.SetItemAsync("authToken", savedToken.SavedLR.Token);
            await _localStorage.SetItemAsync("tokenExpire", savedToken.SavedLR.ExpirationDate);
            _genericRepository.SetToken(savedToken.SavedLR.Token);
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("tokenExpire");
            _genericRepository.SetToken(string.Empty);
            NotifyAuthenticationStateChanged(authState);
        }

        private async Task<SavedToken> GetTokenAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            var expireDate = await _localStorage.GetItemAsync<DateTime>("tokenExpire");
            return await ParseToken(new LoginResult() {
                Token=savedToken,
                ExpirationDate=expireDate
            });
        }

        private async Task<SavedToken> ParseToken(LoginResult lr) { 
            if (string.IsNullOrWhiteSpace(lr.Token))
            {
                return new SavedToken();
            }
            var tokenExpired = IsTokenExpired(lr.ExpirationDate);
            if (tokenExpired)
            {
                await MarkUserAsLoggedOut();
                return new SavedToken();
            }
            var claims = JwtParserHelper.ParseClaimsFromJwt(lr.Token);
            string userId = claims.Where(x => x.Type == "nameid").Select(x => x.Value).FirstOrDefault();
            return new SavedToken()
            {
                Claims = claims,
                SavedLR = new LoginResult()
                {
                    UserId = userId,
                    Token = lr.Token,
                    ExpirationDate = lr.ExpirationDate
                }
            };
        }

        private bool IsTokenExpired(DateTime expireDate)
        {
            return expireDate < DateTime.UtcNow;
        }
    }
}
