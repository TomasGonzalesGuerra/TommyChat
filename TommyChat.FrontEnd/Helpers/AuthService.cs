using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TommyChat.FrontEnd.Helpers
{
    public class AuthService(IJSRuntime jSRuntime)
    {
        private readonly IJSRuntime _jSRuntime = jSRuntime;
        private readonly String _tokenKey = "TOKEN_KEY";

        public async Task<string?> GetUserRoleAsync()
        {
            string token = (string)await _jSRuntime.GetLocalStorage(_tokenKey);
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            return roleClaim?.Value;
        }
    }
}


//@inject AuthService AuthService
//@code {
//    private string userRole;

//protected override async Task OnInitializedAsync()
//{
//    userRole = await AuthService.GetUserRoleAsync();
//}
//}

//< nav >
//    < a href = "/" > Home </ a >

//    @if(userRole == "SuperAdmin")
//    {
//        < a href = "/admin-dashboard" > Admin Panel </ a >
//    }

//@if(!string.IsNullOrEmpty(userRole))
//    {
//        < a href = "/contacts" > Contacts </ a >
//        < a href = "/groups" > Groups </ a >
//    }

//@if(string.IsNullOrEmpty(userRole))
//    {
//        < a href = "/login" > Login </ a >
//        < a href = "/register" > Register </ a >
//    }
//</ nav >