using System;

namespace GymWPF.Services.Interfaces
{
	public interface INavigationService
	{
		void RegisterWindow(string key, Type windowType);
		void NavigateTo(string key, object viewModel = null);
		void CloseWindow(string key);
	}
}
