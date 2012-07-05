// 
// MainWindow.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009 Thomas Mayer
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
using System.Reflection;
using Gdk;
using Gtk;
using MonoMultiJack;
using MonoMultiJack.BusinessLogic.Common;
using MonoMultiJack.BusinessLogic.Configuration;
using MonoMultiJack.ConnectionWrapper;
using MonoMultiJack.ConnectionWrapper.Alsa;
using MonoMultiJack.ConnectionWrapper.Jack;
using MonoMultiJack.Widgets;
using MessageType = Gtk.MessageType;

namespace MonoMultiJack.Forms
{
	/// <summary>
	/// Main Window 
	/// </summary>
	public partial class MainWindow: Gtk.Window
	{		
		JackdConfiguration _jackdConfig;
		List<AppConfiguration> _appConfigs;

		/// <summary>
		/// Instance for managing Jackd
		/// </summary>
		ProgramManagement _jackd;
		
		/// <summary>
		/// Jackd status messages
		/// </summary>
		readonly string JackdStatusRunning = "Jackd is running.";
		readonly string JackdStatusStopped = "Jackd is stopped.";
		
		/// <summary>
		/// Path to icon file
		/// </summary>
		readonly string IconFile = "monomultijack.png";
		
		/// <summary>
		/// Icon File
		/// </summary>
		Pixbuf _programIcon;
		
