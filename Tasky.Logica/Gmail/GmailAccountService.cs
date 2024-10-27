using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Tasky.Entidad.GmailAccount;
using Microsoft.AspNetCore.Authentication.Google;
using Tasky.Datos.EF;
using Microsoft.AspNetCore.Identity;


namespace Tasky.Logica.Gmail;

public interface IGmailAccountService
{
    AuthenticationProperties GetGoogleAuthProperties(string redirectUrl);
    ChallengeResult Challenge(AuthenticationProperties properties);
    Task<AspNetUsers> GetAccount(AuthenticateResult authenticateResult);
}

public class GmailAccountService : IGmailAccountService
{

    private readonly HttpClient _httpClient;
    private readonly UserManager<AspNetUsers> _userManager;
  
    public GmailAccountService(HttpClient httpClient, UserManager<AspNetUsers> userManager)
    {
        _httpClient = httpClient;
        _userManager = userManager;
  
    }


    public AuthenticationProperties GetGoogleAuthProperties(string redirectUrl)
    {
        return new AuthenticationProperties { RedirectUri = redirectUrl };

    }

    public ChallengeResult Challenge(AuthenticationProperties properties)
    {
        return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);
    }

    public async Task<AspNetUsers> GetAccount(AuthenticateResult authenticateResult)
    {
       

        var accessToken = authenticateResult.Properties.GetTokenValue("access_token");

        if (string.IsNullOrEmpty(accessToken)) 
            return null;

      

        var userInfo = await GetUserInfoAsync(accessToken);

        if (userInfo == null)
            return null;

        var user = await _userManager.FindByEmailAsync(userInfo.Email);

        //Aca podria comparar para ver si el usuario tiene el mismo access en la bd 

        return user;
        
      /*  return new GmailAccount
        {
            Name = userInfo.Name,
            Email = userInfo.Email,
            Picture = userInfo.Picture,
            AccessToken = accessToken,
          //  SuscriptionId = subscriptionId  */

        
    }

    private async Task<GoogleUserInfo> GetUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

        if (response.IsSuccessStatusCode)
        {
            var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfo>();
            return userInfo;
        }

        return null;
    }





}
