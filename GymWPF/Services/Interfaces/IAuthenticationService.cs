using System.Threading.Tasks;

namespace GymWPF.Services.Interfaces
{
	public interface IAuthenticationService
	{
		Task<string> AuthenticateAsync(string username, string password);
		Task RefreshTokenAsync();
		void SetToken(string token);
	}
}
