using GymWPF.Services.Interfaces;
using GymWPF.Services;
using GymWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Windows;
using GymWPF.Views;
using GymWPF.Views.Dialogs;

namespace GymWPF
{
	public partial class App : Application
	{
		private readonly IServiceProvider _serviceProvider;

		public App()
		{
			DotNetEnv.Env.Load();
			DotNetEnv.Env.TraversePath().Load();

			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);
			_serviceProvider = serviceCollection.BuildServiceProvider();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(new HttpClient { BaseAddress = new Uri("http://localhost") });

			services.AddTransient<IAuthenticationService, AuthenticationService>();
			services.AddTransient<IChipService, ChipService>();
			services.AddTransient<IMemberService, MemberService>();
			services.AddTransient<IMembershipService, MembershipService>();
			services.AddSingleton<INavigationService, NavigationService>();

			services.AddTransient<MainViewModel>();
			services.AddTransient<AddMembershipViewModel>();
			services.AddTransient<AddMemberViewModel>();
			services.AddTransient<AddChipViewModel>();
			services.AddTransient<ChangeChipViewModel>();
			services.AddTransient<UserInfoViewModel>();

			services.AddTransient<MainView>();
			services.AddTransient<AddMembershipView>();
			services.AddTransient<AddMemberView>();
			services.AddTransient<AddChipView>();
			services.AddTransient<ChangeChipView>();
			services.AddTransient<UserInfoView>();
			services.AddTransient<LargeMembershipView>();
			services.AddTransient<LargeChipView>();
			services.AddTransient<AllUsersView>();

			services.AddSingleton<INavigationService, NavigationService>();
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var username = Environment.GetEnvironmentVariable("USERNAME");
			var password = Environment.GetEnvironmentVariable("PASSWORD");

			var authService = _serviceProvider.GetRequiredService<IAuthenticationService>();
			var isAuthenticated = await authService.AuthenticateAsync(username, password);

			if (!string.IsNullOrEmpty(isAuthenticated))
			{
				var navigationService = _serviceProvider.GetRequiredService<INavigationService>();

				navigationService.RegisterWindow("Main", typeof(MainView));
				navigationService.RegisterWindow("AddMembership", typeof(AddMembershipView));
				navigationService.RegisterWindow("AddMember", typeof(AddMemberView));
				navigationService.RegisterWindow("AddChip", typeof(AddChipView));
				navigationService.RegisterWindow("ChangeChip", typeof(ChangeChipView));
				navigationService.RegisterWindow("UserInfo", typeof(UserInfoView));
				navigationService.RegisterWindow("LargeMembership", typeof(LargeMembershipView));
				navigationService.RegisterWindow("LargeChip", typeof(LargeChipView));
				navigationService.RegisterWindow("AllUsers", typeof(AllUsersView));


				navigationService.NavigateTo("Main");

				var mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault();
				if (mainView?.DataContext is MainViewModel mainViewModel)
				{
					mainViewModel.RefreshDataCommand.Execute(null);
				}
			}
			else
			{
				MessageBox.Show("Nepodarilo sa autentifikovať. Aplikácia sa teraz ukončí.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				Shutdown();
			}
		}
	}
}
