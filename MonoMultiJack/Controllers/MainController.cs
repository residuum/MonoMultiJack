//
// MainPresenter.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2012 Thomas Mayer
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
using System.Linq;
using MonoMultiJack.Forms;
using MonoMultiJack.Configuration;
using System.Collections.Generic;
using MonoMultiJack.OS;
using System.Reflection;
using System.IO;
using MonoMultiJack.Controllers.EventArguments;
using MonoMultiJack.ConnectionWrapper;

namespace MonoMultiJack.Controllers
{
	public class MainController :IController
	{				
		readonly string IconFile = "monomultijack.png";
		string _programIcon;

		private string ProgramIconPath {
			get {
				if (_programIcon == null) {
					Assembly executable = Assembly.GetEntryAssembly ();
					string baseDir = Path.GetDirectoryName (executable.Location);
					_programIcon = Path.Combine (baseDir, IconFile);
				}
				return _programIcon;
			}
		}

		JackdConfiguration _jackdConfiguration;
		List<AppConfiguration> _appConfigurations;
		IProgram _jackd;
	    readonly IMainWindow _mainWindow;
		List<AppStartController> _startWidgetControllers = new List<AppStartController>();
	    readonly List<ConnectionController> _connectionControllers;

		public MainController ()
		{
			_mainWindow = new Forms.MainWindow ();
			_mainWindow.IconPath = _programIcon;
			_mainWindow.Hide ();
			_connectionControllers = new List<ConnectionController> ();
		    IConnectionManagerFactory factory =
		        DependencyResolver.GetImplementation<IConnectionManagerFactory>("IConnectionManagerFactoryImplementation");
			foreach (IConnectionManager connectionManager in factory.GetConnectionManagers()) { 
				_connectionControllers.Add (new ConnectionController (connectionManager));
			}
			_mainWindow.ConnectionWidgets = _connectionControllers.Select(c => c.Widget);
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
			TryLoadJackdConfiguration (out _jackdConfiguration);
			if (TryLoadAppConfigurations (out _appConfigurations)) {
				UpdateApps (_appConfigurations);
			}			
			if (TryLoadWindowConfiguration (out windowConfiguration)) {
				_mainWindow.WindowConfiguration = windowConfiguration;
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

			InitJackd (_jackdConfiguration);
		}

		bool TryLoadJackdConfiguration (out JackdConfiguration jackdConfig)
		{
			try {
				jackdConfig = PersistantConfiguration.LoadJackdConfiguration ();
				return true;
			} catch (System.Xml.XmlException e) {
				#if DEBUG
				Console.WriteLine (e.Message);
				#endif
				ShowInfoMessage ("Jackd configuration File is corrupt.");
				jackdConfig = new JackdConfiguration ();
			} catch (FileNotFoundException e) {
				#if DEBUG
				Console.WriteLine (e.Message);
				#endif
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
			} catch (System.Xml.XmlException e) {
				#if DEBUG
				Console.WriteLine (e.Message);
				#endif
				ShowInfoMessage ("Application configuration File is corrupt.");
				appConfigs = new List<AppConfiguration> ();
			} catch (FileNotFoundException e) {
				#if DEBUG
				Console.WriteLine (e.Message);
				#endif
				ShowInfoMessage ("Applications are not configured.");
				appConfigs = new List<AppConfiguration> ();
			}
			return false;
		}

		bool TryLoadWindowConfiguration (out WindowConfiguration windowConfig)
		{
			try {
				windowConfig = PersistantConfiguration.LoadWindowSize ();
				if (windowConfig.XSize != 0 && windowConfig.YSize != 0) {
					return true;
				}
			} catch (Exception e) {
                #if DEBUG
                Console.WriteLine (e.Message);
                #endif
			}
            windowConfig = new WindowConfiguration(0, 0, 0, 0);
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
			_jackd = DependencyResolver.GetImplementation<IProgram>("IProgramImplementation", new object[]{jackdConfig});
			_jackd.HasStarted += Jackd_HasStarted;
			_jackd.HasExited += Jackd_HasExited;
		}

#region Model events
		void Jackd_HasStarted (object sender, EventArgs e)
		{
			_mainWindow.JackdIsRunning = true;
			_mainWindow.AppsAreRunning = true;
		}

		void Jackd_HasExited (object sender, EventArgs e)
		{
			_mainWindow.JackdIsRunning = false;
		}
#endregion

#region IMainWindow events
		void MainWindow_StartJackd (object sender, EventArgs e)
		{
			if (_jackd.IsRunning) {
				_jackd.Stop ();
			}
			_jackd.Start ();
		}

		void MainWindow_StopJackd (object sender, EventArgs e)
		{
			if (_jackd.IsRunning) {
				_jackd.Stop ();
			}
		}

		void MainWindow_StopAll (object sender, EventArgs e)
		{
			StopAllApplications ();
			foreach (AppStartController startWidgetController in _startWidgetControllers) {
				startWidgetController.StopApplication ();
			}
		}

		void StopAllApplications ()
		{
			if (_jackd.IsRunning) {
				_jackd.Stop ();
			}

		}

		void MainWindow_ShowConfigureJackd (object sender, EventArgs e)
		{
			JackdConfigController jackdConfigController = new JackdConfigController (_jackdConfiguration);
			jackdConfigController.UpdateJackd += Controller_UpdateJackd;
			jackdConfigController.AllWidgetsAreClosed += Controller_WidgetsAreClosed;
			_mainWindow.Sensitive = false;
		}

		void MainWindow_ShowConfigureApps (object sender, EventArgs e)
		{
			AppConfigController appConfigController = new AppConfigController (_appConfigurations);
			appConfigController.UpdateApps += Controller_UpdateApps;
			appConfigController.AllWidgetsAreClosed += Controller_WidgetsAreClosed;
			_mainWindow.Sensitive = false;
		}

		void MainWindow_ShowAbout (object sender, EventArgs e)
		{
			//TODO: Move to view.
			IAboutWindow AboutWindow = new AboutWindow ();
			AboutWindow.ProgramName = "MonoMultiJack";
			AboutWindow.Version = "0.2";
			AboutWindow.Copyright = "(c) Thomas Mayer 2013";
			AboutWindow.Comments = @"MonoMultiJack is a simple tool for controlling Jackd and diverse audio 
	programs.";
			AboutWindow.Website = "http://ix.residuum.org/";
			AboutWindow.Authors = new String[] {"Thomas Mayer"};
			AboutWindow.License = @"Copyright (c) 2009-2013 Thomas Mayer
	
	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the ""Software""), to deal

	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:
	
	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.
	
	THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.";
			AboutWindow.IconPath = ProgramIconPath;
			AboutWindow.Show ();
			AboutWindow.Closing += Window_Closing;
		}

		void MainWindow_ShowHelp (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		void MainWindow_QuitApplication (object sender, EventArgs e)
		{
			WindowConfiguration newWindowConfig = _mainWindow.WindowConfiguration;
			PersistantConfiguration.SaveWindowSize (newWindowConfig);
			StopAllApplications ();
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
			window.Destroy ();
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

		void AppStartController_StatusHasChanged (object sender, EventArgs e)
		{
			if (_jackd.IsRunning) {
				_mainWindow.AppsAreRunning = true;
				return;
			}
			foreach (AppStartController startWidgetController in _startWidgetControllers) {
				if (startWidgetController.IsRunning) {
					_mainWindow.AppsAreRunning = true;
					return;
				}
			}
			_mainWindow.AppsAreRunning = false;
		}

		void ShowInfoMessage (string message)
		{
			IInfoWindow messageWindow = new InfoWindow ();
			messageWindow.Message = message;
			messageWindow.Closing += Window_Closing;
			messageWindow.Show ();
		}

		public event EventHandler AllWidgetsAreClosed;

	}
}