using System;
using System.Threading.Tasks;

namespace GymWPF.Services.Interfaces
{
	public interface IAuthenticationService
	{
		
		bool IsAuthenticated { get; }

		Task<bool> LoginAsync();

		Task<T> ExecuteWithAutoRefreshAsync<T>(Func<Task<T>> operation);

	}
}
