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
/// ViewModel for displaying a large list of chips (either active or inactive).
/// Handles loading chips and deleting them based on user actions.
/// </summary>
public class LargeChipViewModel : ObservableObject
{
	private readonly IChipService _chipService;

	private bool _isActive;

	public ObservableCollection<ChipDTO> Chips { get; } = new ObservableCollection<ChipDTO>();

	public ICommand DeleteChipCommand { get; }

	/// <summary>
	/// Initializes a new instance of LargeChipViewModel.
	/// Loads the chips and sets up the delete command.
	/// </summary>
	/// <param name="chipService">Service for managing chip data.</param>
	/// <param name="isActive">Specifies whether to load active or inactive chips.</param>
	public LargeChipViewModel(IChipService chipService, bool isActive)
	{
		_chipService = chipService;
		_isActive = isActive;

		DeleteChipCommand = new AsyncRelayCommand<object>(DeleteChipAsync);

		LoadChipsAsync();
	}

	/// <summary>
	/// Loads chips from the backend based on their active or inactive status.
	/// Displays an error message if the operation fails.
	/// </summary>
	private async Task LoadChipsAsync()
	{
		try
		{
			Chips.Clear(); 

			if (_isActive)
			{
				var activeChips = await _chipService.GetActiveChipsAsync();
				if (activeChips != null)
				{
					foreach (var chip in activeChips)
						Chips.Add(chip);
				}
			}
			else
			{
				var inactiveChips = await _chipService.GetInactiveChipsAsync();
				if (inactiveChips != null)
				{
					foreach (var chip in inactiveChips)
						Chips.Add(chip);
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Chyba pri načítaní čipov: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	/// <summary>
	/// Deletes a chip based on its ID after user confirmation.
	/// Reloads the list of chips upon successful deletion.
	/// </summary>
	/// <param name="chipID">The ID of the chip to delete.</param>
	private async Task DeleteChipAsync(object chipID)
	{
		if (chipID is int id)
		{
			var result = MessageBox.Show("Ste si istý, že chcete odstrániť tento čip?", "Potvrdenie", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				bool isDeleted = await _chipService.DeleteChipAsync(id);
				if (isDeleted)
				{
					await LoadChipsAsync();
				}
			}
		}
	}
}
