using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GymWPF.Models;
using GymWPF.Services.Interfaces;

/// <summary>
/// Service for managing user authentication and token handling.
/// Provides functionality for login, token management, and automatic token refresh.
/// </summary>
public class AuthenticationService : BaseService, IAuthenticationService
{
	private string _token;
	private string _username;
	private string _password;
	private System.Timers.Timer _refreshTimer;

	/// <summary>
	/// Initializes a new instance of the <see cref="AuthenticationService"/> class.
	/// </summary>
	/// <param name="httpClient">The HTTP client used for making API requests.</param>
	public AuthenticationService(HttpClient httpClient) : base(httpClient) { }

	/// <summary>
	/// Authenticates a user with the given username and password.
	/// Retrieves and sets the JWT token and starts the token refresh timer.
	/// </summary>
	/// <param name="username">The username of the user.</param>
	/// <param name="password">The password of the user.</param>
	/// <returns>The JWT token as a string.</returns>
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

	/// <summary>
	/// Manually sets the JWT token and updates the Authorization header.
	/// </summary>
	/// <param name="token">The JWT token.</param>
	public void SetToken(string token)
	{
		_token = token;
		SetAuthorizationHeader(token);
	}

	/// <summary>
	/// Refreshes the JWT token by re-authenticating the user with stored credentials.
	/// </summary>
	public async Task RefreshTokenAsync()
	{
		if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
			return;

		var newToken = await AuthenticateAsync(_username, _password);
		SetToken(newToken);
	}

	/// <summary>
	/// Starts a timer to refresh the token before it expires.
	/// </summary>
	/// <param name="expiryDate">The expiry date of the current token.</param>
	private void StartTokenRefreshTimer(DateTime expiryDate)
	{
		var refreshInterval = expiryDate.Subtract(DateTime.UtcNow).TotalMilliseconds - TimeSpan.FromMinutes(5).TotalMilliseconds;
		if (refreshInterval <= 0)
		{
			refreshInterval = TimeSpan.FromSeconds(30).TotalMilliseconds;
		}

		_refreshTimer = new System.Timers.Timer(refreshInterval);
		_refreshTimer.Elapsed += async (sender, e) => await RefreshTokenAsync();
		_refreshTimer.AutoReset = false;
		_refreshTimer.Start();
	}
}
