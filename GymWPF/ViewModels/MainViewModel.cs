using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services;
using GymWPF.Services.Interfaces;
using GymWPF.Views.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GymWPF.ViewModels
{
	public partial class MainViewModel : BaseViewModel
	{
		private readonly IMembershipService _membershipService;
		private readonly IChipService _chipService;
		private readonly IMemberService _memberService;

		private readonly AddMemberViewModel _addMemberViewModel;
		private readonly AddMembershipViewModel _addMembershipViewModel;
		private readonly AddChipViewModel _addChipViewModel;
		private readonly ChangeChipViewModel _changeChipViewModel;
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

		public ICommand SelectSearchResultCommand { get; }
		public ICommand SearchTextChangedCommand { get; }

		public ICommand DeleteMembershipCommand { get; }
		public ICommand DeleteChipCommand { get; }

		public ICommand RefreshDataCommand { get; }

		private string _selectedMember;
		public string SelectedMember
		{
			get => _selectedMember;
			set => SetProperty(ref _selectedMember, value);
		}


		private bool _isActiveMemberships;
		public bool IsActiveMemberships
		{
			get => _isActiveMemberships;
			set
			{
				SetProperty(ref _isActiveMemberships, value);
				if (value)
				{
					_ = LoadMembershipsAsync(true);
				}
			}
		}


		private bool _isInactiveMemberships;
		public bool IsInactiveMemberships
		{
			get => _isInactiveMemberships;
			set
			{
				SetProperty(ref _isInactiveMemberships, value);
				if (value)
				{
					_ = LoadMembershipsAsync(false);
				}
			}
		}

		private bool _isActiveChips;
		public bool IsActiveChips
		{
			get => _isActiveChips;
			set
			{
				SetProperty(ref _isActiveChips, value);
				if (value)
				{
					_ = LoadChipsAsync(true);
				}
			}
		}


		private bool _isInactiveChips;
		public bool IsInactiveChips
		{
			get => _isInactiveChips;
			set
			{
				SetProperty(ref _isInactiveChips, value);
				if (value)
				{
					_ = LoadChipsAsync(false);
				}
			}
		}


		public string Note1
		{
			get => _note1;
			set => SetProperty(ref _note1, value);
		}
		private string _note1;


		public string Note2
		{
			get => _note2;
			set => SetProperty(ref _note2, value);
		}
		private string _note2;


		public MainViewModel(IMembershipService membershipService,
					   IChipService chipService,
					   IMemberService memberService,
					   AddMemberViewModel addMemberViewModel,
					   AddMembershipViewModel addMembershipViewModel,
					   AddChipViewModel addChipViewModel,
					   ChangeChipViewModel changeChipViewModel,
					   UserInfoViewModel userInfoViewModel)
		{
			_membershipService = membershipService;
			_chipService = chipService;
			_memberService = memberService;

			_addMemberViewModel = addMemberViewModel;
			_addMembershipViewModel = addMembershipViewModel;
			_addChipViewModel = addChipViewModel;
			_changeChipViewModel = changeChipViewModel;
			_userInfoViewModel = userInfoViewModel;

			Memberships = new ObservableCollection<MembershipDTO>();
			Chips = new ObservableCollection<ChipDTO>();

			IsActiveMemberships = true;
			IsActiveChips = true;

			LoadDataCommand = new RelayCommand(LoadData);
			LoadMembershipsCommand = new AsyncRelayCommand<string>(param => LoadMembershipsAsync(param == "Active"));
			LoadChipsCommand = new AsyncRelayCommand<string>(param => LoadChipsAsync(param == "Active"));
			FindUserInfoDialogCommand = new RelayCommand(FindUserInfoDialog);
			DeleteMembershipCommand = new AsyncRelayCommand<object>(DeleteMembershipRow);
			DeleteChipCommand = new AsyncRelayCommand<object>(DeleteChipRow);
			ShowAddMemberViewCommand = new RelayCommand(OpenAddMemberDialog);
			ShowAddMembershipViewCommand = new RelayCommand(OpenAddMembershipDialog);
			ShowAddChipViewCommand = new RelayCommand(OpenAddChipDialog);
			ShowChangeChipViewCommand = new RelayCommand(OpenChangeChipDialog);
			ShowUserInfoViewCommand = new RelayCommand(OpenUserInfoDialog);
			LogoutCommand = new RelayCommand(LogOutCommand);
			RefreshDataCommand = new RelayCommand(RefreshData);

			_ = LoadMembershipsAsync(IsActiveChips);
			_ = LoadChipsAsync(IsActiveChips);
			LoadData();
			LoadNotes();
		}

		private async void RefreshData()
		{
			await UpdateMembershipStatus();

			Memberships.Clear();
			Chips.Clear();
			MemberNames.Clear();

			LoadData();
			await LoadMembershipsAsync(IsActiveChips);
			await LoadChipsAsync(IsActiveChips);

			_addMemberViewModel.ClearData();
			_addMembershipViewModel.ClearData();
			_addChipViewModel.ClearData();
			_changeChipViewModel.ClearData();
			_userInfoViewModel.ClearData();

			_addMemberViewModel.RefreshData();
			await _addMembershipViewModel.RefreshData();
			await _addChipViewModel.RefreshData();
			await _changeChipViewModel.RefreshData();
		}


		private async Task LoadMembershipsAsync(bool isActive)
		{
			try
			{
				if (isActive)
				{
					var activeMemberships = await _membershipService.GetActiveMembershipsAsync();

					if (Memberships != null)
					{
						Memberships.Clear();
					}

					foreach (var membership in activeMemberships)
					{
						Memberships?.Add(membership);
					}
				}
				else
				{
					var inactiveMemberships = await _membershipService.GetInactiveMembershipsAsync();

					if (Memberships != null)
					{
						Memberships.Clear();
					}

					foreach (var membership in inactiveMemberships)
					{
						Memberships?.Add(membership);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading memberships: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private async Task LoadChipsAsync(bool isActive)
		{
			try
			{
				if (isActive)
				{
					var activeChips = await _chipService.GetActiveChipsAsync();

					if (Chips != null)
					{
						Chips.Clear();
					}

					foreach (var chip in activeChips)
					{
						Chips?.Add(chip);
					}
				}
				else
				{
					var inactiveChips = await _chipService.GetInactiveChipsAsync();

					if (Chips != null)
					{
						Chips.Clear();
					}

					foreach (var chip in inactiveChips)
					{
						Chips?.Add(chip);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading chips: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private async void LoadData()
		{
			try
			{
				var members = await _memberService.GetAllMembersAsync();
				foreach (var member in members)
				{
					MemberNames.Add($"{member.FirstName} {member.LastName}");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private void OpenDialog(UserControl dialogContent, double width, double height)
		{
			try
			{
				object viewModel = dialogContent.GetType() switch
				{
					Type t when t == typeof(AddMemberView) => _addMemberViewModel,
					Type t when t == typeof(AddMembershipView) => _addMembershipViewModel,
					Type t when t == typeof(AddChipView) => _addChipViewModel,
					Type t when t == typeof(ChangeChipView) => _changeChipViewModel,
					Type t when t == typeof(UserInfoView) => _userInfoViewModel,
					_ => throw new ArgumentException("Unknown dialog content type"),
				};

				dialogContent.DataContext = viewModel;

				var dialog = new Window
				{
					Content = dialogContent,
					SizeToContent = SizeToContent.Manual,
					Width = width,
					Height = height,
					WindowStartupLocation = WindowStartupLocation.CenterScreen,
					Owner = Application.Current.MainWindow
				};

				dialog.ShowDialog();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error opening dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private async Task DeleteMembershipRow(object membershipID)
		{
			try
			{
				if (membershipID != null && membershipID is int id)
				{
					bool isDeleted = await _membershipService.DeleteMembershipAsync(id);
					if (isDeleted)
					{
						await LoadMembershipsAsync(IsActiveMemberships); 
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error deleting membership: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private async Task DeleteChipRow(object chipID)
		{
			try
			{
				if (chipID != null && chipID is int id)
				{
					bool isDeleted = await _chipService.DeleteChipAsync(id);
					if (isDeleted)
					{
						await LoadChipsAsync(IsActiveChips); 
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error deleting chip: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private async void OpenUserInfoDialog()
		{
			try
			{
				await _userInfoViewModel.LoadUserInfo(1); 
				OpenDialog(new UserInfoView(), 790, 780);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error opening user info dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private void OpenAddMemberDialog()
		{
			OpenDialog(new AddMemberView(), 757, 550);
		}


		private void OpenAddMembershipDialog()
		{
			OpenDialog(new AddMembershipView(), 690, 620);
		}


		private void OpenAddChipDialog()
		{
			OpenDialog(new AddChipView(), 763, 522);
		}


		private void OpenChangeChipDialog()
		{
			OpenDialog(new ChangeChipView(), 763, 520);
		}


		private void LogOutCommand()
		{
			Application.Current.Shutdown();
			SaveNotes();
		}


		private void SaveNotes()
		{
			Properties.Settings.Default.Note1 = Note1;
			Properties.Settings.Default.Note2 = Note2;
			Properties.Settings.Default.Save();
		}


		private void LoadNotes()
		{
			try
			{
				Note1 = Properties.Settings.Default.Note1;
				Note2 = Properties.Settings.Default.Note2;

			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading notes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private async void FindUserInfoDialog()
		{
			if (string.IsNullOrWhiteSpace(SelectedMember))
			{
				MessageBox.Show("Please select a member.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var names = SelectedMember.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (names.Length < 2)
			{
				MessageBox.Show("Please select a full name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var firstName = names.First();
			var lastName = names.Last();

			try
			{
				var allMembers = await _memberService.GetAllMembersAsync();
				if (allMembers == null)
				{
					MessageBox.Show("Failed to retrieve members.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				var selectedMember = allMembers.FirstOrDefault(member =>
					string.Equals(member.FirstName, firstName, StringComparison.OrdinalIgnoreCase) &&
					string.Equals(member.LastName, lastName, StringComparison.OrdinalIgnoreCase));

				if (selectedMember != null)
				{
					_userInfoViewModel.ClearData();
					await _userInfoViewModel.LoadUserInfo(selectedMember.MemberID);
					Properties.Settings.Default.SelectedMemberId = selectedMember.MemberID;
					Properties.Settings.Default.Save();
					OpenDialog(new UserInfoView(), 790, 780);
					
				}
				else
				{
					MessageBox.Show("Member not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error finding user info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private async Task UpdateMembershipStatus()
		{
			try
			{
				var activeMemberships = await _membershipService.GetActiveMembershipsAsync();

				var currentDate = DateTime.Now;

				foreach (var mem in activeMemberships)
				{
					if (mem.EndDate < currentDate)
					{
						int? memberId = await _memberService.GetMemberIdByNameAsync(mem.MemberName);

						if (memberId.HasValue)
						{
							var membershipToUpdate = (await _membershipService.GetAllMembershipsAsync())
								.FirstOrDefault(m => m.MemberID == memberId && m.PaymentType == mem.Type);

							if (membershipToUpdate != null)
							{
								membershipToUpdate.IsActive = false; 

								bool isUpdated = await _membershipService.UpdateMembershipAsync(membershipToUpdate.MembershipID, membershipToUpdate);
							}
						}
						
					}
				}


			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error updating membership status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

	}
}