		/// <summary>
		/// public getter for <see cref="_programIcon"/>
		/// </summary>
		public Pixbuf ProgramIcon {
			get {
				if (_programIcon == null) {
					Assembly executable = Assembly.GetEntryAssembly ();
					string baseDir = System.IO.Path.GetDirectoryName (executable.Location);
					string iconPath = System.IO.Path.Combine (baseDir, IconFile);
					if (File.Exists (iconPath)) {
						_programIcon = new Pixbuf (iconPath);
					}
				}
				return _programIcon;
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindow () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			BuildWindowContent ();
			DeleteEvent += OnDelete;
		}
		
		/// <summary>
		/// builds window content
		/// </summary>
		void BuildWindowContent ()
		{
			Title = "MonoMultiJack";
			Icon = ProgramIcon;
			_statusbar.Push (0, JackdStatusStopped);
			ReadConfiguration ();
			foreach (ConnectionType connType in Enum.GetValues(typeof(ConnectionType))) {
				IConnectionManager connManager = ConnectionManagerFactory.GetConnectionManager (connType);
				if (connManager != null) {					
					_connectionNotebook.AppendPage (
			new ConnectionDisplay (connManager),
			new Label (connManager.ConnectionType.ToString ())
					);
				}
			}
		}
		
		/// <summary>
		/// read configuration
		/// </summary>
		void ReadConfiguration ()
		{
			try {
				_jackdConfig = PersistantConfiguration.LoadJackdConfiguration ();
				
				UpdateJackd (_jackdConfig);
				
			} catch (System.Xml.XmlException e) {
				#if DEBUG
		Console.WriteLine (e.Message);
				#endif
				InfoMessage ("Jackd configuration File is corrupt.");
				_jackdConfig = new JackdConfiguration ();
			} catch (FileNotFoundException e) {
				#if DEBUG
		Console.WriteLine (e.Message);
				#endif
				InfoMessage ("Jackd is not configured.");
				_jackdConfig = new JackdConfiguration ();
			}

			try {
				_appConfigs = PersistantConfiguration.LoadAppConfigurations ();
				
				UpdateAppWidgets (_appConfigs);
				
			} catch (System.Xml.XmlException e) {
				#if DEBUG
		Console.WriteLine (e.Message);
				#endif
				InfoMessage ("Application configuration File is corrupt.");
				_appConfigs = new List<AppConfiguration> ();
			} catch (FileNotFoundException e) {
				#if DEBUG
		Console.WriteLine (e.Message);
				#endif
				InfoMessage ("Applications are not configured.");
				_appConfigs = new List<AppConfiguration> ();
			}

			try {
				WindowConfiguration windowSize = PersistantConfiguration.LoadWindowSize ();
				if (windowSize.XSize != 0 && windowSize.YSize != 0) {
					UpdateWindowSize (windowSize);
				}
			} catch (Exception e) {				
				#if DEBUG
		Console.WriteLine (e.Message);
				#endif
			}

		}
		
		/// <summary>
		/// Adds appWidget from appConfig to appTable
		/// </summary>
		/// <param name="appConfig">
		/// A <see cref="AppConfiguration"/>
		/// </param>
		void AddAppWidget (AppConfiguration appConfig)
		{
			AppWidget newApp = new AppWidget (appConfig);
			_appButtonBox.Add (newApp);
			newApp.Show ();
		}
		
		/// <summary>
		/// Updates appWidgets 
		/// </summary>
		/// <param name="appConfigs">
		/// A <see cref="List"/> of <see cref="appConfiguration"/>s
		/// </param>
		void UpdateAppWidgets (List<AppConfiguration> appConfigs)
		{
			foreach (Widget widget in _appButtonBox.Children) {
				AppWidget appWidget = widget as AppWidget;
				if (appWidget != null) {
					appWidget.Destroy ();
				}
			}
			foreach (AppConfiguration appConfig in appConfigs) {
				AddAppWidget (appConfig);
			}
			_appButtonBox.ShowAll ();
		}
		
		/// <summary>
		/// Updates Jackd startup commando
		/// </summary>
		/// <param name="jackdConfig">
		/// A <see cref="JackdConfiguration"/>
		/// </param>
		void UpdateJackd (JackdConfiguration jackdConfig)
		{	
			if (_jackd != null && _jackd.IsRunning) {
				_jackd.StopProgram ();
			}
			_jackd = new ProgramManagement (
		jackdConfig.Path,
		jackdConfig.GeneralOptions + " -d " + jackdConfig.Driver + " " + jackdConfig.DriverOptions,
		true
			);
			_jackd.HasStarted += OnJackdHasStarted;
			_jackd.HasExited += OnJackdHasExited;
			reStartJackdAction.Sensitive = true;
		}

		void UpdateWindowSize (WindowConfiguration windowConfig)
		{
			Resize (windowConfig.XSize, windowConfig.YSize);
			Move (windowConfig.XPosition, windowConfig.YPosition);
		}

		/// <summary>
		/// Handles the exit event of jackd.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		void OnJackdHasExited (object sender, EventArgs e)
		{
			CleanUpJackd ();	
		}

		/// <summary>
		/// Handles the start event of jackd.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		void OnJackdHasStarted (object sender, EventArgs e)
		{
			stopJackdAction.Sensitive = true;
			stopAllAction.Sensitive = true;
			_statusbar.Push (0, JackdStatusRunning);
		}
		
		/// <summary>
		/// Quits window
		/// </summary>
		void QuitIt ()
		{
			int xPosition, yPosition, xSize, ySize;
			GetPosition (out xPosition, out yPosition);
			GetSize (out xSize, out ySize);
			WindowConfiguration newWindowConfig = new WindowConfiguration (
		xPosition,
		yPosition,
		xSize,
		ySize
			);
			PersistantConfiguration.SaveWindowSize (newWindowConfig);
			StopAll ();
			Application.Quit ();
		}
		
		/// <summary>
		/// Starts or restart jackd process
		/// </summary>
		void RestartJackd ()
		{
			StopJackd ();
			_jackd.StartProgram ();
		}
		
		/// <summary>
		/// Cleans up after jackd process has stopped
		/// </summary>
		void CleanUpJackd ()
		{
			stopJackdAction.Sensitive = false;
			_statusbar.Push (0, JackdStatusStopped);
		}

		/// <summary>
		/// stops jackd
		/// </summary>
		void StopJackd ()
		{
			if (_jackd != null && _jackd.IsRunning) {
				_jackd.StopProgram ();
			}
		}
		
		/// <summary>
		/// stops Jackd and all running applications
		/// </summary>
		void StopAll ()
		{
			if (_appButtonBox.Children != null) {
				foreach (Widget child in _appButtonBox.Children) {
					AppWidget app = child as AppWidget;
					app.StopApplication ();
				}
			}
			StopJackd ();
			stopAllAction.Sensitive = false;
		}
		
		/// <summary>
		/// Shows an popup window with info message
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>, the message to show in the popup
		/// </param>
		void InfoMessage (string message)
		{
			MessageDialog popup = new MessageDialog (
		this,
		DialogFlags.DestroyWithParent,
		MessageType.Info,
		ButtonsType.Ok,
		message
			);
			popup.Run ();
			popup.Destroy ();
		}
		
		/// <summary>
		/// On deletion of window
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="DeleteEventArgs"/>
		/// </param>
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			QuitIt ();
		}
		
