using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Soda.Bee.Shared.Consts;
using System.Security.Claims;
using System.Text.Json;

namespace Soda.Bee.Web.Auth;

public interface ILoginService
{
    Task Login(string token);

    Task Logout();
}

public class AuthProvider : AuthenticationStateProvider, ILoginService
{
    private readonly ILocalStorageService _localStorageService;
    private AuthenticationState anonimo => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    public AuthProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await _localStorageService.GetItemAsStringAsync(GlobalKeys.ClientTokenKey);

        if (string.IsNullOrWhiteSpace(savedToken))
        {
            return anonimo;
        }

        return BuildAuthenticationState(savedToken);
    }

    private AuthenticationState BuildAuthenticationState(string token)
    {
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("token", token) }, "token")));
    }

    public async Task Login(string token)
    {
        await _localStorageService.SetItemAsStringAsync(GlobalKeys.ClientTokenKey, token);
        var authState = BuildAuthenticationState(token);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public async Task Logout()
    {
        await _localStorageService.RemoveItemAsync(GlobalKeys.ClientTokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(anonimo));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        keyValuePairs!.TryGetValue(ClaimTypes.Role, out object? roles);

        if (roles != null)
        {
            if (roles.ToString()!.Trim().StartsWith("["))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);

                foreach (var parsedRole in parsedRoles!)
                {
                    claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }

        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}