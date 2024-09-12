using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

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
		public List<string> IsActiveOptions { get; } = new List<string> { "Áno", "Nie" };

		private string _selectedMember;
		public string? SelectedMember
		{
			get => _selectedMember;
			set => SetProperty(ref _selectedMember, value);
		}

		private string _selectedIsActive;
		public string? SelectedIsActive
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
			if (SelectedMember == null || string.IsNullOrWhiteSpace(ChipInfo))
			{
				MessageBox.Show("Please select a member and enter chip information.");
				return;
			}

			var memberId = await _memberService.GetMemberIdByNameAsync(SelectedMember);
			if (!memberId.HasValue)
			{
				MessageBox.Show("Selected member not found.");
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
				MessageBox.Show("Chip added successfully!");
				_navigationService.CloseWindow("AddChip");
			}
			else
			{
				MessageBox.Show("Failed to add chip.");
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
	}
}
