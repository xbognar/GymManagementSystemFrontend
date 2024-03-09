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
	public class UserInfoViewModel : ObservableObject
	{
		private readonly IMemberService _memberService;
		private readonly IMembershipService _membershipService;
		private readonly IChipService _chipService;

		private string _firstName;
		public string? FirstName
		{
			get => _firstName;
			set => SetProperty(ref _firstName, value);
		}

		private string _lastName;
		public string? LastName
		{
			get => _lastName;
			set => SetProperty(ref _lastName, value);
		}

		private string _email;
		public string? Email
		{
			get => _email;
			set => SetProperty(ref _email, value);
		}

		private string _phoneNumber;
		public string? PhoneNumber
		{
			get => _phoneNumber;
			set => SetProperty(ref _phoneNumber, value);
		}

		private int _membershipCount;
		public int MembershipCount
		{
			get => _membershipCount;
			set => SetProperty(ref _membershipCount, value);
		}

		private string _chipNumber;
		public string? ChipNumber
		{
			get => _chipNumber;
			set => SetProperty(ref _chipNumber, value);
		}

		public ObservableCollection<UserMembershipsDTO> UserMemberships { get; } = new ObservableCollection<UserMembershipsDTO>();

		public ICommand DeleteUser { get; }


		public UserInfoViewModel(IMemberService memberService, IMembershipService membershipService, IChipService chipService)
		{
			_memberService = memberService;
			_membershipService = membershipService;
			_chipService = chipService;

			DeleteUser = new AsyncRelayCommand(OnDeleteUser);

		}

		private async Task OnDeleteUser()
		{
			
			var memberID = Properties.Settings.Default.SelectedMemberId;
			await _memberService.DeleteMemberAsync(memberID);

		}

		public async Task LoadUserInfo(int memberId)
		{
			var member = await _memberService.GetMemberByIdAsync(memberId);
			if (member != null)
			{
				FirstName = member.FirstName;
				LastName = member.LastName;
				Email = member.Email;
				PhoneNumber = member.PhoneNumber;
				ChipNumber = await _chipService.GetChipInfoByMemberIdAsync(memberId);

				await LoadUserMemberships(memberId);

				MembershipCount = UserMemberships.Count;
			}

		}

		public void ClearData()
		{
			FirstName = string.Empty;
			LastName = string.Empty;
			Email = string.Empty;
			PhoneNumber = string.Empty;
			MembershipCount = 0;
			ChipNumber = string.Empty;
			UserMemberships.Clear();
		}

		public async Task RefreshData(int memberId)
		{
			await LoadUserInfo(memberId);
		}



		private async Task LoadUserMemberships(int memberId)
		{
			var userMemberships = await _membershipService.GetUserMembershipsAsync(memberId);
			if (userMemberships != null)
			{
				foreach (var membership in userMemberships)
				{
					UserMemberships.Add(membership);
				}
			}
		}
	}
}
