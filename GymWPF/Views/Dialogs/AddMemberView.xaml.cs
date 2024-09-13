using GymWPF.ViewModels;
using System.Windows;

namespace GymWPF.Views.Dialogs
{
	public partial class AddMemberView : Window
	{
		public AddMemberView(AddMemberViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
