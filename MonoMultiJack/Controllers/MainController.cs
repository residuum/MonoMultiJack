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
using MonoMultiJack.BusinessLogic.Configuration;
using System.Collections.Generic;
using MonoMultiJack.BusinessLogic.Common;
using MonoMultiJack.Widgets;
using Gtk;
using System.Reflection;

namespace MonoMultiJack.Controllers
{
	public class MainController :IDisposable
	{				
		readonly string IconFile = "monomultijack.png";

		string _programIcon;

		private string ProgramIconPath {
			get {
				if (_programIcon == null) {
					Assembly executable = Assembly.GetEntryAssembly();
					string baseDir = System.IO.Path.GetDirectoryName(executable.Location);
					_programIcon = System.IO.Path.Combine(baseDir, IconFile);
				}
				return _programIcon;
			}
		}

		/// <summary>
		/// Jackd status messages
		/// </summary>
		readonly string JackdStatusRunning = "Jackd is running.";
		readonly string JackdStatusStopped = "Jackd is stopped.";
		JackdConfiguration _jackdConfiguration;
		List<AppConfiguration> _appConfigurations;
		ProgramManagement _jackd;
		IMainWindow _mainWindow;
		List<AppStartWidgetController> _startWidgetControllers;

		public MainController()
		{
			_mainWindow = new Forms.MainWindow();
			_mainWindow.IconPath = _programIcon;
			_mainWindow.Hide();
		}

		~MainController()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	
		protected virtual void Dispose(bool isDisposing)
		{
			if (_jackd != null) {
				_jackd.Dispose();
			}
			for (int i = _startWidgetControllers.Count - 1; i >= 0; i--) {
				_startWidgetControllers [i].Dispose();
			}
			_mainWindow.Dispose();
		}

		public void Start()
		{	
			_jackdConfiguration = PersistantConfiguration.LoadJackdConfiguration();
			_appConfigurations = PersistantConfiguration.LoadAppConfigurations();

			WindowConfiguration windowConfiguration = PersistantConfiguration.LoadWindowSize();
			_mainWindow.WindowConfiguration = windowConfiguration;
			_mainWindow.Show();

			_startWidgetControllers = new List<AppStartWidgetController>();
			foreach (AppConfiguration appConfig in _appConfigurations) {
				AppStartWidgetController startWidgetController = new AppStartWidgetController(appConfig);
				startWidgetController.ApplicationStatusHasChanged += AppStartController_StatusHasChanged;
				_startWidgetControllers.Add(startWidgetController);
			}
			_mainWindow.AppStartWidgets = _startWidgetControllers.Select(c => c.Widget);
			_mainWindow.Status = JackdStatusStopped;
			
			_mainWindow.StartJackd += MainWindow_StartJackd;
			_mainWindow.StopJackd += MainWindow_StopJackd;
			_mainWindow.StopAll += MainWindow_StopAll;
			_mainWindow.ShowConfigureJackd += MainWindow_ShowConfigureJackd;
			_mainWindow.ShowConfigureApps += MainWindow_ShowConfigureApps;
			_mainWindow.ShowAbout += MainWindow_ShowAbout;
			_mainWindow.ShowHelp += MainWindow_ShowHelp;
			_mainWindow.QuitApplication += MainWindow_QuitApplication;

			_mainWindow.Show();

			StartJackd(_jackdConfiguration);
		}

		void StartJackd(JackdConfiguration jackdConfig)
		{
			if (_jackd != null) {
				_jackd.StopProgram();
				_jackd.HasStarted -= Jackd_HasStarted;
				_jackd.HasExited -= Jackd_HasExited;
				_jackd.Dispose();
			}
			_jackd = new ProgramManagement(jackdConfig);
			_jackd.HasStarted += Jackd_HasStarted;
			_jackd.HasExited += Jackd_HasExited;
		}

#region Model events
		void Jackd_HasStarted(object sender, EventArgs e)
		{
			_mainWindow.Status = JackdStatusRunning;
			_mainWindow.JackdIsRunning = true;
			_mainWindow.AppsAreRunning = true;
		}

		void Jackd_HasExited(object sender, EventArgs e)
		{
			_mainWindow.Status = JackdStatusStopped;
			_mainWindow.JackdIsRunning = false;
		}
#endregion

#region IMainWindow events
		void MainWindow_StartJackd(object sender, EventArgs e)
		{
			if (_jackd.IsRunning) {
				_jackd.StopProgram();
			}
			_jackd.StartProgram();
		}

		void MainWindow_StopJackd(object sender, EventArgs e)
		{
			if (_jackd.IsRunning) {
				_jackd.StopProgram();
			}
		}

		void MainWindow_StopAll(object sender, EventArgs e)
		{
			StopAllApplications();
			foreach (AppStartWidgetController startWidgetController in _startWidgetControllers) {
				startWidgetController.StopApplication();
			}
		}

		void StopAllApplications()
		{
			if (_jackd.IsRunning) {
				_jackd.StopProgram();
			}

		}

		void MainWindow_ShowConfigureJackd(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		void MainWindow_ShowConfigureApps(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		void MainWindow_ShowAbout(object sender, EventArgs e)
		{
			IAboutWindow AboutWindow = new AboutWindow();
			AboutWindow.ProgramName = "MonoMultiJack";
			AboutWindow.Version = "0.1";
			AboutWindow.Copyright = "(c) Thomas Mayer 2012";
			AboutWindow.Comments = @"MonoMultiJack is a simple tool for controlling Jackd and diverse audio 
	programs.";
			AboutWindow.Website = "http://ix.residuum.org/";
			AboutWindow.Authors = new String[] {"Thomas Mayer"};
			AboutWindow.License = @"Copyright (c) 2009-2012 Thomas Mayer
	
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
			AboutWindow.Show();
			AboutWindow.Closing +=AboutWindow_HasBeenClosed;
		}

		void MainWindow_ShowHelp(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		void MainWindow_QuitApplication(object sender, EventArgs e)
		{
			WindowConfiguration newWindowConfig = _mainWindow.WindowConfiguration;
			PersistantConfiguration.SaveWindowSize(newWindowConfig);
			StopAllApplications();
			Application.Quit();		
		}
#endregion

		void AboutWindow_HasBeenClosed(object sender, EventArgs e)
		{
			IAboutWindow aboutWindow = sender as IAboutWindow;
			if (aboutWindow == null) return;

			aboutWindow.Destroy();
			aboutWindow.Dispose();
		}

		void AppStartController_StatusHasChanged(object sender, EventArgs e)
		{
			if (_jackd.IsRunning) {
				_mainWindow.AppsAreRunning = true;
				return;
			}
			foreach(AppStartWidgetController startWidgetController in _startWidgetControllers){
				if (startWidgetController.IsRunning){
					_mainWindow.AppsAreRunning = true;
					return;
				}
			}
			_mainWindow.AppsAreRunning = false;

		}
	}
}