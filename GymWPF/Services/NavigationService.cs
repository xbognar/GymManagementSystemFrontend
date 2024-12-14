using GymWPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;

/// <summary>
/// Service for managing navigation between windows in the application.
/// Handles registration, opening, and closing of windows with optional ViewModel binding.
/// </summary>
public class NavigationService : INavigationService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly Dictionary<string, Type> _windowRegistry = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="NavigationService"/> class.
	/// </summary>
	/// <param name="serviceProvider">The service provider for resolving window instances.</param>
	public NavigationService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	/// <summary>
	/// Registers a window type with a specified key.
	/// </summary>
	/// <param name="key">The unique key identifying the window.</param>
	/// <param name="windowType">The type of the window to register.</param>
	public void RegisterWindow(string key, Type windowType)
	{
		if (!_windowRegistry.ContainsKey(key))
		{
			_windowRegistry.Add(key, windowType);
		}
	}

	/// <summary>
	/// Navigates to a window based on the registered key.
	/// Optionally assigns a ViewModel to the window's DataContext.
	/// </summary>
	/// <param name="key">The unique key identifying the window to navigate to.</param>
	/// <param name="viewModel">The ViewModel to bind to the window's DataContext (optional).</param>
	/// <exception cref="InvalidOperationException">Thrown if the specified key is not registered.</exception>
	public void NavigateTo(string key, object viewModel = null)
	{
		if (_windowRegistry.ContainsKey(key))
		{
			var window = (Window)_serviceProvider.GetService(_windowRegistry[key]);
			if (viewModel != null)
			{
				window.DataContext = viewModel;
			}
			window.Show();
		}
		else
		{
			throw new InvalidOperationException($"Window with key '{key}' not registered.");
		}
	}

	/// <summary>
	/// Closes the window associated with the specified key.
	/// </summary>
	/// <param name="key">The unique key identifying the window to close.</param>
	public void CloseWindow(string key)
	{
		if (_windowRegistry.ContainsKey(key))
		{
			var window = (Window)_serviceProvider.GetService(_windowRegistry[key]);
			window?.Close();
		}
	}
}
