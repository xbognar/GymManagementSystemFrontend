using GymWPF.ViewModels;
using System.Windows;

namespace GymWPF.Views
{
	public partial class MainView : Window
	{
		public MainView(MainViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
