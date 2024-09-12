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
	public class AddMemberViewModel : BaseViewModel
	{
		private readonly IMemberService _memberService;
		private readonly INavigationService _navigationService;

		public ICommand CancelCommand { get; }
		public ICommand AddMemberCommand { get; }

		private string _firstName;
		public string FirstName
		{
			get => _firstName;
			set => SetProperty(ref _firstName, value);
		}

		private string _lastName;
		public string LastName
		{
			get => _lastName;
			set => SetProperty(ref _lastName, value);
		}

		private DateTime _dateOfBirth = DateTime.Now;
		public DateTime DateOfBirth
		{
			get => _dateOfBirth;
			set => SetProperty(ref _dateOfBirth, value);
		}

		private string _email;
		public string Email
		{
			get => _email;
			set => SetProperty(ref _email, value);
		}

		private string _phoneNumber;
		public string PhoneNumber
		{
			get => _phoneNumber;
			set => SetProperty(ref _phoneNumber, value);
		}

		public AddMemberViewModel(IMemberService memberService, INavigationService navigationService)
		{
			_memberService = memberService;
			_navigationService = navigationService;

			CancelCommand = new RelayCommand(CloseWindow);
			AddMemberCommand = new AsyncRelayCommand(AddMemberAsync);
		}

		private async Task AddMemberAsync()
		{
			if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
			{
				MessageBox.Show("Please provide both first name and last name without spaces.");
				return;
			}

			if (FirstName.Contains(" ") || LastName.Contains(" "))
			{
				MessageBox.Show("First name and last name cannot contain spaces.");
				return;
			}

			var existingMembers = await _memberService.GetAllMembersAsync();

			if (existingMembers != null && existingMembers.Any(m => m.FirstName == FirstName && m.LastName == LastName))
			{
				MessageBox.Show("A member with the same first and last name already exists.");
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
				MessageBox.Show("Member added successfully!");
				_navigationService.CloseWindow("AddMember");
			}
			else
			{
				MessageBox.Show("Failed to add member.");
			}
		}

		public void ClearData()
		{
			FirstName = string.Empty;
			LastName = string.Empty;
			DateOfBirth = DateTime.Now;
			Email = string.Empty;
			PhoneNumber = string.Empty;
		}

		private void CloseWindow()
		{
			_navigationService.CloseWindow("AddMember");
		}
	}
}
