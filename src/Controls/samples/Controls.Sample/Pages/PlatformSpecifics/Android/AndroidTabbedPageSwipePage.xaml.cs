﻿using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace Maui.Controls.Sample.Pages
{
	public partial class AndroidTabbedPageSwipePage : Microsoft.Maui.Controls.TabbedPage
	{
		ICommand? _returnToPlatformSpecificsPage;

		public AndroidTabbedPageSwipePage()
		{
			InitializeComponent();
		}

		public AndroidTabbedPageSwipePage(ICommand restore)
		{
			InitializeComponent();

			_returnToPlatformSpecificsPage = restore;
		}

		void OnSwipePagingButtonClicked(object sender, EventArgs e)
		{
			On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(!On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().IsSwipePagingEnabled());
		}

		void OnSmoothScrollButtonClicked(object sender, EventArgs e)
		{
			On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetIsSmoothScrollEnabled(!On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().IsSmoothScrollEnabled());
		}

		void OnReturnButtonClicked(object sender, EventArgs e)
		{
			_returnToPlatformSpecificsPage?.Execute(null);
		}
	}
}
