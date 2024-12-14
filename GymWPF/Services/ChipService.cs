using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

/// <summary>
/// Service for managing operations related to chips in the gym management system.
/// Provides functionality for creating, updating, deleting, and retrieving chips.
/// </summary>
public class ChipService : BaseService, IChipService
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ChipService"/> class.
	/// </summary>
	/// <param name="httpClient">The HTTP client used for making API requests.</param>
	public ChipService(HttpClient httpClient) : base(httpClient) { }

	/// <summary>
	/// Retrieves a chip by its ID.
	/// </summary>
	/// <param name="id">The ID of the chip.</param>
	/// <returns>The chip if found; otherwise, null.</returns>
	public async Task<Chip?> GetChipByIdAsync(int id)
	{
		var response = await _httpClient.GetAsync($"/api/Chips/{id}");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Chip>() : null;
	}

	/// <summary>
	/// Retrieves all chips.
	/// </summary>
	/// <returns>A collection of chips if successful; otherwise, null.</returns>
	public async Task<IEnumerable<Chip>?> GetAllChipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Chips");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<Chip>>() : null;
	}

	/// <summary>
	/// Adds a new chip.
	/// </summary>
	/// <param name="chip">The chip to add.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> AddChipAsync(Chip chip)
	{
		var response = await _httpClient.PostAsJsonAsync("/api/Chips", chip);
		return response.IsSuccessStatusCode;
	}

	/// <summary>
	/// Updates an existing chip.
	/// </summary>
	/// <param name="id">The ID of the chip to update.</param>
	/// <param name="request">The update request containing new chip information.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> UpdateChipAsync(int id, ChipUpdateRequest request)
	{
		var response = await _httpClient.PutAsJsonAsync($"/api/Chips/{id}", request);
		return response.IsSuccessStatusCode;
	}

	/// <summary>
	/// Deletes a chip by its ID.
	/// </summary>
	/// <param name="id">The ID of the chip to delete.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> DeleteChipAsync(int id)
	{
		var response = await _httpClient.DeleteAsync($"/api/Chips/{id}");
		return response.IsSuccessStatusCode;
	}

	/// <summary>
	/// Retrieves all active chips.
	/// </summary>
	/// <returns>A collection of active chips if successful; otherwise, null.</returns>
	public async Task<IEnumerable<ChipDTO>?> GetActiveChipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Chips/active");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<ChipDTO>>() : null;
	}

	/// <summary>
	/// Retrieves all inactive chips.
	/// </summary>
	/// <returns>A collection of inactive chips if successful; otherwise, null.</returns>
	public async Task<IEnumerable<ChipDTO>?> GetInactiveChipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Chips/inactive");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<ChipDTO>>() : null;
	}

	/// <summary>
	/// Retrieves chip information by the associated member's ID.
	/// </summary>
	/// <param name="memberId">The ID of the member.</param>
	/// <returns>The chip information as a string if found; otherwise, null.</returns>
	public async Task<string?> GetChipInfoByMemberIdAsync(int memberId)
	{
		var response = await _httpClient.GetAsync($"/api/Chips/infoByMember/{memberId}");
		return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
	}
}
