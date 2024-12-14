using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GymWPF.ViewModels
{
	public class ChangeChipViewModel : ObservableObject
	{
		private readonly IMemberService _memberService;
		private readonly IChipService _chipService;
		private readonly INavigationService _navigationService;

		public ObservableCollection<string> MemberNames { get; } = new ObservableCollection<string>();

		public ICommand CancelCommand { get; }
		public ICommand ChangeChipCommand { get; }

		private string _selectedOldOwner;
		/// <summary>
		/// The currently selected old owner of the chip.
		/// </summary>
		public string? SelectedOldOwner
		{
			get => _selectedOldOwner;
			set => SetProperty(ref _selectedOldOwner, value);
		}

		private string _selectedNewOwner;
		/// <summary>
		/// The currently selected new owner of the chip.
		/// </summary>
		public string? SelectedNewOwner
		{
			get => _selectedNewOwner;
			set => SetProperty(ref _selectedNewOwner, value);
		}

		/// <summary>
		/// Initializes a new instance of the ChangeChipViewModel, loads all members, and sets up commands.
		/// </summary>
		public ChangeChipViewModel(IMemberService memberService, IChipService chipService, INavigationService navigationService)
		{
			_memberService = memberService;
			_chipService = chipService;
			_navigationService = navigationService;

			CancelCommand = new RelayCommand(Cancel);
			ChangeChipCommand = new AsyncRelayCommand(UpdateChipOwnerAsync);

			LoadAllMembersAsync();
		}

		/// <summary>
		/// Asynchronously loads all members and populates the MemberNames collection.
		/// </summary>
		private async void LoadAllMembersAsync()
		{
			var members = await _memberService.GetAllMembersAsync();
			if (members != null)
			{
				foreach (var member in members)
				{
					var fullName = $"{member.FirstName} {member.LastName}";
					MemberNames?.Add(fullName);
				}
			}
		}

		/// <summary>
		/// Asynchronously updates the chip owner from the old owner to the new owner.
		/// Validates inputs, finds the chip, and updates it using the ChipService.
		/// Shows message boxes in Slovak and clears fields after a successful update.
		/// </summary>
		private async Task UpdateChipOwnerAsync()
		{
			if (string.IsNullOrEmpty(SelectedOldOwner) || string.IsNullOrEmpty(SelectedNewOwner))
			{
				MessageBox.Show("Prosím vyberte starého a nového majiteľa čipu pred pokračovaním.", "Neplatný výber", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var oldMemberId = await _memberService.GetMemberIdByNameAsync(SelectedOldOwner);
			if (!oldMemberId.HasValue)
			{
				MessageBox.Show("Starý majiteľ nebol nájdený. Uistite sa, že ste vybrali platného člena.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var chips = await _chipService.GetAllChipsAsync();
			var chipToUpdate = chips?.FirstOrDefault(chip => chip.MemberID == oldMemberId.Value);
			if (chipToUpdate == null)
			{
				MessageBox.Show("Pre vybraného starého majiteľa neboli nájdené žiadne čipy. Uistite sa, že starý majiteľ má priradené čipy.", "Žiadne čipy neboli nájdené", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var newMemberId = await _memberService.GetMemberIdByNameAsync(SelectedNewOwner);
			if (!newMemberId.HasValue)
			{
				MessageBox.Show("Nový majiteľ nebol nájdený. Uistite sa, že ste vybrali platného člena.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var updateRequest = new ChipUpdateRequest
			{
				ChipID = chipToUpdate.ChipID,
				NewMemberID = newMemberId.Value
			};

			var success = await _chipService.UpdateChipAsync(updateRequest.ChipID, updateRequest);
			if (!success)
			{
				MessageBox.Show("Aktualizácia majiteľa čipu zlyhala. Skúste to znova neskôr.", "Chyba pri aktualizácii", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			MessageBox.Show("Majiteľ čipu bol úspešne aktualizovaný.", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);

			ClearData();
		}

		/// <summary>
		/// Clears the SelectedOldOwner and SelectedNewOwner fields.
		/// </summary>
		public void ClearData()
		{
			SelectedOldOwner = null;
			SelectedNewOwner = null;
		}

		/// <summary>
		/// Asynchronously refreshes member data and repopulates the MemberNames collection.
		/// </summary>
		public async Task RefreshData()
		{
			MemberNames.Clear();
			var members = await _memberService.GetAllMembersAsync();
			if (members != null)
			{
				foreach (var member in members)
				{
					var fullName = $"{member.FirstName} {member.LastName}";
					MemberNames?.Add(fullName);
				}
			}
		}

		/// <summary>
		/// Cancels the chip owner change and closes the window.
		/// </summary>
		private void Cancel()
		{
			_navigationService.CloseWindow("ChangeChip");
		}
	}
}
