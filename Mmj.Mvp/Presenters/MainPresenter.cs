//
// MainController.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2009-2016 Thomas Mayer
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
using Mmj.ConnectionWrapper;
using Mmj.OS;
using Mmj.FileOperations;
using Mmj.FileOperations.Configuration;
using Mmj.FileOperations.Snapshot;
using Mmj.Presenters.EventArguments;
using Mmj.Utilities;
using Mmj.Views;
using Mmj.Views.Windows;
using Connection = Mmj.FileOperations.Snapshot.Connection;
using Xwt;

namespace Mmj.Presenters
{
	public class MainPresenter :IPresenter
	{
		JackdConfiguration _jackdConfiguration;
		List<AppConfiguration> _appConfigurations;
		IProgram _jackd;
		readonly IMainWindow _mainWindow;
		List<AppStartPresenter> _startWidgetControllers = new List<AppStartPresenter> ();
		readonly List<ConnectionPresenter> _connectionControllers;
		readonly IStartupParameters _parameters;

		public MainPresenter (string[] args)
		{
			_mainWindow = new MainWindow ();
			_mainWindow.Icon = Icons.Program;
			_mainWindow.Hide ();
			_connectionControllers = new List<ConnectionPresenter> ();
			IConnectionManagerFactory factory =
				DependencyResolver.GetImplementation<IConnectionManagerFactory> ("IConnectionManagerFactory");
			foreach (IConnectionManager connectionManager in factory.GetConnectionManagers()) { 
				_connectionControllers.Add (new ConnectionPresenter (connectionManager));
			}
			_mainWindow.ConnectionWidgets = _connectionControllers.Select (c => c.Widget);
			_parameters = DependencyResolver.GetImplementation<IStartupParameters> ("IStartupParameters", new object[] { args });
			Persister.SetConfigDirectory (_parameters.ConfigDirectory);
			Logging.SetLogFile (_parameters.LogFile);
		}

		~MainPresenter ()
		{
			Dispose (false);
			GC.SuppressFinalize (this);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool isDisposing)
		{
			if (_jackd != null) {
				_jackd.HasStarted -= Jackd_HasStarted;
				_jackd.HasExited -= Jackd_HasExited;
				_jackd.Dispose ();
			}
			for (int i = _startWidgetControllers.Count - 1; i >= 0; i--) {
				_startWidgetControllers [i].ApplicationStatusHasChanged -= AppStartController_StatusHasChanged;
				_startWidgetControllers [i].Dispose ();
			}
			for (int i = _connectionControllers.Count - 1; i >= 0; i--) {
				_connectionControllers [i].Dispose ();
			}
			_mainWindow.StartJackd -= MainWindow_StartJackd;
			_mainWindow.StopJackd -= MainWindow_StopJackd;
			_mainWindow.StopAll -= MainWindow_StopAll;
			_mainWindow.ConfigureJackd -= MainWindowConfigureJackd;
			_mainWindow.ConfigureApps -= MainWindowConfigureApps;
			_mainWindow.About -= MainWindowAbout;
			_mainWindow.Help -= MainWindowHelp;
			_mainWindow.Quit -= MainWindowQuit;
			_mainWindow.SaveSnapshot -= MainWindowSave;
			_mainWindow.LoadSnapshot -= MainWindowLoad;
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
			_mainWindow.ConfigureJackd += MainWindowConfigureJackd;
			_mainWindow.ConfigureApps += MainWindowConfigureApps;
			_mainWindow.About += MainWindowAbout;
			_mainWindow.Help += MainWindowHelp;
			_mainWindow.Quit += MainWindowQuit;
			_mainWindow.SaveSnapshot += MainWindowSave;
			_mainWindow.LoadSnapshot += MainWindowLoad;

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
				jackdConfig = Persister.LoadJackdConfiguration ();
				return true;
			} catch (System.Xml.XmlException ex) {
				Logging.LogException (ex);
				ShowInfoMessage ("Jackd configuration file is corrupt.");
				jackdConfig = new JackdConfiguration ();
			} catch (FileNotFoundException ex) {
				Logging.LogException (ex);
				ShowInfoMessage ("Jackd is not configured.");
				jackdConfig = new JackdConfiguration ();
			}
			return false;
		}

