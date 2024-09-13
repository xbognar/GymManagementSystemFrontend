using GymWPF.ViewModels;
using System.Windows;

namespace GymWPF.Views.Dialogs
{
	public partial class AddChipView : Window
	{
		public AddChipView(AddChipViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
