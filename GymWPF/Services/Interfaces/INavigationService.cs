using System;

namespace GymWPF.Services.Interfaces
{
	public interface INavigationService
	{
		void RegisterWindow(string key, Type windowType);
		void NavigateTo(string key);
		void CloseWindow(string key);
	}
}
