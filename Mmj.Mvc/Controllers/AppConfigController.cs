//
// AppConfigController.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2009-2015 Thomas Mayer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using Mmj.Configuration;
using Mmj.Controllers.EventArguments;
using Mmj.Views;
using Mmj.Views.Widgets;
using Mmj.Views.Windows;

namespace Mmj.Controllers
{
	public class AppConfigController : IController
	{
		readonly IAppConfigWindow _configWindow;
		readonly List<IAppConfigWidget> _widgets = new List<IAppConfigWidget> ();

		public AppConfigController (IEnumerable<AppConfiguration> appConfigurations)
		{
			_configWindow = new AppConfigWindow { Icon = Icons.Program };
			_configWindow.Show ();
			_configWindow.Closing += Window_Closing;
			_configWindow.AddApplication += Window_AddApplication;
			_configWindow.SaveApplicationConfigs += Window_SaveConfigs;
			foreach (AppConfiguration config in appConfigurations) {
				IAppConfigWidget widget = CreateWidget ();	
				widget.Name = config.Name;
				widget.Command = config.Command;
				widget.Arguments = config.Arguments;			
				_configWindow.AddAppConfigWidget (widget);
				_widgets.Add (widget);
			}
		}

		~AppConfigController ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool isDisposing)
		{
			for (int i = _widgets.Count - 1; i >= 0; i--) {
				_widgets [i].Dispose ();
			}
			_configWindow.Dispose ();
		}

		IAppConfigWidget CreateWidget ()
		{
			IAppConfigWidget widget = new AppConfigWidget ();
			widget.RemoveApplication += Widget_RemoveApplication;
			return widget;
		}

		void Widget_RemoveApplication (object sender, EventArgs e)
		{
			IAppConfigWidget widget = sender as IAppConfigWidget;
			if (widget == null) {
				return;
			}
			_configWindow.RemoveAppConfigWidget (widget);
			_widgets.Remove (widget);
			widget.Dispose ();
		}

		void Window_SaveConfigs (object sender, EventArgs e)
		{
			if (UpdateApps != null) {
				List<AppConfiguration> newConfigurations = new List<AppConfiguration> ();
				foreach (IAppConfigWidget widget in _widgets) {
					AppConfiguration newConfig = new AppConfiguration (widget.Name, widget.Command, widget.Arguments);
					newConfigurations.Add (newConfig);
				}
				UpdateApps (this, new UpdateAppsEventArgs { AppConfigurations = newConfigurations });
			}

		}

		void Window_AddApplication (object sender, EventArgs e)
		{
			IAppConfigWidget newWidget = CreateWidget ();
			_configWindow.AddAppConfigWidget (newWidget);
			_widgets.Add (newWidget);
		}

		void Window_Closing (object sender, EventArgs e)
		{				
			IWindow window = sender as IWindow;
			if (window != null) {
				window.Dispose ();
			}
			if (AllWidgetsAreClosed != null) {
				AllWidgetsAreClosed (this, new EventArgs ());
			}
		}
		#region IController implementation
		public event EventHandler AllWidgetsAreClosed;
		#endregion
		public event EventHandler<UpdateAppsEventArgs> UpdateApps;
	}
}
