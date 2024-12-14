using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

/// <summary>
/// ViewModel for displaying and managing all users in the application.
/// Handles loading, displaying, and deleting users.
/// </summary>
public class AllUsersViewModel : ObservableObject
{
	private readonly IMemberService _memberService;

	public ObservableCollection<Member> Users { get; } = new ObservableCollection<Member>();

	public ICommand DeleteUserCommand { get; }

	/// <summary>
	/// Initializes a new instance of the AllUsersViewModel.
	/// Loads all users and sets up the delete command.
	/// </summary>
	/// <param name="memberService">Service for managing member data.</param>
	public AllUsersViewModel(IMemberService memberService)
	{
		_memberService = memberService;

		DeleteUserCommand = new AsyncRelayCommand<object>(DeleteUserAsync);

		LoadUsersAsync();
	}

	/// <summary>
	/// Loads all users from the backend and populates the Users collection.
	/// Displays an error message if the operation fails.
	/// </summary>
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

	/// <summary>
	/// Deletes a user based on their ID after user confirmation.
	/// Reloads the list of users upon successful deletion.
	/// </summary>
	/// <param name="memberID">The ID of the user to delete.</param>
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