		void UpdateApps (IEnumerable<AppConfiguration> appConfigurations)
		{
			_startWidgetControllers = new List<AppStartPresenter> ();
			foreach (AppConfiguration appConfig in appConfigurations) {
				AppStartPresenter startWidgetPresenter = new AppStartPresenter (appConfig);
				startWidgetPresenter.ApplicationStatusHasChanged += AppStartController_StatusHasChanged;
				_startWidgetControllers.Add (startWidgetPresenter);
			}
			_mainWindow.AppStartWidgets = _startWidgetControllers.Select (c => c.Widget);
		}

		bool TryLoadAppConfigurations (out List<AppConfiguration> appConfigs)
		{
			try {
				appConfigs = Persister.LoadAppConfigurations ();
				return true;
			} catch (System.Xml.XmlException ex) {				
				Logging.LogException (ex);
				ShowInfoMessage ("Application configuration file is corrupt.");
				appConfigs = new List<AppConfiguration> ();
			} catch (FileNotFoundException ex) {				
				Logging.LogException (ex);
				ShowInfoMessage ("Applications are not configured.");
				appConfigs = new List<AppConfiguration> ();
			}
			return false;
		}

		bool TryLoadWindowConfiguration (out WindowConfiguration windowConfig)
		{
			try {
				windowConfig = Persister.LoadWindowSize ();
				if (Math.Abs (windowConfig.Width) > 1 && Math.Abs (windowConfig.Height) > 1) {
					return true;
				}
			} catch (Exception ex) {				
				Logging.LogException (ex);
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
			Logging.LogMessage ("Jackd started", LogLevel.Info);
			UpdateRunningStatus ();
		}

		void Jackd_HasExited (object sender, EventArgs e)
		{
			Logging.LogMessage ("Jackd exited", LogLevel.Info);
			UpdateRunningStatus ();
		}

		#endregion

		#region IMainWindow events

		void MainWindow_StartJackd (object sender, EventArgs e)
		{
			Logging.LogMessage ("Starting jackd", LogLevel.Debug);
			if (_jackd.IsRunning) {
				_jackd.Stop ();
			}
			_jackd.Start ();
		}

		void MainWindow_StopJackd (object sender, EventArgs e)
		{
			Logging.LogMessage ("Stopping jackd", LogLevel.Debug);
			StopJackd ();
		}

		void MainWindow_StopAll (object sender, EventArgs e)
		{
			Logging.LogMessage ("Stopping all apps and jackd", LogLevel.Debug);
			StopJackd ();
			foreach (AppStartPresenter startWidgetController in _startWidgetControllers) {
				startWidgetController.StopApplication ();
			}
		}

		void StopJackd ()
		{
			if (_jackd.IsRunning) {
				_jackd.Stop ();
			}

		}

		void MainWindowConfigureJackd (object sender, EventArgs e)
		{
			ConfigureJackd ();
		}

		void ConfigureJackd ()
		{
			JackdConfigPresenter jackdConfigPresenter = new JackdConfigPresenter (_jackdConfiguration);
			jackdConfigPresenter.UpdateJackd += Controller_UpdateJackd;
			jackdConfigPresenter.AllWidgetsAreClosed += Controller_WidgetsAreClosed;
			_mainWindow.Sensitive = false;
		}

		void MainWindowConfigureApps (object sender, EventArgs e)
		{
			ConfigureApps ();
		}

		void ConfigureApps ()
		{
			AppConfigPresenter appConfigPresenter = new AppConfigPresenter (_appConfigurations);
			appConfigPresenter.UpdateApps += Controller_UpdateApps;
			appConfigPresenter.AllWidgetsAreClosed += Controller_WidgetsAreClosed;
			_mainWindow.Sensitive = false;
		}

		void MainWindowAbout (object sender, EventArgs e)
		{
			//TODO: Move to view.
			IAboutWindow aboutWindow = new AboutWindow ();
			aboutWindow.ProgramName = "MonoMultiJack";
			aboutWindow.Version = Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			aboutWindow.Copyright = "(c) Thomas Mayer 2009-2015";
			aboutWindow.Comments = I18N._ (@"MonoMultiJack is a simple tool for controlling Jackd and diverse audio programs.");
			aboutWindow.Website = "http://ix.residuum.org/";
			aboutWindow.Authors = new string[] { "Thomas Mayer" };
			aboutWindow.License = @"Copyright (c) 2009-2015 Thomas Mayer

				Permission is hereby granted, free of charge, to any person obtaining
				a copy of this software and associated documentation files (the
						""Software""), to deal in the Software without restriction, including
				without limitation the rights to use, copy, modify, merge, publish,
					distribute, sublicense, and/or sell copies of the Software, and to
						permit persons to whom the Software is furnished to do so, subject to
						the following conditions:

						The above copyright notice and this permission notice shall be
						included in all copies or substantial portions of the Software.

						THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
					EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
						MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
						IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
						CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
					TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
						SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


						*Additional Components used*

						**NLog**

						Copyright (c) 2004-2011 Jaroslaw Kowalski <jaak@jkowalski.net>

						All rights reserved.

						Redistribution and use in source and binary forms, with or without
						modification, are permitted provided that the following conditions are
						met:

						* Redistributions of source code must retain the above copyright
						notice, this list of conditions and the following disclaimer. 
						* Redistributions in binary form must reproduce the above copyright
						notice, this list of conditions and the following disclaimer in the
						documentation and/or other materials provided with the distribution. 
						* Neither the name of Jaroslaw Kowalski nor the names of its
						contributors may be used to endorse or promote products derived from
						this software without specific prior written permission. 

						THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
						""AS IS"" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
						LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
						A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
						OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
					SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
							LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
							DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
						THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
						(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
						OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


						**XWT**

						Copyright (c) 2011 Xamarin Inc

						Permission is hereby granted, free of charge, to any person obtaining
						a copy of this software and associated documentation files (the
								""Software""), to deal in the Software without restriction, including
						without limitation the rights to use, copy, modify, merge, publish,
					distribute, sublicense, and/or sell copies of the Software, and to
						permit persons to whom the Software is furnished to do so, subject to
						the following conditions:

						The above copyright notice and this permission notice shall be
						included in all copies or substantial portions of the Software.

						THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
					EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
						MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
						IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
						CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
					TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
						SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


						**NGettext**

						Copyright (c) 2012 Neris Ereptoris (www.neris.ws)

						Permission is hereby granted, free of charge, to any person obtaining
						a copy of this software and associated documentation files (the
								""Software""), to deal in the Software without restriction, including
						without limitation the rights to use, copy, modify, merge, publish,
					distribute, sublicense, and/or sell copies of the Software, and to
						permit persons to whom the Software is furnished to do so, subject to
						the following conditions:

						The above copyright notice and this permission notice shall be
						included in all copies or substantial portions of the Software.

						THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
					EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
						MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
						IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
						CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
					TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
						SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
						";
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

		void MainWindowHelp (object sender, EventArgs e)
		{
			ShowHelp ();
		}

		void MainWindowQuit (object sender, EventArgs e)
		{
			if (!_mainWindow.Fullscreen) {
				WindowConfiguration newWindowConfig = _mainWindow.WindowConfiguration;
				Persister.SaveWindowSize (newWindowConfig);
			}
			StopJackd ();
			if (AllWidgetsAreClosed != null) {
				AllWidgetsAreClosed (this, new EventArgs ());
			}
		}

		void MainWindowSave (object sender, EventArgs e)
		{
			string fileName = _mainWindow.SaveFileDialog (Persister.SnapshotFolder, "Save Snapshot", "snap");
			if (fileName == null) {
				return;
			}
			IEnumerable<string> apps = _startWidgetControllers.Where (s => s.IsRunning).Select (s => s.Name);

			IEnumerable<Connection> connections = _connectionControllers.SelectMany (c => c.Connections).Select (c => new Connection (c.InPort.Name, c.OutPort.Name, (int)c.ConnectionType));
			Moment snap = new Moment (apps, connections);
			Persister.SaveSnapshot (snap, fileName);
		}

		void MainWindowLoad (object sender, EventArgs e)
		{
			string fileName = _mainWindow.OpenFileDialog (Persister.SnapshotFolder, "Load Snapshot", "snap");
			if (fileName == null) {
				return;
			}
			Moment snap = Persister.LoadSnapshot (fileName);
			if (!_jackd.IsRunning && snap.Apps.Any ()) {
				_jackd.Start ();
			}
			foreach (AppStartPresenter appStarter in _startWidgetControllers.Where(s => s.IsRunning && !snap.Apps.Contains(s.Name))) {
				appStarter.StopApplication ();
			}
			foreach (AppStartPresenter appStarter in _startWidgetControllers.Where(s => !s.IsRunning && snap.Apps.Contains(s.Name))) {
				appStarter.StartApplication ();
			}
			int trials = 0;
			Application.TimeoutInvoke (500, () => {
				bool allFound = true;
				foreach (IGrouping<int, Connection> connections in snap.Connections.GroupBy(c => c.Type)) {
					ConnectionPresenter presenter = _connectionControllers.SingleOrDefault (c => (int)c.ConnectionType == connections.Key);
					if (presenter == null) {
						continue;
					}
					foreach (IConnection connection in presenter.Connections) {
						presenter.Disconnect (connection);
					}
					foreach (Connection connection in connections) {
						allFound = allFound && presenter.Connect (connection.OutPort, connection.InPort);
					}
				}
				trials++;
				return !(allFound || trials >= 20);
			});
		}

		#endregion

		void Window_Closing (object sender, EventArgs e)
		{
			IWindow window = sender as IWindow;
			if (window == null) {
				return;
			}
			window.Closing -= Window_Closing;
			window.Dispose ();
			_mainWindow.Sensitive = true;
		}

		void Controller_WidgetsAreClosed (object sender, EventArgs e)
		{
			JackdConfigPresenter jackPresenter = sender as JackdConfigPresenter;
			if (jackPresenter != null) {
				jackPresenter.UpdateJackd -= Controller_UpdateJackd;
				jackPresenter.AllWidgetsAreClosed -= Controller_WidgetsAreClosed;
			}
			AppConfigPresenter appConfigPresenter = sender as AppConfigPresenter;
			if (appConfigPresenter != null) {
				appConfigPresenter.UpdateApps += Controller_UpdateApps;
				appConfigPresenter.AllWidgetsAreClosed += Controller_WidgetsAreClosed;
			}
			IPresenter presenter = sender as IPresenter;
			if (presenter != null) {
				presenter.Dispose ();
				_mainWindow.Sensitive = true;
			}
		}

		void Controller_UpdateJackd (object sender, UpdateJackdEventArgs e)
		{
			_jackdConfiguration = e.JackdConfiguration;
			Persister.SaveJackdConfig (_jackdConfiguration);
			InitJackd (_jackdConfiguration);
		}

		void Controller_UpdateApps (object sender, UpdateAppsEventArgs e)
		{
			_appConfigurations = e.AppConfigurations.ToList ();
			Persister.SaveAppConfiguations (_appConfigurations);
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

		void ShowErrorMessage (string message)
		{			
			_mainWindow.ShowErrorMessage (message);
		}

		void ShowInfoMessage (string message)
		{
			_mainWindow.ShowInfoMessage (message);
		}

		public event EventHandler AllWidgetsAreClosed;
	}
}
