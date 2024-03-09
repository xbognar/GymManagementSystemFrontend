using GymWPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace GymWPF.Services
{
	public class NavigationService : INavigationService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly Dictionary<string, Type> _pagesByKey;
		private readonly Stack<UserControl> _navigationHistory = new Stack<UserControl>();
		private ContentControl _mainContent;

		public NavigationService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			_pagesByKey = new Dictionary<string, Type>();
		}

		public void Configure(string key, Type pageType)
		{
			if (!_pagesByKey.ContainsKey(key))
			{
				_pagesByKey.Add(key, pageType);
			}
		}

		public void NavigateTo(string pageKey)
		{
			if (_pagesByKey.ContainsKey(pageKey))
			{
				var control = _serviceProvider.GetService(_pagesByKey[pageKey]) as UserControl;
				_navigationHistory.Push(control); 
				_mainContent.Content = control;
			}
			else
			{
				throw new ArgumentException($"No such page: {pageKey}", nameof(pageKey));
			}
		}

		public void SetMainContent(ContentControl mainContent)
		{
			_mainContent = mainContent;
		}

	}
}
