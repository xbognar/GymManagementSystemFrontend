using System.Net.Http;
using System.Net.Http.Headers;

/// <summary>
/// Base service class providing common functionality for HTTP-based services.
/// Handles the setup of an HTTP client and authorization headers.
/// </summary>
public abstract class BaseService
{
	protected readonly HttpClient _httpClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="BaseService"/> class.
	/// </summary>
	/// <param name="httpClient">The HTTP client to use for making requests.</param>
	public BaseService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	/// <summary>
	/// Sets the Authorization header for the HTTP client using a bearer token.
	/// Clears the header if the token is null or empty.
	/// </summary>
	/// <param name="token">The bearer token to set in the Authorization header.</param>
	public void SetAuthorizationHeader(string token)
	{
		if (!string.IsNullOrEmpty(token))
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
		else
		{
			_httpClient.DefaultRequestHeaders.Authorization = null;
		}
	}
}
