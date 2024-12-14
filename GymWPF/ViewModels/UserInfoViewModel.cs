using CommunityToolkit.Mvvm.ComponentModel;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Linq;

namespace GymWPF.ViewModels
{
	/// <summary>
	/// ViewModel for managing and displaying detailed user information.
	/// Includes personal details, chip information, and membership data.
	/// </summary>
	public class UserInfoViewModel : ObservableObject
	{
		private readonly IMemberService _memberService;
		private readonly IMembershipService _membershipService;
		private readonly IChipService _chipService;
		private readonly INavigationService _navigationService;

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
		/// <summary>
		/// The number of memberships the user has.
		/// </summary>
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

		/// <summary>
		/// Initializes a new instance of UserInfoViewModel.
		/// Attempts to load saved member info or the first available member.
		/// </summary>
		public UserInfoViewModel(IMemberService memberService, IMembershipService membershipService, IChipService chipService, INavigationService navigationService)
		{
			_memberService = memberService;
			_membershipService = membershipService;
			_chipService = chipService;
			_navigationService = navigationService;

			InitializeUserInfo();
		}

		/// <summary>
		/// Initializes user info by checking if there's a saved member ID.
		/// If not, it loads the first available member.
		/// </summary>
		private async void InitializeUserInfo()
		{
			try
			{
				int savedMemberId = Properties.Settings.Default.SelectedMemberId;
				if (savedMemberId > 0)
				{
					await LoadUserInfoAsync(savedMemberId);
				}
				else
				{
					var members = await _memberService.GetAllMembersAsync();
					if (members != null && members.Any())
					{
						var firstMember = members.First();
						ClearData();
						await LoadUserInfoAsync(firstMember.MemberID);

						Properties.Settings.Default.SelectedMemberId = firstMember.MemberID;
						Properties.Settings.Default.Save();
					}
					else
					{
						MessageBox.Show("Neboli nájdení žiadni používatelia.", "Informácia", MessageBoxButton.OK, MessageBoxImage.Information);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri načítaní informácií o používateľovi: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Loads detailed information about the specified user.
		/// Includes personal details, chip info, and memberships.
		/// </summary>
		/// <param name="memberId">The ID of the member to load.</param>
		public async Task LoadUserInfoAsync(int memberId)
		{
			try
			{
				var member = await _memberService.GetMemberByIdAsync(memberId);
				if (member != null)
				{
					FirstName = member.FirstName;
					LastName = member.LastName;
					Email = member.Email;
					PhoneNumber = member.PhoneNumber;

					ChipNumber = await _chipService.GetChipInfoByMemberIdAsync(memberId);

					await LoadUserMembershipsAsync(memberId);

					MembershipCount = UserMemberships.Count;
				}
				else
				{
					MessageBox.Show("Zvolený používateľ nebol nájdený.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri načítaní informácií o používateľovi: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Clears all user information fields and membership data.
		/// </summary>
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

		/// <summary>
		/// Refreshes the currently displayed user's data.
		/// </summary>
		/// <param name="memberId">The ID of the member to refresh data for.</param>
		public async Task RefreshDataAsync(int memberId)
		{
			await LoadUserInfoAsync(memberId);
		}

		/// <summary>
		/// Loads the memberships of a user and populates the UserMemberships collection.
		/// </summary>
		/// <param name="memberId">The ID of the member whose memberships to load.</param>
		private async Task LoadUserMembershipsAsync(int memberId)
		{
			try
			{
				var userMemberships = await _membershipService.GetUserMembershipsAsync(memberId);
				if (userMemberships != null)
				{
					UserMemberships.Clear();
					foreach (var membership in userMemberships)
					{
						UserMemberships.Add(membership);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri načítaní členstiev: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
