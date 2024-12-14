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
	public class AddChipViewModel : ObservableObject
	{
		private readonly IMemberService _memberService;
		private readonly IChipService _chipService;
		private readonly INavigationService _navigationService;

		public ICommand CancelCommand { get; }
		public ICommand AddChipCommand { get; }

		public ObservableCollection<string> MemberNames { get; } = new ObservableCollection<string>();
		public System.Collections.Generic.List<string> IsActiveOptions { get; } = new System.Collections.Generic.List<string> { "Áno", "Nie" };

		private string _selectedMember;
		public string SelectedMember
		{
			get => _selectedMember;
			set => SetProperty(ref _selectedMember, value);
		}

		private string _selectedIsActive = "Áno";
		public string SelectedIsActive
		{
			get => _selectedIsActive;
			set => SetProperty(ref _selectedIsActive, value);
		}

		private string _chipInfo;
		public string ChipInfo
		{
			get => _chipInfo;
			set => SetProperty(ref _chipInfo, value);
		}

		public AddChipViewModel(IMemberService memberService, IChipService chipService, INavigationService navigationService)
		{
			_memberService = memberService;
			_chipService = chipService;
			_navigationService = navigationService;

			CancelCommand = new RelayCommand(Cancel);
			AddChipCommand = new AsyncRelayCommand(AddChipAsync);

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
			_navigationService.CloseWindow("AddChip");
		}

		private void ClearData()
		{
			SelectedMember = null;
			ChipInfo = string.Empty;
			SelectedIsActive = "Áno";
		}
	}
}
