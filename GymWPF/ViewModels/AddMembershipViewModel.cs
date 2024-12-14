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
	public class AddMembershipViewModel : ObservableObject
	{
		private readonly IMemberService _memberService;
		private readonly IMembershipService _membershipService;
		private readonly INavigationService _navigationService;

		public ObservableCollection<string> MemberNames { get; } = new ObservableCollection<string>();
		public ObservableCollection<string> MembershipTypes { get; } = new ObservableCollection<string> { "1 mesiac", "3 mesiace", "6 mesiacov" };
		public List<string> IsActiveOptions { get; } = new List<string> { "Áno", "Nie" };

		private string _selectedMember;
		public string SelectedMember
		{
			get => _selectedMember;
			set => SetProperty(ref _selectedMember, value);
		}

		private string _selectedMembershipType = "1 mesiac";
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
		public string SelectedIsActive
		{
			get => _selectedIsActive;
			set => SetProperty(ref _selectedIsActive, value);
		}

		private DateTime _startDate = DateTime.Now;
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
		public DateTime EndDate
		{
			get => _endDate;
			set => SetProperty(ref _endDate, value);
		}

		public ICommand CreateMembershipCommand { get; }
		public ICommand CancelCommand { get; }

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

		private void UpdateEndDateAutomatically()
		{
			int monthsToAdd = 1;
			if (SelectedMembershipType == "3 mesiace") monthsToAdd = 3;
			else if (SelectedMembershipType == "6 mesiacov") monthsToAdd = 6;

			EndDate = StartDate.AddMonths(monthsToAdd);
		}

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

		public void ClearData()
		{
			SelectedMember = null;
			SelectedMembershipType = "1 mesiac";
			SelectedIsActive = "Áno";
			StartDate = DateTime.Now;
			UpdateEndDateAutomatically();
		}

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

		private void Cancel()
		{
			_navigationService.CloseWindow("AddMembership");
		}
	}
}
