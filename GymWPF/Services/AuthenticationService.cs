using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GymWPF.Services
{
	public class AuthenticationService : BaseService, IAuthenticationService
	{
		public TokenResponse tokenResponse;
		private readonly string username = "RozsaTomi";
		private readonly string password = "TomiFit123";

		public AuthenticationService() : base() { }

		public bool IsAuthenticated => tokenResponse != null && DateTime.UtcNow < tokenResponse.ExpiryDate;

		public async Task<bool> LoginAsync()
		{
			var loginModel = new { username, password };
			var response = await httpClient.PostAsJsonAsync("/api/Auth/login", loginModel);

			if (response.IsSuccessStatusCode)
			{
				var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
				tokenResponse.ExpiryDate = DateTime.UtcNow.AddHours(1); 
				SetAuthorizationHeader(tokenResponse.Token);
				return true;
			}
			else
			{
				return false;
			}
		}

		public async Task<T> ExecuteWithAutoRefreshAsync<T>(Func<Task<T>> operation)
		{
			if (!IsAuthenticated)
			{
				if (!await LoginAsync())
				{
					throw new InvalidOperationException("Unable to re-authenticate.");
				}
			}

			return await operation();
		}
	}
}
