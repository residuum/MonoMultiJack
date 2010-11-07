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

using Gdk;
using Gtk;
using MonoMultiJack;
using MonoMultiJack.Common;
using MonoMultiJack.Configuration;
using MonoMultiJack.Widgets;
using MonoMultiJack.ConnectionWrapper;
using MonoMultiJack.ConnectionWrapper.Jack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MessageType = Gtk.MessageType;

namespace MonoMultiJack
{
	/// <summary>
	/// Main Window 
	/// </summary>
	public partial class MainWindow: Gtk.Window
	{		
		/// <summary>
		/// Xml Configuration
		/// </summary>
		private PersistantConfiguration _config;
		
		/// <summary>
		/// Instance for managing Jackd
		/// </summary>
		private ProgramManagement _jackd;
		
		/// <summary>
		/// Jackd status messages
		/// </summary>
		private readonly string JackdStatusRunning = "Jackd is running.";
		private readonly string JackdStatusStopped = "Jackd is stopped.";
		
		/// <summary>
		/// Path to icon file
		/// </summary>
		private readonly string IconFile = "monomultijack.png";
		
		/// <summary>
		/// Icon File
		/// </summary>
		private Pixbuf _programIcon;
		
		/// <summary>
		/// public getter for <see cref="_programIcon"/>
		/// </summary>
		public Pixbuf ProgramIcon
		{
			get
			{
				if (_programIcon == null)
				{
					Assembly executable = Assembly.GetEntryAssembly();
					string baseDir = System.IO.Path.GetDirectoryName(executable.Location);
					string iconPath = System.IO.Path.Combine(baseDir, IconFile);
					if (File.Exists(iconPath))
					{
						_programIcon = new Pixbuf(iconPath);
					}
				}
				return _programIcon;
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build ();	
			BuildWindowContent ();
			DeleteEvent += OnDelete;
		}
		
		/// <summary>
		/// builds window content
		/// </summary>
		private void BuildWindowContent ()
		{
			Title = "MonoMultiJack";
			Icon = ProgramIcon;
			_statusbar.Push (0, JackdStatusStopped);
			ReadConfiguration ();
			IConnectionManager jackAudio = new JackAudioManager ();
			IConnectionManager jackMidi = new JackMidiManager();
			_connectionNotebook.AppendPage (new ConnectionDisplay (jackAudio), new Label (jackAudio.ConnectionType.ToString ()));
			_connectionNotebook.AppendPage (new ConnectionDisplay (jackMidi), new Label (jackMidi.ConnectionType.ToString ()));
		}
		
		/// <summary>
		/// read configuration
		/// </summary>
		private void ReadConfiguration ()
		{
			try
			{
				_config = new PersistantConfiguration();
				UpdateAppWidgets(_config.AppConfigs);
				UpdateJackd(_config.JackdConfig);
			}
			catch (System.Xml.XmlException)
			{
				InfoMessage("Configuration file is not readable, does not exist or is corrupt.");
				_config = new PersistantConfiguration(new JackdConfiguration(), new List<AppConfiguration> ());
			}
		}
		
		/// <summary>
		/// Adds appWidget from appConfig to appTable
		/// </summary>
		/// <param name="appConfig">
		/// A <see cref="AppConfiguration"/>
		/// </param>
		private void AddAppWidget (AppConfiguration appConfig)
		{
			AppWidget newApp = new AppWidget(appConfig);
			_appButtonBox.Add(newApp);
			newApp.Show();
		}
		
		/// <summary>
		/// Updates appWidgets 
		/// </summary>
		/// <param name="appConfigs">
		/// A <see cref="List"/> of <see cref="appConfiguration"/>s
		/// </param>
		private void UpdateAppWidgets (List<AppConfiguration> appConfigs)
		{
			foreach (Widget widget in _appButtonBox.Children)
			{
				AppWidget appWidget = widget as AppWidget;
				if (appWidget != null)
				{
					appWidget.Destroy();
				}
			}
			foreach (AppConfiguration appConfig in appConfigs)
			{
				AddAppWidget (appConfig);
			}
			_appButtonBox.ShowAll();
		}
		
		/// <summary>
		/// Updates Jackd startup commando
		/// </summary>
		/// <param name="jackdConfig">
		/// A <see cref="JackdConfiguration"/>
		/// </param>
		private void UpdateJackd (JackdConfiguration jackdConfig)
		{	
			if (_jackd != null && _jackd.IsRunning)
			{
				_jackd.StopProgram();
			}
			_jackd = new ProgramManagement(jackdConfig.Path, "-d " + jackdConfig.Driver + " -r "+jackdConfig.Audiorate, true);
			_jackd.HasStarted += OnJackdHasStarted;
			_jackd.HasExited += OnJackdHasExited;
			reStartJackdAction.Sensitive = true;
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
			CleanUpJackd();	
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
		private void QuitIt()
		{
			StopAll();
			Application.Quit ();
		}
		
		/// <summary>
		/// Starts or restart jackd process
		/// </summary>
		private void RestartJackd()
		{
			StopJackd();
			_jackd.StartProgram();
		}
		
		/// <summary>
		/// Cleans up after jackd process has stopped
		/// </summary>
		private void CleanUpJackd()
		{
			stopJackdAction.Sensitive = false;
			_statusbar.Push(0, JackdStatusStopped);
		}

		/// <summary>
		/// stops jackd
		/// </summary>
		private void StopJackd ()
		{
			if (_jackd != null && _jackd.IsRunning)
			{
				_jackd.StopProgram();
			}
		}
		
		/// <summary>
		/// stops Jackd and all running applications
		/// </summary>
		private void StopAll()
		{
			if (_appButtonBox.Children != null)
			{
				foreach (Widget child in _appButtonBox.Children)
				{
					AppWidget app = child as AppWidget;
					app.StopApplication();
				}
			}
			StopJackd();
			stopAllAction.Sensitive = false;
		}
		
		/// <summary>
		/// Shows an popup window with info message
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>, the message to show in the popup
		/// </param>
		private void InfoMessage(string message)
		{
			MessageDialog popup = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, message);
			popup.Run();
			popup.Destroy();
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
			RestartJackd();
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
			StopAll();
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
			JackdConfigWindow jackdConfigWindow = new JackdConfigWindow (_config.JackdConfig);
			Sensitive = false;
			jackdConfigWindow.ShowAll ();
			ResponseType response = (ResponseType)jackdConfigWindow.Run ();
			if (response == ResponseType.Ok)
			{
				StopAll();			
				JackdConfiguration jackdConfig = jackdConfigWindow.JackdConfig;
				if (!_config.UpdateConfiguration(jackdConfig))
				{
					InfoMessage("Configuration file is not writable.");
				}
				else
				{
					UpdateJackd (_config.JackdConfig);
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
			AppConfigWindow appConfigWindow = new AppConfigWindow (_config.AppConfigs);
			Sensitive = false;		
			appConfigWindow.ShowAll ();
			ResponseType response = (ResponseType)appConfigWindow.Run ();
			if (response == ResponseType.Ok)
			{
				StopAll();
				List<AppConfiguration> newAppConfigs = appConfigWindow.AppConfigs;
				newAppConfigs.Reverse ();
				if (!_config.UpdateConfiguration(newAppConfigs))
				{
					InfoMessage("Configuration file is not writable.");
				}
				else
				{
					UpdateAppWidgets (_config.AppConfigs);
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
			AboutDialog about = new AboutDialog();
			about.ProgramName = "MonoMultiJack";
			about.Version = "0.0.1";
			about.Copyright = "(c) Thomas Mayer 2010";
			about.Comments = @"MonoMultiJack is a simple tool for controlling Jackd and diverse audio 
	programs.";
	        about.Website = "http://ix.residuum.org/";
			about.Authors = new String[] {"Thomas Mayer"};
			about.License = @"Copyright (c) 2010 Thomas Mayer
	
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
			about.Run();
			about.Destroy();
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
		public void OnDelete(object o, EventArgs args)
		{
			QuitIt();
		}
	}
}