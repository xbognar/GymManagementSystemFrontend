﻿using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class MembershipService : BaseService, IMembershipService
{
	public MembershipService(HttpClient httpClient) : base(httpClient) { }

	public async Task<Membership?> GetMembershipByIdAsync(int id)
	{
		var response = await _httpClient.GetAsync($"/api/Memberships/{id}");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Membership>() : null;
	}

	public async Task<IEnumerable<Membership>?> GetAllMembershipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Memberships");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<Membership>>() : null;
	}

	public async Task<bool> AddMembershipAsync(Membership membership)
	{
		var response = await _httpClient.PostAsJsonAsync("/api/Memberships", membership);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> UpdateMembershipAsync(int id, Membership membership)
	{
		var response = await _httpClient.PutAsJsonAsync($"/api/Memberships/{id}", membership);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteMembershipAsync(int id)
	{
		var response = await _httpClient.DeleteAsync($"/api/Memberships/{id}");
		return response.IsSuccessStatusCode;
	}

	public async Task<IEnumerable<MembershipDTO>?> GetActiveMembershipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Memberships/active");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<MembershipDTO>>() : null;
	}

	public async Task<IEnumerable<MembershipDTO>?> GetInactiveMembershipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Memberships/inactive");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<MembershipDTO>>() : null;
	}

	public async Task<IEnumerable<UserMembershipsDTO>?> GetUserMembershipsAsync(int memberId)
	{
		var response = await _httpClient.GetAsync($"/api/Memberships/user/{memberId}/memberships");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<UserMembershipsDTO>>() : null;
	}
}
