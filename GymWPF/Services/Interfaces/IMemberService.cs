using GymWPF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymWPF.Services.Interfaces
{
	public interface IMemberService
	{
		
		Task<Member?> GetMemberByIdAsync(int id);
		
		Task<IEnumerable<Member>?> GetAllMembersAsync();
		
		Task<bool> AddMemberAsync(Member member);
		
		Task<bool> UpdateMemberAsync(int id, Member member);
		
		Task<bool> DeleteMemberAsync(int id);
		
		Task<int?> GetMemberIdByNameAsync(string fullName);
	
	}
}
