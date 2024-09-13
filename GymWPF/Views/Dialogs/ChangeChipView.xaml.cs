using GymWPF.ViewModels;
using System.Windows;

namespace GymWPF.Views.Dialogs
{
	public partial class ChangeChipView : Window
	{
		public ChangeChipView(ChangeChipViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel; 
		}
	}
}
