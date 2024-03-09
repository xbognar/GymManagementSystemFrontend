using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GymWPF.ViewModels
{
	public class ChangeChipViewModel : ObservableObject
	{
		private readonly IMemberService _memberService;
		private readonly IChipService _chipService;

		public ObservableCollection<string> MemberNames { get; } = new ObservableCollection<string>();

		private string _selectedOldOwner;
		public string? SelectedOldOwner
		{
			get => _selectedOldOwner;
			set => SetProperty(ref _selectedOldOwner, value);
		}

		private string _selectedNewOwner;
		public string? SelectedNewOwner
		{
			get => _selectedNewOwner;
			set => SetProperty(ref _selectedNewOwner, value);
		}

		public ICommand CancelCommand { get; }
		public ICommand ChangeChipCommand { get; }

		public ChangeChipViewModel(IMemberService memberService, IChipService chipService)
		{
			_memberService = memberService;
			_chipService = chipService;

			CancelCommand = new RelayCommand(Cancel);
			ChangeChipCommand = new AsyncRelayCommand(UpdateChipOwnerAsync);

			LoadAllMembersAsync();
		}

		private async void LoadAllMembersAsync()
		{
			var members = await _memberService.GetAllMembersAsync();
			foreach (var member in members)
			{
				var fullName = $"{member.FirstName} {member.LastName}";
				MemberNames?.Add(fullName);
			}
		}

		private async Task UpdateChipOwnerAsync()
		{

			if (string.IsNullOrEmpty(SelectedOldOwner) || string.IsNullOrEmpty(SelectedNewOwner))
			{
				MessageBox.Show("Prosím vyberte starého a nového majiteľa čipu pred pokračovaním.", "Neplatný výber", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var oldMemberId = await _memberService.GetMemberIdByNameAsync(SelectedOldOwner);

			var chips = await _chipService.GetAllChipsAsync();
			var chipToUpdate = chips.FirstOrDefault(chip => chip.MemberID == oldMemberId.Value);
			if (chipToUpdate == null)
			{
				MessageBox.Show("Pre vybraného starého majiteľa neboli nájdené žiadne čipy. Uistite sa, že starý majiteľ má priradené čipy.", "Žiadne čipy neboli nájdené", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var newMemberId = await _memberService.GetMemberIdByNameAsync(SelectedNewOwner);

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
		}

		public void ClearData()
		{
			SelectedOldOwner = null;
			SelectedNewOwner = null;
		}

		public async Task RefreshData()
		{
			// Clear and reload member names
			MemberNames.Clear();
			var members = await _memberService.GetAllMembersAsync();
			foreach (var member in members)
			{
				var fullName = $"{member.FirstName} {member.LastName}";
				MemberNames?.Add(fullName);
			}
		}

		private void Cancel()
		{

			Application.Current.Windows[2].Close();

		}
	}
}
