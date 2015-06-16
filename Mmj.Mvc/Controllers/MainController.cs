//
// MainController.cs
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
using System.IO;
using System.Linq;
using System.Reflection;
using Mmj.Configuration;
using Mmj.ConnectionWrapper;
using Mmj.OS;
using Mmj.Controllers.EventArguments;
using Mmj.Utilities;
using Mmj.Views;
using Mmj.Views.Windows;

namespace Mmj.Controllers
{
	public class MainController :IController
	{		
		JackdConfiguration _jackdConfiguration;
		List<AppConfiguration> _appConfigurations;
		IProgram _jackd;
		readonly IMainWindow _mainWindow;
		List<AppStartController> _startWidgetControllers = new List<AppStartController> ();
		readonly List<ConnectionController> _connectionControllers;
		readonly IStartupParameters _parameters;

		public MainController (string[] args)
		{
			_mainWindow = new MainWindow ();
			_mainWindow.Icon = Icons.Program;
			_mainWindow.Hide ();
			_connectionControllers = new List<ConnectionController> ();
			IConnectionManagerFactory factory =
				DependencyResolver.GetImplementation<IConnectionManagerFactory> ("IConnectionManagerFactory");
			foreach (IConnectionManager connectionManager in factory.GetConnectionManagers()) { 
				_connectionControllers.Add (new ConnectionController (connectionManager));
			}
			_mainWindow.ConnectionWidgets = _connectionControllers.Select (c => c.Widget);
			_parameters = DependencyResolver.GetImplementation<IStartupParameters> ("IStartupParameters", new object[] { args });
			PersistantConfiguration.SetConfigDirectory (_parameters.ConfigDirectory);
			Logger.SetLogFile (_parameters.LogFile);
		}

		~MainController ()
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
			if (_jackd != null) {
				_jackd.Dispose ();
			}
			for (int i = _startWidgetControllers.Count - 1; i >= 0; i--) {
				_startWidgetControllers [i].Dispose ();
			}
			for (int i = _connectionControllers.Count - 1; i >= 0; i--) {
				_connectionControllers [i].Dispose ();
			}
			_mainWindow.Dispose ();
		}

		public void Start ()
		{
			WindowConfiguration windowConfiguration;
			if (TryLoadWindowConfiguration (out windowConfiguration)) {
				_mainWindow.WindowConfiguration = windowConfiguration;
			}
			if (!TryLoadJackdConfiguration (out _jackdConfiguration)) {
				ConfigureJackd ();
			}
			if (TryLoadAppConfigurations (out _appConfigurations)) {
				UpdateApps (_appConfigurations);
			} else {
				ConfigureApps ();
			}

			_mainWindow.Show ();
			
			_mainWindow.StartJackd += MainWindow_StartJackd;
			_mainWindow.StopJackd += MainWindow_StopJackd;
			_mainWindow.StopAll += MainWindow_StopAll;
			_mainWindow.ShowConfigureJackd += MainWindow_ShowConfigureJackd;
			_mainWindow.ShowConfigureApps += MainWindow_ShowConfigureApps;
			_mainWindow.ShowAbout += MainWindow_ShowAbout;
			_mainWindow.ShowHelp += MainWindow_ShowHelp;
			_mainWindow.QuitApplication += MainWindow_QuitApplication;

			_mainWindow.Show ();

			if (_parameters.StartWithFullScreen) {
				_mainWindow.Fullscreen = true;
			}

			InitJackd (_jackdConfiguration);
			if (_parameters.StartWithJackd) {
				_jackd.Start ();
			}
			if (_parameters.ShowHelp) {
				ShowHelp ();
			}
		}

		bool TryLoadJackdConfiguration (out JackdConfiguration jackdConfig)
		{
			try {
				jackdConfig = PersistantConfiguration.LoadJackdConfiguration ();
				return true;
			} catch (System.Xml.XmlException ex) {
				Logger.LogException (ex);
				ShowInfoMessage ("Jackd configuration file is corrupt.");
				jackdConfig = new JackdConfiguration ();
			} catch (FileNotFoundException ex) {
				Logger.LogException (ex);
				ShowInfoMessage ("Jackd is not configured.");
				jackdConfig = new JackdConfiguration ();
			}
			return false;
		}

