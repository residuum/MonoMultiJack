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
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
		protected XmlConfiguration _config;
		
		/// <summary>
		/// Jackd startup command
		/// </summary>
		protected string _jackdStartup;
		
		/// <summary>
		/// Table for appWidgets
		/// </summary>
		protected VButtonBox _appButtonBox;
		
		/// <summary>
		/// Area for Jackd Connectors
		/// </summary>
		private Fixed _connectorArea;
		
		private JackConnectionsWidget _clientsOutput;
		
		private JackConnectionsWidget _clientsInput;
		
		/// <summary>
		/// statusbar
		/// </summary>
		private Statusbar _statusbar;
		
		/// <summary>
		/// Jackd status messages
		/// </summary>
		private const string JackdStatusRunning = "Jackd is running.";
		private const string JackdStatusStopped = "Jackd is stopped.";
		
		/// <summary>
		/// jackd process
		/// </summary>
		protected Process _jackd;
		
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
			Icon = new Pixbuf("monomultijack.png");
			HBox NewHBox = new HBox (false, 2);
			mainVbox.Add (NewHBox);
	        _appButtonBox = new VButtonBox();
			_appButtonBox.Spacing = 2;
	        _appButtonBox.Name = "appTable";
			NewHBox.Add (_appButtonBox);
			ReadConfiguration ();
			MakeConnectorArea ();
			NewHBox.Add (_connectorArea);
			_appButtonBox.ShowAll();
			_statusbar = new Statusbar();
			mainVbox.PackEnd(_statusbar,false, false, 0);
			_statusbar.ShowAll();
			_statusbar.Push(0, JackdStatusStopped);
		}
		
		/// <summary>
		/// Creates connector area
		/// </summary>
		/// <returns>
		/// A <see cref="Fixed"/>
		/// </returns>
		private void MakeConnectorArea ()
		{
			_connectorArea = new Fixed();
			_clientsOutput = new JackConnectionsWidget(ConnectionType.Outlet);
			_connectorArea.Put(_clientsOutput, 0, 0);
			_clientsInput = new JackConnectionsWidget(ConnectionType.Inlet);
			_connectorArea.Put(_clientsInput, 200, 0);
		}
		
		/// <summary>
		/// read configuration
		/// </summary>
		protected void ReadConfiguration ()
		{
			try
			{
				_config = new XmlConfiguration();
				UpdateAppWidgets(_config.appConfigs);
				UpdateJackd(_config.jackdConfig);
			}
			catch (System.Xml.XmlException)
			{
				InfoMessage("Configuration file is not readable, does not exist or is corrupt.");
				_config = new XmlConfiguration(new JackdConfiguration(), new List<AppConfiguration> ());
			}
		}
		
		/// <summary>
		/// tests for constructed appWidget with appConfig
		/// </summary>
		/// <param name="appConfig">
		/// A <see cref="AppConfiguration"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		protected bool InAppWidgets (AppConfiguration appConfig)
		{
			bool status = false;
			if (_appButtonBox.Children != null)
			{
				foreach (Widget app in _appButtonBox.Children)
				{
					if (app is AppWidget)
					{
						if ( ((AppWidget)app).appCommand.Equals(appConfig.Command))
						{
							status = true;
							break;
						}
					}
				}
			}
			return status;
		}
		
		/// <summary>
		/// Adds appWidget from appConfig to appTable
		/// </summary>
		/// <param name="appConfig">
		/// A <see cref="AppConfiguration"/>
		/// </param>
		protected void AddAppWidget (AppConfiguration appConfig)
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
		protected void UpdateAppWidgets (List<AppConfiguration> appConfigs)
		{
			foreach (AppConfiguration app in appConfigs)
			{
				if (!InAppWidgets (app))
				{
					AddAppWidget (app);
				}
			}
			//TODO: Delete old AppWidgets, Reorder AppWidgets
		}
		
		/// <summary>
		/// Updates Jackd startup commando
		/// </summary>
		/// <param name="jackdConfig">
		/// A <see cref="JackdConfiguration"/>
		/// </param>
		protected void UpdateJackd (JackdConfiguration jackdConfig)
		{
			if (jackdConfig != null)
			{
				_jackdStartup = jackdConfig.Path 
					+ " -d " + jackdConfig.Driver 
					+ " -r " + jackdConfig.Audiorate;
				reStartJackdAction.Sensitive = true;
			}
		}
		
		/// <summary>
		/// Quits window
		/// </summary>
		protected void QuitIt()
		{
			StopAll();
			Application.Quit ();
		}
		
		/// <summary>
		/// Starts or restart jackd process
		/// </summary>
		protected void RestartJackd()
		{
			if (_jackd != null && !_jackd.HasExited)
			{
				_jackd.CloseMainWindow ();
			}
			_jackd = new Process ();
			_jackd.StartInfo.FileName = _jackdStartup;
			if (_jackd.Start ())
			{
				//int jackdTest = JackdInterop.jack_client_name_size();
				_jackd.EnableRaisingEvents = true;
				_jackd.Exited += JackdExited;
				stopJackdAction.Sensitive = true;
				stopAllAction.Sensitive = true;
				_statusbar.Push(0, JackdStatusRunning);
			}
		}
		
		/// <summary>
		/// Cleans up after jackd process has stopped
		/// </summary>
		protected void CleanUpJackd()
		{
			stopJackdAction.Sensitive = false;
			_statusbar.Push(0, JackdStatusStopped);
		}

		/// <summary>
		/// stops jackd
		/// </summary>
		protected void StopJackd ()
		{
			if (_jackd != null || !_jackd.HasExited) {
				_jackd.CloseMainWindow ();
			}
			CleanUpJackd ();
		}
		
		protected void StopAll()
		{
			if (_appButtonBox.Children != null)
			{
				foreach (Widget app in _appButtonBox.Children)
				{
					if (app is AppWidget && ((AppWidget)app).IsRunning)
					{
						((AppWidget)app).StopApplication();
					}
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
		protected void InfoMessage(string message)
		{
			MessageDialog popup = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, message);
			popup.Run();
			popup.Destroy();
		}
		
		public void AppStarted()
		{
			stopAllAction.Sensitive = true;
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
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected virtual void RestartJackd (object sender, System.EventArgs e)
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
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected virtual void StopJackd (object sender, System.EventArgs e)
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
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected virtual void StopAll (object sender, System.EventArgs e)
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
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected virtual void ConfigureJackd (object sender, System.EventArgs e)
		{
			StopAll();
			JackdConfigWindow jackdConfigWindow = new JackdConfigWindow (_config.jackdConfig);
			Sensitive = false;
			jackdConfigWindow.ShowAll ();
			ResponseType response = (ResponseType)jackdConfigWindow.Run ();
			if (response == ResponseType.Ok)
			{
				JackdConfiguration jackdConfig = jackdConfigWindow.JackdConfig;
				if (!_config.UpdateConfiguration(jackdConfig))
				{
					InfoMessage("Configuration file is not writable.");
				}
				else
				{
					UpdateJackd (_config.jackdConfig);
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
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected virtual void ConfigureApplications (object sender, System.EventArgs e)
		{
			AppConfigWindow appConfigWindow = new AppConfigWindow (_config.appConfigs);
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
					//UpdateAppWidgets (config.appConfigs);
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
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
		{
			QuitIt ();
		}
		
		/// <summary>
		/// Event handler for exit of jackd process
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected virtual void JackdExited (object sender, System.EventArgs args)
		{
			CleanUpJackd();
		}
	
		/// <summary>
		/// shows about dialog
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected virtual void AboutDialog (object sender, System.EventArgs e)
		{
			AboutDialog about = new AboutDialog();
			about.ProgramName = "MonoMultiJack";
			about.Version = "0.1";
			about.Copyright = "(c) Thomas Mayer 2009";
			about.Comments = @"MonoMultiJack is a simple tool for controlling Jackd and diverse audio 
	programs.";
	        about.Website = "http://ix.residuum.org/";
			about.Authors = new String[] {"Thomas Mayer"};
			about.License = @"Copyright (c) 2009 Thomas Mayer
	
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
			about.Logo = new Gdk.Pixbuf("monomultijack.png");
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