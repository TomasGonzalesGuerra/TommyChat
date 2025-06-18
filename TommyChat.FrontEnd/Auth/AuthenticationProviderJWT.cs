using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using TommyChat.FrontEnd.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace TommyChat.FrontEnd.Auth
{
    public class AuthenticationProviderJWT(IJSRuntime jSRuntime, HttpClient httpClient) : AuthenticationStateProvider, ILoginService
    {
        private readonly IJSRuntime _jSRuntime = jSRuntime;
        private readonly HttpClient _httpClient = httpClient;
        private readonly String _tokenKey = "TOKEN_KEY";
        private readonly AuthenticationState _anonimous = new(new ClaimsPrincipal(new ClaimsIdentity()));

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _jSRuntime.GetLocalStorage(_tokenKey);
            if (token is null) { return _anonimous; }
            return BuildAuthenticationState(token.ToString()!);
        }

        private AuthenticationState BuildAuthenticationState(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var claims = ParseClaimsFromJWT(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        private IEnumerable<Claim> ParseClaimsFromJWT(string token)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler(); var unserializedToken = jwtSecurityTokenHandler.ReadJwtToken(token);
            return unserializedToken.Claims;
        }

        public async Task LoginAsync(string token)
        {
            await _jSRuntime.SetLocalStorage(_tokenKey, token);
            var authState = BuildAuthenticationState(token); NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task LogoutAsync()
        {
            await _jSRuntime.RemoveLocalStorage(_tokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = null; NotifyAuthenticationStateChanged(Task.FromResult(_anonimous));
        }
    }
}
