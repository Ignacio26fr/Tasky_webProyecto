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

        var claimsIdentity = authenticateResult.Principal.Identity as ClaimsIdentity;

        return new GoogleUserInfo
        {
            Email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value,
            PhoneNumber = claimsIdentity?.FindFirst(ClaimTypes.MobilePhone)?.Value,
            GivenName = claimsIdentity?.FindFirst(ClaimTypes.GivenName)?.Value,
            Surname = claimsIdentity?.FindFirst(ClaimTypes.Surname)?.Value,
            Picture = claimsIdentity?.FindFirst(ClaimTypes.Uri)?.Value,
            AccessToken = accessToken
        };

    }

}
