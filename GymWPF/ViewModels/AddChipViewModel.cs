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
	/// <summary>
	/// ViewModel for the "Add Chip" window. 
	/// Handles chip creation, data loading, and navigation commands.
	/// </summary>
	public class AddChipViewModel : ObservableObject
	{
		// Services for handling members, chips, and navigation.
		private readonly IMemberService _memberService;
		private readonly IChipService _chipService;
		private readonly INavigationService _navigationService;

		// Commands for the Cancel and Add Chip actions.
		public ICommand CancelCommand { get; }
		public ICommand AddChipCommand { get; }

		public ObservableCollection<string> MemberNames { get; } = new ObservableCollection<string>();
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

		private string _selectedIsActive = "Áno";
		/// <summary>
		/// Gets or sets the selected "IsActive" option.
		/// Defaults to "Áno".
		/// </summary>
		public string SelectedIsActive
		{
			get => _selectedIsActive;
			set => SetProperty(ref _selectedIsActive, value);
		}

		private string _chipInfo;
		/// <summary>
		/// Gets or sets the chip information entered by the user.
		/// </summary>
		public string ChipInfo
		{
			get => _chipInfo;
			set => SetProperty(ref _chipInfo, value);
		}

		/// <summary>
		/// Initializes a new instance of the AddChipViewModel class.
		/// Sets up commands and loads initial member data.
		/// </summary>
		/// <param name="memberService">Service for handling member data.</param>
		/// <param name="chipService">Service for handling chip data.</param>
		/// <param name="navigationService">Service for handling navigation between windows.</param>
		public AddChipViewModel(IMemberService memberService, IChipService chipService, INavigationService navigationService)
		{
			_memberService = memberService;
			_chipService = chipService;
			_navigationService = navigationService;

			CancelCommand = new RelayCommand(Cancel);
			AddChipCommand = new AsyncRelayCommand(AddChipAsync);

			LoadMembersAsync();
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
		/// Adds a new chip based on the selected member, chip information, and "IsActive" status.
		/// Validates user input and displays appropriate messages.
		/// </summary>
		private async Task AddChipAsync()
		{
			if (string.IsNullOrWhiteSpace(SelectedMember) || string.IsNullOrWhiteSpace(ChipInfo))
			{
				MessageBox.Show("Prosím vyberte člena a zadajte informácie o čipe.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var memberId = await _memberService.GetMemberIdByNameAsync(SelectedMember);
			if (!memberId.HasValue)
			{
				MessageBox.Show("Zvolený člen nebol nájdený.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			Chip newChip = new Chip
			{
				MemberID = memberId.Value,
				ChipInfo = ChipInfo,
				IsActive = SelectedIsActive == "Áno"
			};

			bool success = await _chipService.AddChipAsync(newChip);

			if (success)
			{
				MessageBox.Show("Čip bol úspešne pridaný!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
				ClearData();
			}
			else
			{
				MessageBox.Show("Nepodarilo sa pridať čip.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
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
		/// Cancels the "Add Chip" operation and closes the window.
		/// </summary>
		private void Cancel()
		{
			_navigationService.CloseWindow("AddChip");
		}

		/// <summary>
		/// Clears the form data, resetting all fields to their default values.
		/// </summary>
		private void ClearData()
		{
			SelectedMember = null;
			ChipInfo = string.Empty;
			SelectedIsActive = "Áno";
		}
	}
}
