using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;
using GymWPF.Services;
using GymWPF.Services.Interfaces;
using GymWPF.ViewModels;
using GymWPF.Views;
using GymWPF.Views.Dialogs;

namespace GymWPF
{
	public partial class App : Application
	{
		private readonly ServiceProvider _serviceProvider;

		public App()
		{
			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);
			_serviceProvider = serviceCollection.BuildServiceProvider();
		}

		private async Task AuthenticateAndStartApp()
		{
			var authenticationService = _serviceProvider.GetRequiredService<IAuthenticationService>();
			var navigationService = _serviceProvider.GetRequiredService<INavigationService>();

			var isAuthenticated = await authenticationService.LoginAsync();

			if (!isAuthenticated)
			{
				MessageBox.Show("Failed to authenticate. Please check your credentials and try again.");
				Shutdown();
				return;
			}

			navigationService.Configure("Main", typeof(MainView));
			navigationService.Configure("AddMembership", typeof(AddMembershipView));
			navigationService.Configure("AddMember", typeof(AddMemberView));
			navigationService.Configure("AddChip", typeof(AddChipView));
			navigationService.Configure("ChangeChip", typeof(ChangeChipView));
			navigationService.Configure("UserInfo", typeof(UserInfoView));

			var mainView = _serviceProvider.GetRequiredService<MainView>();
			((NavigationService)navigationService).SetMainContent(mainView);

			navigationService.NavigateTo("Main");

			var mainWindow = new Window
			{
				Content = mainView,
				Title = "Gym Management System",
				Width = SystemParameters.WorkArea.Width * 0.85,
				Height = SystemParameters.WorkArea.Height * 0.85,
				WindowStartupLocation = WindowStartupLocation.CenterScreen
			};

			mainWindow.Show();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			// Register services
			services.AddSingleton<IBaseService, BaseService>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<IChipService, ChipService>();
			services.AddScoped<IMemberService, MemberService>();
			services.AddScoped<IMembershipService, MembershipService>();

			// Register ViewModels
			services.AddTransient<MainViewModel>();
			services.AddTransient<AddMembershipViewModel>();
			services.AddTransient<AddMemberViewModel>();
			services.AddTransient<AddChipViewModel>();
			services.AddTransient<ChangeChipViewModel>();
			services.AddTransient<UserInfoViewModel>();

			// Register Views
			services.AddTransient<MainView>();
			services.AddTransient<AddMembershipView>();
			services.AddTransient<AddMemberView>();
			services.AddTransient<AddChipView>();
			services.AddTransient<ChangeChipView>();
			services.AddTransient<UserInfoView>();

			// Register and configure NavigationService
			services.AddSingleton<INavigationService, NavigationService>();
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			await AuthenticateAndStartApp();
		}
	}
}
