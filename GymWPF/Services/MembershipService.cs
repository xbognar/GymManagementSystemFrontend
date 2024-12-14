using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

/// <summary>
/// Service for managing operations related to memberships in the gym management system.
/// Provides functionality for creating, updating, deleting, and retrieving membership information.
/// </summary>
public class MembershipService : BaseService, IMembershipService
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MembershipService"/> class.
	/// </summary>
	/// <param name="httpClient">The HTTP client used for making API requests.</param>
	public MembershipService(HttpClient httpClient) : base(httpClient) { }

	/// <summary>
	/// Retrieves a membership by its ID.
	/// </summary>
	/// <param name="id">The ID of the membership.</param>
	/// <returns>The membership if found; otherwise, null.</returns>
	public async Task<Membership?> GetMembershipByIdAsync(int id)
	{
		var response = await _httpClient.GetAsync($"/api/Memberships/{id}");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Membership>() : null;
	}

	/// <summary>
	/// Retrieves all memberships.
	/// </summary>
	/// <returns>A collection of memberships if successful; otherwise, null.</returns>
	public async Task<IEnumerable<Membership>?> GetAllMembershipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Memberships");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<Membership>>() : null;
	}

	/// <summary>
	/// Adds a new membership.
	/// </summary>
	/// <param name="membership">The membership to add.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> AddMembershipAsync(Membership membership)
	{
		var response = await _httpClient.PostAsJsonAsync("/api/Memberships", membership);
		return response.IsSuccessStatusCode;
	}

	/// <summary>
	/// Updates an existing membership's details.
	/// </summary>
	/// <param name="id">The ID of the membership to update.</param>
	/// <param name="membership">The updated membership information.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> UpdateMembershipAsync(int id, Membership membership)
	{
		var response = await _httpClient.PutAsJsonAsync($"/api/Memberships/{id}", membership);
		return response.IsSuccessStatusCode;
	}

	/// <summary>
	/// Deletes a membership by its ID.
	/// </summary>
	/// <param name="id">The ID of the membership to delete.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> DeleteMembershipAsync(int id)
	{
		var response = await _httpClient.DeleteAsync($"/api/Memberships/{id}");
		return response.IsSuccessStatusCode;
	}

	/// <summary>
	/// Retrieves all active memberships.
	/// </summary>
	/// <returns>A collection of active memberships if successful; otherwise, null.</returns>
	public async Task<IEnumerable<MembershipDTO>?> GetActiveMembershipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Memberships/active");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<MembershipDTO>>() : null;
	}

	/// <summary>
	/// Retrieves all inactive memberships.
	/// </summary>
	/// <returns>A collection of inactive memberships if successful; otherwise, null.</returns>
	public async Task<IEnumerable<MembershipDTO>?> GetInactiveMembershipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Memberships/inactive");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<MembershipDTO>>() : null;
	}

	/// <summary>
	/// Retrieves all memberships associated with a specific user.
	/// </summary>
	/// <param name="memberId">The ID of the member.</param>
	/// <returns>A collection of memberships for the specified user if successful; otherwise, null.</returns>
	public async Task<IEnumerable<UserMembershipsDTO>?> GetUserMembershipsAsync(int memberId)
	{
		var response = await _httpClient.GetAsync($"/api/Memberships/user/{memberId}/memberships");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<UserMembershipsDTO>>() : null;
	}
}
