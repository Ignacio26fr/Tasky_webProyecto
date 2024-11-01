using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Tasky.Entidad.GmailAccount;
using Microsoft.AspNetCore.Authentication.Google;
using Tasky.Datos.EF;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace Tasky.Logica.Gmail;

public interface IGoogleAccountService
{
    Task<GoogleUserInfo> GetGoogleAccountAsync(AuthenticateResult authenticateResult);
}

public class GoogleAccountService : IGoogleAccountService
{

    private readonly UserManager<AspNetUsers> _userManager;

    public GoogleAccountService(UserManager<AspNetUsers> userManager)
    {
        _userManager = userManager;
    }


    public async Task<GoogleUserInfo> GetGoogleAccountAsync(AuthenticateResult authenticateResult)
    {
        if (authenticateResult == null)
            return null;

        var accessToken = authenticateResult.Properties.GetTokenValue("access_token");
        var refreshToken = authenticateResult.Properties.GetTokenValue("refresh_token");

       
        
        var claimsIdentity = authenticateResult.Principal.Identity as ClaimsIdentity;


        return new GoogleUserInfo
        {
            Email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value,
            PhoneNumber = claimsIdentity?.FindFirst(ClaimTypes.MobilePhone)?.Value,
            GivenName = claimsIdentity?.FindFirst(ClaimTypes.GivenName)?.Value,
            Surname = claimsIdentity?.FindFirst(ClaimTypes.Surname)?.Value,
            Picture = claimsIdentity?.FindFirst("picture")?.Value,
            AccessToken = accessToken,
            RefreshToken = refreshToken
            
        };
        

    }

    private async Task<GoogleUserInfo> GetUserInfoAsync(string accessToken)
    {
        using (var _httpClient = new HttpClient())
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

            if (response.IsSuccessStatusCode)
            {
                var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfo>();
                return userInfo;
            }
        }
        

        return null;
    }

}
