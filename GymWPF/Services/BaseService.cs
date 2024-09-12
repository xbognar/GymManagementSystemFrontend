using System.Net.Http;
using System.Net.Http.Headers;

public abstract class BaseService
{
	protected readonly HttpClient _httpClient;

	public BaseService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

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
