using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GymWPF.ViewModels
{
	
	/// <summary>
	/// The main ViewModel for the application, managing memberships, chips, and users.
	/// Handles commands for navigating views, loading data, and executing user actions.
	/// </summary>
	public partial class MainViewModel : BaseViewModel
	{
		private readonly IMembershipService _membershipService;
		private readonly IChipService _chipService;
		private readonly IMemberService _memberService;
		private readonly INavigationService _navigationService;
		private readonly UserInfoViewModel _userInfoViewModel;

		public ObservableCollection<MembershipDTO> Memberships { get; private set; }
		public ObservableCollection<ChipDTO> Chips { get; private set; }
		public ObservableCollection<string> MemberNames { get; } = new ObservableCollection<string>();

		public ICommand LoadDataCommand { get; }
		public ICommand LoadMembershipsCommand { get; }
		public ICommand LoadChipsCommand { get; }
		public ICommand ShowUserInfoViewCommand { get; }
		public ICommand ShowAddChipViewCommand { get; }
		public ICommand ShowAddMemberViewCommand { get; }
		public ICommand ShowAddMembershipViewCommand { get; }
		public ICommand ShowChangeChipViewCommand { get; }
		public ICommand LogoutCommand { get; }
		public ICommand FindUserInfoDialogCommand { get; }
		public ICommand DeleteMembershipCommand { get; }
		public ICommand DeleteChipCommand { get; }
		public ICommand RefreshDataCommand { get; }
		public ICommand ShowLargeMembershipViewCommand { get; }
		public ICommand ShowLargeChipViewCommand { get; }
		public ICommand ShowAllUsersViewCommand { get; }

		private string _selectedMember;
		public string SelectedMember
		{
			get => _selectedMember;
			set => SetProperty(ref _selectedMember, value);
		}

		private bool _isActiveMemberships;
		/// <summary>
		/// Indicates whether active memberships are currently selected. 
		/// No direct load is triggered here. Use LoadMembershipsCommand with "Active" parameter to load.
		/// </summary>
		public bool IsActiveMemberships
		{
			get => _isActiveMemberships;
			set => SetProperty(ref _isActiveMemberships, value);
		}

		private bool _isInactiveMemberships;
		/// <summary>
		/// Indicates whether inactive memberships are currently selected.
		/// No direct load is triggered here. Use LoadMembershipsCommand with "Inactive" parameter to load.
		/// </summary>
		public bool IsInactiveMemberships
		{
			get => _isInactiveMemberships;
			set => SetProperty(ref _isInactiveMemberships, value);
		}

		private bool _isActiveChips;
		/// <summary>
		/// Indicates whether active chips are currently selected.
		/// No direct load is triggered here. Use LoadChipsCommand with "Active" parameter to load.
		/// </summary>
		public bool IsActiveChips
		{
			get => _isActiveChips;
			set => SetProperty(ref _isActiveChips, value);
		}

		private bool _isInactiveChips;
		/// <summary>
		/// Indicates whether inactive chips are currently selected.
		/// No direct load is triggered here. Use LoadChipsCommand with "Inactive" parameter to load.
		/// </summary>
		public bool IsInactiveChips
		{
			get => _isInactiveChips;
			set => SetProperty(ref _isInactiveChips, value);
		}

		private string _note1;
		public string Note1
		{
			get => _note1;
			set
			{
				SetProperty(ref _note1, value);
				SaveNotes();
			}
		}

		private string _note2;
		public string Note2
		{
			get => _note2;
			set
			{
				SetProperty(ref _note2, value);
				SaveNotes();
			}
		}

		/// <summary>
		/// Initializes a new instance of MainViewModel, sets up commands and initial collections.
		/// Does not automatically load memberships or chips. Relies on explicit commands or RefreshData for loading.
		/// </summary>
		public MainViewModel(IMembershipService membershipService, IChipService chipService, IMemberService memberService, INavigationService navigationService, UserInfoViewModel userInfoViewModel)
		{
			_membershipService = membershipService;
			_chipService = chipService;
			_memberService = memberService;
			_navigationService = navigationService;
			_userInfoViewModel = userInfoViewModel;

			Memberships = new ObservableCollection<MembershipDTO>();
			Chips = new ObservableCollection<ChipDTO>();

			LoadDataCommand = new RelayCommand(LoadData);
			LoadMembershipsCommand = new AsyncRelayCommand<string>(param => LoadMembershipsAsync(param == "Active"));
			LoadChipsCommand = new AsyncRelayCommand<string>(param => LoadChipsAsync(param == "Active"));
			FindUserInfoDialogCommand = new RelayCommand(FindUserInfoDialog);
			DeleteMembershipCommand = new AsyncRelayCommand<object>(DeleteMembershipRow);
			DeleteChipCommand = new AsyncRelayCommand<object>(DeleteChipRow);
			ShowAddMemberViewCommand = new RelayCommand(() => _navigationService.NavigateTo("AddMember"));
			ShowAddMembershipViewCommand = new RelayCommand(() => _navigationService.NavigateTo("AddMembership"));
			ShowAddChipViewCommand = new RelayCommand(() => _navigationService.NavigateTo("AddChip"));
			ShowChangeChipViewCommand = new RelayCommand(() => _navigationService.NavigateTo("ChangeChip"));
			ShowUserInfoViewCommand = new RelayCommand(() => _navigationService.NavigateTo("UserInfo"));
			LogoutCommand = new RelayCommand(LogOutCommand);
			RefreshDataCommand = new RelayCommand(RefreshData);
			ShowLargeMembershipViewCommand = new RelayCommand(ShowLargeMembershipView);
			ShowLargeChipViewCommand = new RelayCommand(ShowLargeChipView);
			ShowAllUsersViewCommand = new RelayCommand(ShowAllUsersView);

			LoadNotes();
			LoadData();

			IsActiveMemberships = true;
			IsActiveChips = true;

		}


		/// <summary>
		/// Refreshes all data by updating membership and chip statuses, clearing collections, reloading member names, and reloading currently selected active/inactive data.
		/// </summary>
		private async void RefreshData()
		{
			await UpdateMembershipStatusAsync();
			await UpdateChipStatusAsync();

			Memberships.Clear();
			Chips.Clear();
			MemberNames.Clear();

			LoadData();

			if (IsActiveMemberships)
				await LoadMembershipsAsync(true);
			else if (IsInactiveMemberships)
				await LoadMembershipsAsync(false);

			if (IsActiveChips)
				await LoadChipsAsync(true);
			else if (IsInactiveChips)
				await LoadChipsAsync(false);
		}

		/// <summary>
		/// Loads memberships from the backend based on whether they are active or inactive, and populates the Memberships collection.
		/// </summary>
		/// <param name="isActive">If true, loads active memberships; otherwise, inactive memberships.</param>
		private async Task LoadMembershipsAsync(bool isActive)
		{
			try
			{
				Memberships.Clear();
				if (isActive)
				{
					var activeMemberships = await _membershipService.GetActiveMembershipsAsync();
					if (activeMemberships != null)
					{
						foreach (var membership in activeMemberships)
							Memberships.Add(membership);
					}
				}
				else
				{
					var inactiveMemberships = await _membershipService.GetInactiveMembershipsAsync();
					if (inactiveMemberships != null)
					{
						foreach (var membership in inactiveMemberships)
							Memberships.Add(membership);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri načítaní členstiev: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Loads chips from the backend based on whether they are active or inactive, and populates the Chips collection.
		/// </summary>
		/// <param name="isActive">If true, loads active chips; otherwise, inactive chips.</param>
		private async Task LoadChipsAsync(bool isActive)
		{
			try
			{
				Chips.Clear();
				if (isActive)
				{
					var activeChips = await _chipService.GetActiveChipsAsync();
					if (activeChips != null)
					{
						foreach (var chip in activeChips) Chips.Add(chip);
					}
				}
				else
				{
					var inactiveChips = await _chipService.GetInactiveChipsAsync();
					if (inactiveChips != null)
					{
						foreach (var chip in inactiveChips) Chips.Add(chip);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri načítaní čipov: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Loads the list of member names into MemberNames collection.
		/// Does not load memberships or chips.
		/// </summary>
		private async void LoadData()
		{
			try
			{
				var members = await _memberService.GetAllMembersAsync();
				if (members != null)
				{
					foreach (var member in members) MemberNames.Add($"{member.FirstName} {member.LastName}");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri načítaní dát: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Deletes a membership after user confirmation and refreshes the memberships list.
		/// </summary>
		/// <param name="membershipID">The ID of the membership to delete.</param>
		private async Task DeleteMembershipRow(object membershipID)
		{
			try
			{
				if (membershipID is int id)
				{
					var result = MessageBox.Show("Ste si istý, že chcete odstrániť toto členstvo?", "Potvrdenie", MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes)
					{
						bool isDeleted = await _membershipService.DeleteMembershipAsync(id);
						if (isDeleted)
						{
							if (IsActiveMemberships)
								await LoadMembershipsAsync(true);
							else if (IsInactiveMemberships)
								await LoadMembershipsAsync(false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri mazaní členstva: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Deletes a chip after user confirmation and refreshes the chips list.
		/// </summary>
		/// <param name="chipID">The ID of the chip to delete.</param>
		private async Task DeleteChipRow(object chipID)
		{
			try
			{
				if (chipID is int id)
				{
					var result = MessageBox.Show("Ste si istý, že chcete odstrániť tento čip?", "Potvrdenie", MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes)
					{
						bool isDeleted = await _chipService.DeleteChipAsync(id);
						if (isDeleted)
						{
							if (IsActiveChips)
								await LoadChipsAsync(true);
							else if (IsInactiveChips)
								await LoadChipsAsync(false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri mazaní čipu: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Finds user info based on the currently selected member name. Opens the user info window if the member is found.
		/// </summary>
		private async void FindUserInfoDialog()
		{
			if (string.IsNullOrWhiteSpace(SelectedMember))
			{
				MessageBox.Show("Prosím vyberte člena.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var names = SelectedMember.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (names.Length < 2)
			{
				MessageBox.Show("Prosím vyberte celé meno.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var firstName = names.First();
			var lastName = names.Last();

			try
			{
				var allMembers = await _memberService.GetAllMembersAsync();
				var selectedMember = allMembers?.FirstOrDefault(member =>
					string.Equals(member.FirstName, firstName, StringComparison.OrdinalIgnoreCase) &&
					string.Equals(member.LastName, lastName, StringComparison.OrdinalIgnoreCase));

				if (selectedMember != null)
				{
					_userInfoViewModel.ClearData();
					await _userInfoViewModel.LoadUserInfoAsync(selectedMember.MemberID);
					Properties.Settings.Default.SelectedMemberId = selectedMember.MemberID;
					Properties.Settings.Default.Save();

					_navigationService.NavigateTo("UserInfo", _userInfoViewModel);
				}
				else
				{
					MessageBox.Show("Člen nebol nájdený.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri hľadaní informácií o používateľovi: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Logs out the user by shutting down the application.
		/// </summary>
		private void LogOutCommand()
		{
			Application.Current.Shutdown();
		}

		/// <summary>
		/// Updates membership statuses, marking memberships as inactive if their EndDate has passed.
		/// </summary>
		private async Task UpdateMembershipStatusAsync()
		{
			try
			{
				var allMemberships = await _membershipService.GetAllMembershipsAsync();
				if (allMemberships != null)
				{
					foreach (var mem in allMemberships)
					{
						if (mem.IsActive && mem.EndDate < DateTime.Now)
						{
							mem.IsActive = false;
							await _membershipService.UpdateMembershipAsync(mem.MembershipID, mem);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri aktualizácii stavu členstiev: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Updates chip statuses, marking chips as inactive if their owners have no active memberships.
		/// </summary>
		private async Task UpdateChipStatusAsync()
		{
			try
			{
				var allChips = await _chipService.GetAllChipsAsync();
				if (allChips != null)
				{
					foreach (var chip in allChips)
					{
						if (chip.IsActive)
						{
							var userMemberships = await _membershipService.GetUserMembershipsAsync(chip.MemberID);
							bool hasActiveMembership = userMemberships != null && userMemberships.Any(m => m.EndDate > DateTime.Now);

							if (!hasActiveMembership)
							{
								var updateRequest = new ChipUpdateRequest
								{
									ChipID = chip.ChipID,
									NewMemberID = chip.MemberID,
									IsActive = false
								};

								await _chipService.UpdateChipAsync(chip.ChipID, updateRequest);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri aktualizácii stavu čipov: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Navigates to the Large Membership view.
		/// </summary>
		private void ShowLargeMembershipView()
		{
			bool isActive = IsActiveMemberships || (!IsActiveMemberships && !IsInactiveMemberships);
			var vm = new LargeMembershipViewModel(_membershipService, isActive);
			_navigationService.NavigateTo("LargeMembership", vm);
		}

		/// <summary>
		/// Navigates to the Large Chip view.
		/// </summary>
		private void ShowLargeChipView()
		{
			bool isActive = IsActiveChips || (!IsActiveChips && !IsInactiveChips);
			var vm = new LargeChipViewModel(_chipService, isActive);
			_navigationService.NavigateTo("LargeChip", vm);
		}

		/// <summary>
		/// Navigates to the All Users view.
		/// </summary>
		private void ShowAllUsersView()
		{
			// Create the ViewModel manually
			var vm = new AllUsersViewModel(_memberService);
			// Navigate to the "AllUsers" window and pass the ViewModel
			_navigationService.NavigateTo("AllUsers", vm);
		}

		/// <summary>
		/// Saves the current notes (Note1 and Note2) to application settings.
		/// </summary>
		private void SaveNotes()
		{
			Properties.Settings.Default.Note1 = Note1;
			Properties.Settings.Default.Note2 = Note2;
			Properties.Settings.Default.Save();
		}

		/// <summary>
		/// Loads notes (Note1 and Note2) from application settings.
		/// </summary>
		private void LoadNotes()
		{
			try
			{
				var note1 = Properties.Settings.Default.Note1;
				var note2 = Properties.Settings.Default.Note2;

				Note1 = note1;
				Note2 = note2;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri načítaní poznámok: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
