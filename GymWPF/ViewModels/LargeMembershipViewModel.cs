using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

public class LargeMembershipViewModel : ObservableObject
{
	private readonly IMembershipService _membershipService;
	private bool _isActive;

	public ObservableCollection<MembershipDTO> Memberships { get; } = new ObservableCollection<MembershipDTO>();
	public ICommand DeleteMembershipCommand { get; }

	public LargeMembershipViewModel(IMembershipService membershipService, bool isActive)
	{
		_membershipService = membershipService;
		_isActive = isActive;

		DeleteMembershipCommand = new AsyncRelayCommand<object>(DeleteMembershipAsync);

		LoadMembershipsAsync();
	}

	private async Task LoadMembershipsAsync()
	{
		try
		{
			Memberships.Clear();
			if (_isActive)
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
			System.Windows.MessageBox.Show($"Chyba pri načítaní členstiev: {ex.Message}", "Chyba", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
		}
	}

	private async Task DeleteMembershipAsync(object membershipID)
	{
		if (membershipID is int id)
		{
			var result = System.Windows.MessageBox.Show("Ste si istý, že chcete odstrániť toto členstvo?", "Potvrdenie", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
			if (result == System.Windows.MessageBoxResult.Yes)
			{
				bool isDeleted = await _membershipService.DeleteMembershipAsync(id);
				if (isDeleted)
				{
					await LoadMembershipsAsync();
				}
			}
		}
	}
}
