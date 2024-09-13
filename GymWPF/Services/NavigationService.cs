using GymWPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;

public class NavigationService : INavigationService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly Dictionary<string, Type> _windowRegistry = new();

	public NavigationService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	// Registers a window type with a key
	public void RegisterWindow(string key, Type windowType)
	{
		if (!_windowRegistry.ContainsKey(key))
		{
			_windowRegistry.Add(key, windowType);
		}
	}

	// Navigates to a window based on the key
	public void NavigateTo(string key, object viewModel = null)
	{
		if (_windowRegistry.ContainsKey(key))
		{
			var window = (Window)_serviceProvider.GetService(_windowRegistry[key]);
			if (viewModel != null)
			{
				window.DataContext = viewModel;  // Set DataContext if a viewmodel is provided
			}
			window?.Show();
		}
		else
		{
			throw new InvalidOperationException($"Window with key '{key}' not registered.");
		}
	}


	// Closes the active window
	public void CloseWindow(string key)
	{
		if (_windowRegistry.ContainsKey(key))
		{
			var window = (Window)_serviceProvider.GetService(_windowRegistry[key]);
			window?.Close();
		}
	}
}
