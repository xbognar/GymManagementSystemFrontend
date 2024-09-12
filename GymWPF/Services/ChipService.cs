using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class ChipService : BaseService, IChipService
{
	public ChipService(HttpClient httpClient) : base(httpClient) { }

	public async Task<Chip?> GetChipByIdAsync(int id)
	{
		var response = await _httpClient.GetAsync($"/api/Chips/{id}");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Chip>() : null;
	}

	public async Task<IEnumerable<Chip>?> GetAllChipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Chips");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<Chip>>() : null;
	}

	public async Task<bool> AddChipAsync(Chip chip)
	{
		var response = await _httpClient.PostAsJsonAsync("/api/Chips", chip);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> UpdateChipAsync(int id, ChipUpdateRequest request)
	{
		var response = await _httpClient.PutAsJsonAsync($"/api/Chips/{id}", request);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteChipAsync(int id)
	{
		var response = await _httpClient.DeleteAsync($"/api/Chips/{id}");
		return response.IsSuccessStatusCode;
	}

	public async Task<IEnumerable<ChipDTO>?> GetActiveChipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Chips/active");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<ChipDTO>>() : null;
	}

	public async Task<IEnumerable<ChipDTO>?> GetInactiveChipsAsync()
	{
		var response = await _httpClient.GetAsync("/api/Chips/inactive");
		return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<ChipDTO>>() : null;
	}

	public async Task<string?> GetChipInfoByMemberIdAsync(int memberId)
	{
		var response = await _httpClient.GetAsync($"/api/Chips/infoByMember/{memberId}");
		return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
	}
}
