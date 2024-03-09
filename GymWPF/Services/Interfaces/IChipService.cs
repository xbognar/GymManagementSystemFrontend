using GymWPF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymWPF.Services.Interfaces
{
	public interface IChipService
	{
		
		Task<Chip?> GetChipByIdAsync(int id);
		
		Task<IEnumerable<Chip>?> GetAllChipsAsync();
		
		Task<bool> AddChipAsync(Chip chip);
		
		Task<bool> UpdateChipAsync(int id, ChipUpdateRequest request);
		
		Task<bool> DeleteChipAsync(int id);
		
		Task<IEnumerable<ChipDTO>?> GetActiveChipsAsync();
		
		Task<IEnumerable<ChipDTO>?> GetInactiveChipsAsync();
		
		Task<string?> GetChipInfoByMemberIdAsync(int memberId);
	
	}
}
