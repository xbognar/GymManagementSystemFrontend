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

			services.AddSingleton<INavigationService, NavigationService>();
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var username = System.Environment.GetEnvironmentVariable("USERNAME");
			var password = System.Environment.GetEnvironmentVariable("PASSWORD");

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

				navigationService.NavigateTo("Main");
			}
			else
			{
				MessageBox.Show("Failed to authenticate. The application will now exit.");
				Shutdown();
			}
		}
	}
}
