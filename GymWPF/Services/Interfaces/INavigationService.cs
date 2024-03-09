using System.Windows.Controls;

namespace GymWPF.Services.Interfaces
{
    public interface INavigationService
    {

		void NavigateTo(string pageKey);

		void Configure(string key, Type pageType);

		void SetMainContent(ContentControl mainContent);

	}
}