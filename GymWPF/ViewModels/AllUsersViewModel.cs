using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

public class AllUsersViewModel : ObservableObject
{
	private readonly IMemberService _memberService;

	public ObservableCollection<Member> Users { get; } = new ObservableCollection<Member>();
	public ICommand DeleteUserCommand { get; }

	public AllUsersViewModel(IMemberService memberService)
	{
		_memberService = memberService;
		DeleteUserCommand = new AsyncRelayCommand<object>(DeleteUserAsync);

		LoadUsersAsync();
	}

	private async Task LoadUsersAsync()
	{
		try
		{
			Users.Clear();
			var allMembers = await _memberService.GetAllMembersAsync();
			if (allMembers != null)
			{
				foreach (var member in allMembers)
					Users.Add(member);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Chyba pri načítaní používateľov: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	private async Task DeleteUserAsync(object memberID)
	{
		if (memberID is int id)
		{
			var result = MessageBox.Show("Ste si istý, že chcete odstrániť tohto používateľa?", "Potvrdenie", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				bool isDeleted = await _memberService.DeleteMemberAsync(id);
				if (isDeleted)
				{
					await LoadUsersAsync();
				}
			}
		}
	}
}
