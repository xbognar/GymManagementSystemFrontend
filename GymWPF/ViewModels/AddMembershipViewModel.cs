using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
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
		public string? SelectedMember
		{
			get => _selectedMember;
			set => SetProperty(ref _selectedMember, value);
		}

		private string _selectedMembershipType = "1 mesiac";
		public string SelectedMembershipType
		{
			get => _selectedMembershipType;
			set => SetProperty(ref _selectedMembershipType, value);
		}

		private string _selectedIsActive;
		public string? SelectedIsActive
		{
			get => _selectedIsActive;
			set => SetProperty(ref _selectedIsActive, value);
		}

		private DateTime _startDate = DateTime.Now;
		public DateTime StartDate
		{
			get => _startDate;
			set => SetProperty(ref _startDate, value);
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

		private async Task CreateMembershipAsync()
		{
			if (SelectedMember == null)
			{
				MessageBox.Show("Please select a member.");
				return;
			}

			var memberId = await _memberService.GetMemberIdByNameAsync(SelectedMember);
			if (!memberId.HasValue)
			{
				MessageBox.Show("Selected member not found.");
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
				MessageBox.Show("Membership created successfully!");
				_navigationService.CloseWindow("AddMembership");
			}
			else
			{
				MessageBox.Show("Failed to create membership.");
			}
		}

		public void ClearData()
		{
			SelectedMember = null;
			SelectedMembershipType = "1 mesiac";
			SelectedIsActive = null;
			StartDate = DateTime.Now;
			EndDate = DateTime.Now;
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
