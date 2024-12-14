using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GymWPF.ViewModels
{
	/// <summary>
	/// ViewModel for the "Add Member" window.
	/// Handles the creation of a new member, including validation and interaction with the member service.
	/// </summary>
	public class AddMemberViewModel : BaseViewModel
	{
		// Services for managing members and navigation.
		private readonly IMemberService _memberService;
		private readonly INavigationService _navigationService;

		// Commands for canceling the operation and adding a new member.
		public ICommand CancelCommand { get; }
		public ICommand AddMemberCommand { get; }

		private string _firstName;
		/// <summary>
		/// Gets or sets the first name of the new member.
		/// </summary>
		public string FirstName
		{
			get => _firstName;
			set => SetProperty(ref _firstName, value);
		}

		private string _lastName;
		/// <summary>
		/// Gets or sets the last name of the new member.
		/// </summary>
		public string LastName
		{
			get => _lastName;
			set => SetProperty(ref _lastName, value);
		}

		private DateTime _dateOfBirth = DateTime.Now;
		/// <summary>
		/// Gets or sets the date of birth of the new member.
		/// </summary>
		public DateTime DateOfBirth
		{
			get => _dateOfBirth;
			set => SetProperty(ref _dateOfBirth, value);
		}

		private string _email;
		/// <summary>
		/// Gets or sets the email of the new member.
		/// </summary>
		public string Email
		{
			get => _email;
			set => SetProperty(ref _email, value);
		}

		private string _phoneNumber;
		/// <summary>
		/// Gets or sets the phone number of the new member.
		/// </summary>
		public string PhoneNumber
		{
			get => _phoneNumber;
			set => SetProperty(ref _phoneNumber, value);
		}

		/// <summary>
		/// Initializes a new instance of AddMemberViewModel.
		/// Sets up commands and initializes necessary services.
		/// </summary>
		/// <param name="memberService">Service for managing member data.</param>
		/// <param name="navigationService">Service for managing navigation between windows.</param>
		public AddMemberViewModel(IMemberService memberService, INavigationService navigationService)
		{
			_memberService = memberService;
			_navigationService = navigationService;

			CancelCommand = new RelayCommand(CloseWindow);
			AddMemberCommand = new AsyncRelayCommand(AddMemberAsync);
		}

		/// <summary>
		/// Adds a new member after validating input data.
		/// Displays success or error messages based on the operation's outcome.
		/// </summary>
		private async Task AddMemberAsync()
		{
			if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
			{
				MessageBox.Show("Prosím, zadajte meno aj priezvisko bez medzier.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (FirstName.Contains(" ") || LastName.Contains(" "))
			{
				MessageBox.Show("Meno a priezvisko nesmú obsahovať medzery.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var existingMembers = await _memberService.GetAllMembersAsync();
			if (existingMembers != null && existingMembers.Any(m => m.FirstName == FirstName && m.LastName == LastName))
			{
				MessageBox.Show("Člen s rovnakým menom a priezviskom už existuje.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			Member newMember = new Member
			{
				FirstName = FirstName,
				LastName = LastName,
				DateOfBirth = DateOfBirth,
				Email = Email,
				PhoneNumber = PhoneNumber
			};

			bool success = await _memberService.AddMemberAsync(newMember);

			if (success)
			{
				MessageBox.Show("Člen bol úspešne pridaný!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
				ClearData();
			}
			else
			{
				MessageBox.Show("Nepodarilo sa pridať člena.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Clears the form data, resetting all fields to their default values.
		/// </summary>
		public void ClearData()
		{
			FirstName = string.Empty;
			LastName = string.Empty;
			DateOfBirth = DateTime.Now;
			Email = string.Empty;
			PhoneNumber = string.Empty;
		}

		/// <summary>
		/// Cancels the "Add Member" operation and closes the window.
		/// </summary>
		private void CloseWindow()
		{
			_navigationService.CloseWindow("AddMember");
		}
	}
}
