using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeHub.Data.Models
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //var httpContext = _httpContextAccessor.HttpContext;
            //var identity = new ClaimsIdentity();

            //if (httpContext != null && httpContext.Request.Cookies.TryGetValue("authToken", out string? token))
            //{
            //    var tokenHandler = new JwtSecurityTokenHandler();
            //    var jwtToken = tokenHandler.ReadJwtToken(token);
            //    identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            //}

            //var user = new ClaimsPrincipal(identity);
            //return Task.FromResult(new AuthenticationState(user));
            var session = _httpContextAccessor.HttpContext?.Session;
            string token = session?.GetString("authToken");

            ClaimsIdentity identity;
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                }
                catch
                {
                    identity = new ClaimsIdentity();
                }
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }

        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            return ParseClaimsFromJwt(token);
        }

        public void TriggerAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public string? GetLoggedInUserId()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            string token = session?.GetString("authToken");

            if (!string.IsNullOrEmpty(token))
            {
                var claims = ParseClaimsFromJwt(token);
                var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return userIdClaim?.Value;
            }

            return null;
        }
    }
}
