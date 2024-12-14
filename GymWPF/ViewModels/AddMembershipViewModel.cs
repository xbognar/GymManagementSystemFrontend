using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GymWPF.ViewModels
{
	/// <summary>
	/// ViewModel for the "Add Membership" window.
	/// Handles the creation of memberships, member selection, and related business logic.
	/// </summary>
	public class AddMembershipViewModel : ObservableObject
	{
		// Services for managing members, memberships, and navigation.
		private readonly IMemberService _memberService;
		private readonly IMembershipService _membershipService;
		private readonly INavigationService _navigationService;

		// Commands for creating a membership and canceling the operation.
		public ICommand CreateMembershipCommand { get; }
		public ICommand CancelCommand { get; }

		public ObservableCollection<string> MemberNames { get; } = new ObservableCollection<string>();
		public ObservableCollection<string> MembershipTypes { get; } = new ObservableCollection<string> { "1 mesiac", "3 mesiace", "6 mesiacov" };
		public List<string> IsActiveOptions { get; } = new List<string> { "Áno", "Nie" };

		private string _selectedMember;
		/// <summary>
		/// Gets or sets the selected member from the ComboBox.
		/// </summary>
		public string SelectedMember
		{
			get => _selectedMember;
			set => SetProperty(ref _selectedMember, value);
		}

		private string _selectedMembershipType = "1 mesiac";
		/// <summary>
		/// Gets or sets the selected membership type.
		/// Updates the end date automatically when changed.
		/// </summary>
		public string SelectedMembershipType
		{
			get => _selectedMembershipType;
			set
			{
				if (SetProperty(ref _selectedMembershipType, value))
				{
					UpdateEndDateAutomatically();
				}
			}
		}

		private string _selectedIsActive = "Áno";
		/// <summary>
		/// Gets or sets the "IsActive" status.
		/// </summary>
		public string SelectedIsActive
		{
			get => _selectedIsActive;
			set => SetProperty(ref _selectedIsActive, value);
		}

		private DateTime _startDate = DateTime.Now;
		/// <summary>
		/// Gets or sets the start date of the membership.
		/// Updates the end date automatically when changed.
		/// </summary>
		public DateTime StartDate
		{
			get => _startDate;
			set
			{
				if (SetProperty(ref _startDate, value))
				{
					UpdateEndDateAutomatically();
				}
			}
		}

		private DateTime _endDate = DateTime.Now;
		/// <summary>
		/// Gets or sets the end date of the membership.
		/// </summary>
		public DateTime EndDate
		{
			get => _endDate;
			set => SetProperty(ref _endDate, value);
		}


		/// <summary>
		/// Initializes a new instance of AddMembershipViewModel.
		/// Sets up commands and loads initial member data.
		/// </summary>
		/// <param name="memberService">Service for handling member data.</param>
		/// <param name="membershipService">Service for handling membership data.</param>
		/// <param name="navigationService">Service for handling navigation between windows.</param>
		public AddMembershipViewModel(IMemberService memberService, IMembershipService membershipService, INavigationService navigationService)
		{
			_memberService = memberService;
			_membershipService = membershipService;
			_navigationService = navigationService;

			CreateMembershipCommand = new AsyncRelayCommand(CreateMembershipAsync);
			CancelCommand = new RelayCommand(Cancel);

			LoadMembersAsync();
			SelectedIsActive = "Áno";
			UpdateEndDateAutomatically();
		}

		/// <summary>
		/// Loads all members asynchronously and populates the MemberNames collection.
		/// </summary>
		private async void LoadMembersAsync()
		{
			var members = await _memberService.GetAllMembersAsync();
			if (members != null)
			{
				foreach (var member in members)
				{
					MemberNames.Add($"{member.FirstName} {member.LastName}");
				}
			}
		}

		/// <summary>
		/// Updates the end date automatically based on the selected membership type and start date.
		/// </summary>
		private void UpdateEndDateAutomatically()
		{
			int monthsToAdd = 1;
			if (SelectedMembershipType == "3 mesiace") monthsToAdd = 3;
			else if (SelectedMembershipType == "6 mesiacov") monthsToAdd = 6;

			EndDate = StartDate.AddMonths(monthsToAdd);
		}

		/// <summary>
		/// Creates a new membership based on user input and adds it to the database.
		/// Validates user input and displays appropriate messages.
		/// </summary>
		private async Task CreateMembershipAsync()
		{
			if (SelectedMember == null)
			{
				MessageBox.Show("Prosím vyberte člena.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var memberId = await _memberService.GetMemberIdByNameAsync(SelectedMember);
			if (!memberId.HasValue)
			{
				MessageBox.Show("Zvolený člen nebol nájdený.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var newMembership = new Membership
			{
				MemberID = memberId.Value,
				StartDate = StartDate,
				EndDate = EndDate,
				PaymentType = SelectedMembershipType,
				IsActive = SelectedIsActive == "Áno"
			};

			bool success = await _membershipService.AddMembershipAsync(newMembership);

			if (success)
			{
				MessageBox.Show("Členstvo bolo úspešne vytvorené!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
				ClearData();
			}
			else
			{
				MessageBox.Show("Nepodarilo sa vytvoriť členstvo.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Clears the form data, resetting all fields to their default values.
		/// </summary>
		public void ClearData()
		{
			SelectedMember = null;
			SelectedMembershipType = "1 mesiac";
			SelectedIsActive = "Áno";
			StartDate = DateTime.Now;
			UpdateEndDateAutomatically();
		}

		/// <summary>
		/// Refreshes the member data in the ComboBox.
		/// </summary>
		public async Task RefreshData()
		{
			MemberNames.Clear();
			var members = await _memberService.GetAllMembersAsync();
			if (members != null)
			{
				foreach (var member in members)
				{
					MemberNames.Add($"{member.FirstName} {member.LastName}");
				}
			}
		}

		/// <summary>
		/// Cancels the "Add Membership" operation and closes the window.
		/// </summary>
		private void Cancel()
		{
			_navigationService.CloseWindow("AddMembership");
		}
	}
}