		void UpdateApps (IEnumerable<AppConfiguration> appConfigurations)
		{
			_startWidgetControllers = new List<AppStartController> ();
			foreach (AppConfiguration appConfig in appConfigurations) {
				AppStartController startWidgetController = new AppStartController (appConfig);
				startWidgetController.ApplicationStatusHasChanged += AppStartController_StatusHasChanged;
				_startWidgetControllers.Add (startWidgetController);
			}
			_mainWindow.AppStartWidgets = _startWidgetControllers.Select (c => c.Widget);
		}

		bool TryLoadAppConfigurations (out List<AppConfiguration> appConfigs)
		{
			try {
				appConfigs = PersistantConfiguration.LoadAppConfigurations ();
				return true;
			} catch (System.Xml.XmlException ex) {				
				Logger.LogException (ex);
				ShowInfoMessage ("Application configuration file is corrupt.");
				appConfigs = new List<AppConfiguration> ();
			} catch (FileNotFoundException ex) {				
				Logger.LogException (ex);
				ShowInfoMessage ("Applications are not configured.");
				appConfigs = new List<AppConfiguration> ();
			}
			return false;
		}

		bool TryLoadWindowConfiguration (out WindowConfiguration windowConfig)
		{
			try {
				windowConfig = PersistantConfiguration.LoadWindowSize ();
				if (Math.Abs (windowConfig.Width) > 1 && Math.Abs (windowConfig.Height) > 1) {
					return true;
				}
			} catch (Exception ex) {				
				Logger.LogException (ex);
			}
			windowConfig = new WindowConfiguration (0, 0, 0, 0);
			return false;
		}

		void InitJackd (JackdConfiguration jackdConfig)
		{
			if (_jackd != null) {
				_jackd.Stop ();
				_jackd.HasStarted -= Jackd_HasStarted;
				_jackd.HasExited -= Jackd_HasExited;
				_jackd.Dispose ();
			}
			_jackd = DependencyResolver.GetImplementation<IProgram> ("IProgram", new object[] { jackdConfig });
			_jackd.HasStarted += Jackd_HasStarted;
			_jackd.HasExited += Jackd_HasExited;
		}
		#region Model events
		void Jackd_HasStarted (object sender, EventArgs e)
		{
			Logger.LogMessage ("Jackd started", LogLevel.Info);
			UpdateRunningStatus ();
		}

		void Jackd_HasExited (object sender, EventArgs e)
		{
			Logger.LogMessage ("Jackd exited", LogLevel.Info);
			UpdateRunningStatus ();
		}
		#endregion
		#region IMainWindow events
		void MainWindow_StartJackd (object sender, EventArgs e)
		{
			Logger.LogMessage ("Starting jackd", LogLevel.Debug);
			if (_jackd.IsRunning) {
				_jackd.Stop ();
			}
			_jackd.Start ();
		}

		void MainWindow_StopJackd (object sender, EventArgs e)
		{
			Logger.LogMessage ("Stopping jackd", LogLevel.Debug);
			StopJackd ();
		}

		void MainWindow_StopAll (object sender, EventArgs e)
		{
			Logger.LogMessage ("Stopping all apps and jackd", LogLevel.Debug);
			StopJackd ();
			foreach (AppStartController startWidgetController in _startWidgetControllers) {
				startWidgetController.StopApplication ();
			}
		}

		void StopJackd ()
		{
			if (_jackd.IsRunning) {
				_jackd.Stop ();
			}

		}

		void MainWindow_ShowConfigureJackd (object sender, EventArgs e)
		{
			ConfigureJackd ();
		}

		void ConfigureJackd ()
		{
			JackdConfigController jackdConfigController = new JackdConfigController (_jackdConfiguration);
			jackdConfigController.UpdateJackd += Controller_UpdateJackd;
			jackdConfigController.AllWidgetsAreClosed += Controller_WidgetsAreClosed;
			_mainWindow.Sensitive = false;
		}

