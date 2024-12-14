using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GymWPF.ViewModels
{
	/// <summary>
	/// A base ViewModel class that provides property change notifications.
	/// Inherits from ObservableObject to leverage the MVVM framework's built-in functionality.
	/// </summary>
	public class BaseViewModel : ObservableObject
	{
		/// <summary>
		/// Sets the backing field of a property and raises the PropertyChanged event if the value has changed.
		/// </summary>
		/// <typeparam name="T">The type of the property being updated.</typeparam>
		/// <param name="backingStore">The reference to the backing field of the property.</param>
		/// <param name="value">The new value to assign to the property.</param>
		/// <param name="propertyName">The name of the property (automatically provided by the CallerMemberName attribute).</param>
		/// <param name="onChanged">An optional action to invoke after the property value has changed.</param>
		/// <returns>True if the property value was updated, false if the value was unchanged.</returns>
		protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
		{
			// Check if the new value is the same as the current value.
			if (EqualityComparer<T>.Default.Equals(backingStore, value))
				return false;

			// Update the backing field and invoke any onChanged action.
			backingStore = value;
			onChanged?.Invoke();

			// Notify listeners of the property change.
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}
