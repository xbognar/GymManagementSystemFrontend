using GymWPF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymWPF.Services.Interfaces
{
	public interface IMembershipService
	{
		
		Task<Membership?> GetMembershipByIdAsync(int id);
		
		Task<IEnumerable<Membership>?> GetAllMembershipsAsync();
		
		Task<bool> AddMembershipAsync(Membership membership);
		
		Task<bool> UpdateMembershipAsync(int id, Membership membership);
		
		Task<bool> DeleteMembershipAsync(int id);
		
		Task<IEnumerable<MembershipDTO>?> GetActiveMembershipsAsync();
		
		Task<IEnumerable<MembershipDTO>?> GetInactiveMembershipsAsync();
		
		Task<IEnumerable<UserMembershipsDTO>?> GetUserMembershipsAsync(int memberId);
	}
}
