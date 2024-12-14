using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymWPF.Models;
using GymWPF.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

public class LargeChipViewModel : ObservableObject
{
	private readonly IChipService _chipService;
	private bool _isActive;

	public ObservableCollection<ChipDTO> Chips { get; } = new ObservableCollection<ChipDTO>();
	public ICommand DeleteChipCommand { get; }

	public LargeChipViewModel(IChipService chipService, bool isActive)
	{
		_chipService = chipService;
		_isActive = isActive;

		DeleteChipCommand = new AsyncRelayCommand<object>(DeleteChipAsync);

		LoadChipsAsync();
	}

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
