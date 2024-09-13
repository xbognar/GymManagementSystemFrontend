using GymWPF.ViewModels;
using System.Windows;

namespace GymWPF.Views.Dialogs
{
	public partial class UserInfoView : Window
	{
		public UserInfoView(UserInfoViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
