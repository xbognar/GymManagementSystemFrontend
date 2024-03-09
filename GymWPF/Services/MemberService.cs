using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GymWPF.Services
{
	public class MemberService : BaseService, IMemberService
	{
		public MemberService() : base() { }

		public async Task<Member?> GetMemberByIdAsync(int id)
		{
			var response = await httpClient.GetAsync($"/api/Members/{id}");
			return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Member>() : null;
		}

		public async Task<IEnumerable<Member>?> GetAllMembersAsync()
		{
			var response = await httpClient.GetAsync("/api/Members");
			return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<IEnumerable<Member>>() : null;
		}

		public async Task<bool> AddMemberAsync(Member member)
		{
			var response = await httpClient.PostAsJsonAsync("/api/Members", member);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> UpdateMemberAsync(int id, Member member)
		{
			var response = await httpClient.PutAsJsonAsync($"/api/Members/{id}", member);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteMemberAsync(int id)
		{
			var response = await httpClient.DeleteAsync($"/api/Members/{id}");
			return response.IsSuccessStatusCode;
		}

		public async Task<int?> GetMemberIdByNameAsync(string fullName)
		{
			var response = await httpClient.GetAsync($"/api/Members/getMemberIdByName?fullName={fullName}");
			if (response.IsSuccessStatusCode)
			{
				var memberId = await response.Content.ReadFromJsonAsync<int>();
				return memberId;
			}
			return null;
		}
	}
}
