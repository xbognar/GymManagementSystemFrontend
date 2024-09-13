using GymWPF.ViewModels;
using System.Windows;

namespace GymWPF.Views.Dialogs
{
	public partial class AddMembershipView : Window
	{
		public AddMembershipView(AddMembershipViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
