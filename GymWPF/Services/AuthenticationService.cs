using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GymWPF.Models;
using GymWPF.Services.Interfaces;

public class AuthenticationService : BaseService, IAuthenticationService
{
    private string _token;
    private string _username;
    private string _password;
    private System.Timers.Timer _refreshTimer;

    public AuthenticationService(HttpClient httpClient) : base(httpClient) { }

    public async Task<string> AuthenticateAsync(string username, string password)
    {
        _username = username;
        _password = password;

        var loginModel = new { Username = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginModel);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        _token = tokenResponse?.Token ?? string.Empty;
        SetAuthorizationHeader(_token);

        StartTokenRefreshTimer(tokenResponse?.ExpiryDate ?? DateTime.UtcNow.AddMinutes(120));

        return _token;
    }

    public void SetToken(string token)
    {
        _token = token;
        SetAuthorizationHeader(token);
    }

    public async Task RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            return;

        var newToken = await AuthenticateAsync(_username, _password);
        SetToken(newToken);
    }

    private void StartTokenRefreshTimer(DateTime expiryDate)
    {
        var refreshInterval = expiryDate.Subtract(DateTime.UtcNow).TotalMilliseconds * 0.8; // refresh before expiry
        _refreshTimer = new System.Timers.Timer(refreshInterval);
        _refreshTimer.Elapsed += async (sender, e) => await RefreshTokenAsync();
        _refreshTimer.AutoReset = false;
        _refreshTimer.Start();
    }
}