		/// <summary>
		/// starts or restarts Jackd
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected virtual void RestartJackd (object sender, EventArgs e)
		{
			RestartJackd ();
		}
	
		/// <summary>
		/// stops Jackd
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected virtual void StopJackd (object sender, EventArgs e)
		{
			StopJackd ();
		}
	
		/// <summary>
		/// stops all running appWidgets
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected virtual void StopAll (object sender, EventArgs e)
		{
			StopAll ();
		}
		
		/// <summary>
		/// starts JackdConfigWindow, updates config and jackdStartupPath
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected virtual void ConfigureJackd (object sender, EventArgs e)
		{
			JackdConfigWindow jackdConfigWindow = new JackdConfigWindow (_jackdConfig);
			Sensitive = false;
			jackdConfigWindow.ShowAll ();
			ResponseType response = (ResponseType)jackdConfigWindow.Run ();
			if (response == ResponseType.Ok) {
				StopAll ();			
				JackdConfiguration jackdConfig = jackdConfigWindow.JackdConfig;
				try {
					PersistantConfiguration.SaveJackdConfig (jackdConfig);
					_jackdConfig = jackdConfig;
					UpdateJackd (_jackdConfig);
				} catch (Exception ex) {
					#if DEBUG
					Console.WriteLine (ex.Message);
					#endif
					InfoMessage ("Configuration file is not writable.");
				}
			}
			jackdConfigWindow.Destroy ();
			Sensitive = true;
		}	
		
		/// <summary>
		/// starts AppConfigWindow, updates config, updates appWidgets
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected virtual void ConfigureApplications (object sender, EventArgs e)
		{
			AppConfigWindow appConfigWindow = new AppConfigWindow (_appConfigs);
			Sensitive = false;		
			appConfigWindow.ShowAll ();
			ResponseType response = (ResponseType)appConfigWindow.Run ();
			if (response == ResponseType.Ok) {
				StopAll ();
				List<AppConfiguration> newAppConfigs = appConfigWindow.AppConfigs;
				newAppConfigs.Reverse ();
				try {
					PersistantConfiguration.SaveAppConfiguations (newAppConfigs);
					_appConfigs = newAppConfigs;
					UpdateAppWidgets (_appConfigs);
				} catch (Exception ex) {
					#if DEBUG
					Console.WriteLine (ex.Message);
					#endif
					InfoMessage ("Configuration file is not writable.");
				}
			}
			appConfigWindow.Destroy ();
			Sensitive = true;
		}
	
		/// <summary>
		/// quits window
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected virtual void OnQuitActionActivated (object sender, EventArgs e)
		{
			QuitIt ();
		}
		
		/// <summary>
		/// shows about dialog
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected virtual void AboutDialog (object sender, EventArgs e)
		{
			AboutDialog about = new AboutDialog ();
			about.ProgramName = "MonoMultiJack";
			about.Version = "0.0.2";
			about.Copyright = "(c) Thomas Mayer 2011";
			about.Comments = @"MonoMultiJack is a simple tool for controlling Jackd and diverse audio 
	programs.";
			about.Website = "http://ix.residuum.org/";
			about.Authors = new String[] {"Thomas Mayer"};
			about.License = @"Copyright (c) 2011 Thomas Mayer
	
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
			about.Logo = ProgramIcon;
			about.Run ();
			about.Destroy ();
		}
		
		/// <summary>
		/// Event handler for destruction of main window
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnDelete (object o, EventArgs args)
		{
			QuitIt ();
		}
	}
}