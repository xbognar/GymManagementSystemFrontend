using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

/// <summary>
/// Service for managing operations related to members in the gym management system.
/// Provides functionality for creating, updating, deleting, and retrieving member information.
/// </summary>
public class MemberService : BaseService, IMemberService
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MemberService"/> class.
	/// </summary>
	/// <param name="httpClient">The HTTP client used for making API requests.</param>
	public MemberService(HttpClient httpClient) : base(httpClient) { }

	/// <summary>
	/// Retrieves a member by their ID.
	/// </summary>
	/// <param name="id">The ID of the member.</param>
	/// <returns>The member if found; otherwise, null.</returns>
	public async Task<Member?> GetMemberByIdAsync(int id)
	{
		var response = await _httpClient.GetAsync($"/api/Members/{id}");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Member>() : null;
	}

	/// <summary>
	/// Retrieves all members.
	/// </summary>
	/// <returns>A collection of members if successful; otherwise, null.</returns>
	public async Task<IEnumerable<Member>?> GetAllMembersAsync()
	{
		var response = await _httpClient.GetAsync("/api/Members");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<Member>>() : null;
	}

	/// <summary>
	/// Adds a new member to the system.
	/// </summary>
	/// <param name="member">The member to add.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> AddMemberAsync(Member member)
	{
		var response = await _httpClient.PostAsJsonAsync("/api/Members", member);
		return response.IsSuccessStatusCode;
	}

	/// <summary>
	/// Updates an existing member's details.
	/// </summary>
	/// <param name="id">The ID of the member to update.</param>
	/// <param name="member">The updated member information.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> UpdateMemberAsync(int id, Member member)
	{
		var response = await _httpClient.PutAsJsonAsync($"/api/Members/{id}", member);
		return response.IsSuccessStatusCode;
	}

	/// <summary>
	/// Deletes a member by their ID.
	/// </summary>
	/// <param name="id">The ID of the member to delete.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> DeleteMemberAsync(int id)
	{
		var response = await _httpClient.DeleteAsync($"/api/Members/{id}");
		return response.IsSuccessStatusCode;
	}

	/// <summary>
	/// Retrieves a member's ID by their full name.
	/// </summary>
	/// <param name="fullName">The full name of the member.</param>
	/// <returns>The member's ID if found; otherwise, null.</returns>
	public async Task<int?> GetMemberIdByNameAsync(string fullName)
	{
		var response = await _httpClient.GetAsync($"/api/Members/getMemberIdByName?fullName={fullName}");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<int?>() : null;
	}
}