		void MainWindow_ShowConfigureApps (object sender, EventArgs e)
		{
			ConfigureApps ();
		}

		private void ConfigureApps ()
		{
			AppConfigController appConfigController = new AppConfigController (_appConfigurations);
			appConfigController.UpdateApps += Controller_UpdateApps;
			appConfigController.AllWidgetsAreClosed += Controller_WidgetsAreClosed;
			_mainWindow.Sensitive = false;
		}

		void MainWindow_ShowAbout (object sender, EventArgs e)
		{
			//TODO: Move to view.
			IAboutWindow aboutWindow = new AboutWindow ();
			aboutWindow.ProgramName = "MonoMultiJack";
			aboutWindow.Version = Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			aboutWindow.Copyright = "(c) Thomas Mayer 2009-2014";
			aboutWindow.Comments = @"MonoMultiJack is a simple tool for controlling Jackd and diverse audio programs.";
			aboutWindow.Website = "http://ix.residuum.org/";
			aboutWindow.Authors = new string[] { "Thomas Mayer" };
			aboutWindow.License = @"Copyright (c) 2009-2014 Thomas Mayer
	
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
	
THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
			aboutWindow.Icon = Icons.Info;
			aboutWindow.Show ();
			aboutWindow.Closing += Window_Closing;
		}

		void ShowHelp ()
		{
			IHelpWindow helpWindow = new HelpWindow ();
			helpWindow.ProgramName = "MonoMultiJack";
			helpWindow.HelpContent = _parameters.GetHelpText ();
			helpWindow.Icon = Icons.Help;
			helpWindow.Show ();
			helpWindow.Closing += Window_Closing;
		}

		void MainWindow_ShowHelp (object sender, EventArgs e)
		{
			ShowHelp ();
		}

		void MainWindow_QuitApplication (object sender, EventArgs e)
		{
			if (!_mainWindow.Fullscreen) {
				WindowConfiguration newWindowConfig = _mainWindow.WindowConfiguration;
				PersistantConfiguration.SaveWindowSize (newWindowConfig);
			}
			StopJackd ();
			if (AllWidgetsAreClosed != null) {
				AllWidgetsAreClosed (this, new EventArgs ());
			}
		}
		#endregion
		void Window_Closing (object sender, EventArgs e)
		{
			IWindow window = sender as IWindow;
			if (window == null) {
				return;
			}
			window.Dispose ();
			_mainWindow.Sensitive = true;
		}

		void Controller_WidgetsAreClosed (object sender, EventArgs e)
		{
			IController controller = sender as IController;
			if (controller != null) {
				controller.Dispose ();
				_mainWindow.Sensitive = true;
			}
		}

		void Controller_UpdateJackd (object sender, UpdateJackdEventArgs e)
		{
			_jackdConfiguration = e.JackdConfiguration;
			PersistantConfiguration.SaveJackdConfig (_jackdConfiguration);
			InitJackd (_jackdConfiguration);
		}

		void Controller_UpdateApps (object sender, UpdateAppsEventArgs e)
		{
			_appConfigurations = e.AppConfigurations;
			PersistantConfiguration.SaveAppConfiguations (_appConfigurations);
			UpdateApps (_appConfigurations);
		}

		void UpdateRunningStatus ()
		{
			_mainWindow.JackdIsRunning = _jackd.IsRunning;
			if (_startWidgetControllers.Any (startWidgetController => startWidgetController.IsRunning)) {
				_mainWindow.AppsAreRunning = true;
				return;
			}
			_mainWindow.AppsAreRunning = false;
		}

		void AppStartController_StatusHasChanged (object sender, EventArgs e)
		{
			UpdateRunningStatus ();
		}

		void ShowErrorMessage(string message){			
			Dialog.ShowErrorMessage(message);
		}

		void ShowInfoMessage (string message)
		{
			Dialog.ShowInfoMessage(message);
		}

		public event EventHandler AllWidgetsAreClosed;
	}
}
