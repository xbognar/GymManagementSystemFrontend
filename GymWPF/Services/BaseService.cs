using GymWPF.Services.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;


public class HttpClientSingleton
{
	private static readonly Lazy<HttpClient> _client = new Lazy<HttpClient>(() =>
	{
		var client = new HttpClient
		{
			BaseAddress = new Uri("http://localhost")
		};
		return client;
	});

	public static HttpClient Instance => _client.Value;

	private HttpClientSingleton() { }
}



public class BaseService : IBaseService
{

	protected HttpClient httpClient => HttpClientSingleton.Instance;

	public void SetAuthorizationHeader(string token)
	{
		if (!string.IsNullOrEmpty(token))
		{
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			
		}
		else
		{
			httpClient.DefaultRequestHeaders.Authorization = null;
		}


	}

}
