using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class MemberService : BaseService, IMemberService
{
	public MemberService(HttpClient httpClient) : base(httpClient) { }

	public async Task<Member?> GetMemberByIdAsync(int id)
	{
		var response = await _httpClient.GetAsync($"/api/Members/{id}");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Member>() : null;
	}

	public async Task<IEnumerable<Member>?> GetAllMembersAsync()
	{
		var response = await _httpClient.GetAsync("/api/Members");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<Member>>() : null;
	}

	public async Task<bool> AddMemberAsync(Member member)
	{
		var response = await _httpClient.PostAsJsonAsync("/api/Members", member);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> UpdateMemberAsync(int id, Member member)
	{
		var response = await _httpClient.PutAsJsonAsync($"/api/Members/{id}", member);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteMemberAsync(int id)
	{
		var response = await _httpClient.DeleteAsync($"/api/Members/{id}");
		return response.IsSuccessStatusCode;
	}

	public async Task<int?> GetMemberIdByNameAsync(string fullName)
	{
		var response = await _httpClient.GetAsync($"/api/Members/getMemberIdByName?fullName={fullName}");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<int?>() : null;
	}
}
