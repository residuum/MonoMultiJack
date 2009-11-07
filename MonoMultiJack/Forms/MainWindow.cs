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

using Gtk;
using Gdk;
using MonoMultiJack;
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
		/// Count of elements in appTable
		/// </summary>
		protected uint _tableCounter;
		
		/// <summary>
		/// Rows in appTable
		/// </summary>
		protected uint _tableRows = 10;
		
		/// <summary>
		/// Table for appWidgets
		/// </summary>
		protected Table _appTable;
		
		/// <summary>
		/// Area for Jackd Connectors
		/// </summary>
		private Fixed _ConnectorArea;
		
		/// <summary>
		/// statusbar
		/// </summary>
		private Statusbar _statusbar;
		
		/// <summary>
		/// Jackd status messages
		/// </summary>
		private string _jackdStatusRunning = "Jackd is running.";
		private string _jackdStatusStopped = "Jackd is stopped.";
		
		/// <summary>
		/// jackd process
		/// </summary>
		protected Process _jackd;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			this._tableCounter = 0;
			this.Build ();	
			this.BuildWindowContent ();
			this.DeleteEvent += OnDelete;
		}
		
		/// <summary>
		/// builds window content
		/// </summary>
		private void BuildWindowContent ()
		{
			this.Title = "MonoMultiJack";
			this.Icon = new Pixbuf("monomultijack.png");
			HBox NewHBox = new HBox (false, 0);
			this.mainVbox.Add (NewHBox);
	        this._appTable = new Table((this._tableRows), ((uint)(2)), false);
	        this._appTable.Name = "appTable";
	        this._appTable.RowSpacing = ((uint)(2));
	        this._appTable.ColumnSpacing = ((uint)(10));
			NewHBox.Add (this._appTable);
			this.ReadConfiguration ();
			this._ConnectorArea = MakeConnectorArea ();
			NewHBox.Add (_ConnectorArea);
			this._appTable.ShowAll();
			this._statusbar = new Statusbar();
			this.mainVbox.PackEnd(this._statusbar,false, false, 0);
			this._statusbar.ShowAll();
			this._statusbar.Push(0, this._jackdStatusStopped);
		}
		
		private Fixed MakeConnectorArea ()
		{
			Gtk.Fixed Area = new Fixed ();
			return Area;
		}
		
		/// <summary>
		/// read configuration
		/// </summary>
		protected void ReadConfiguration ()
		{
			this._config = new XmlConfiguration();
			if (this._config.jackdConfig == null && this._config.appConfigs.Count == 0)
			{
				this.InfoMessage("Configuration file is not readable, does not exist or is corrupt.");
			}
			this.UpdateAppWidgets(this._config.appConfigs);
			this.UpdateJackd(this._config.jackdConfig);
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
			if (this._appTable.Children != null)
			{
				foreach (Gtk.Widget app in this._appTable.Children)
				{
					if (app is AppWidget)
					{
						if ( ((AppWidget)app).appCommand.Equals(appConfig.command))
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
			if (this._tableCounter < this._tableRows)
			{
				this._appTable.Attach(newApp, 0, 1, this._tableCounter, this._tableCounter + 1);
			}
			else
			{
				this._appTable.Attach(newApp, 1, 2, this._tableCounter - this._tableRows, this._tableCounter - this._tableRows + 1);
			}
			this._tableCounter++;
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
				if (!this.InAppWidgets (app))
				{
					this.AddAppWidget (app);
				}
			}
			//TODO: Delete old AppWidgets
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
				this._jackdStartup = jackdConfig.path 
					+ " -d " + jackdConfig.driver 
					+ " -r " + jackdConfig.audiorate;
				this.reStartJackdAction.Sensitive = true;
			}
		}
		
		/// <summary>
		/// Quits window
		/// </summary>
		protected void QuitIt()
		{
			if (this._jackd != null && !this._jackd.HasExited)
			{
				try
				{
					this._jackd.Kill();
				}
				catch (Exception ex)
				{
					this.InfoMessage(ex.Message);
				}
			}
			foreach (Widget appPart in this._appTable.Children)
			{
				if (appPart is AppWidget)
				{
					((AppWidget)appPart).StopApplication();
				}	
			}
			Application.Quit ();
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
			this.QuitIt ();
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
			if (this._jackd != null && !this._jackd.HasExited)
			{
				this._jackd.CloseMainWindow ();
			}
			this._jackd = new Process ();
			this._jackd.StartInfo.FileName = _jackdStartup;
			if (this._jackd.Start ())
			{
				this._jackd.EnableRaisingEvents = true;
				_jackd.Exited += JackdExited;
				this.stopJackdAction.Sensitive = true;
				this._statusbar.Push(0, this._jackdStatusRunning);
			}
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
			if (this._jackd != null || !this._jackd.HasExited)
			{
				this._jackd.CloseMainWindow ();
			}	
			this.stopJackdAction.Sensitive = false;
			this._statusbar.Push(0, this._jackdStatusStopped);
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
			//TODO
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
			//TODO: stopall
			JackdConfigWindow jackdConfigWindow = new JackdConfigWindow (this._config.jackdConfig);
			this.Sensitive = false;
			jackdConfigWindow.ShowAll ();
			ResponseType response = (ResponseType)jackdConfigWindow.Run ();
			if (response == ResponseType.Ok)
			{
				JackdConfiguration jackdConfig = jackdConfigWindow.jackdConfig;
				if (!this._config.UpdateConfiguration(jackdConfig))
				{
					this.InfoMessage("Configuration file is not writable.");
				}
				else
				{
					this.UpdateJackd (this._config.jackdConfig);
				}
			}
			jackdConfigWindow.Destroy ();
			this.Sensitive = true;
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
			AppConfigWindow appConfigWindow = new AppConfigWindow (this._config.appConfigs);
			this.Sensitive = false;		
			appConfigWindow.ShowAll ();
			ResponseType response = (ResponseType)appConfigWindow.Run ();
			if (response == ResponseType.Ok)
			{
				//TODO: stop all
				List<AppConfiguration> newAppConfigs = appConfigWindow.appConfigs;
				newAppConfigs.Reverse ();
				if (!this._config.UpdateConfiguration(newAppConfigs))
				{
					this.InfoMessage("Configuration file is not writable.");
				}
				else
				{
				//this._UpdateAppWidgets (this.config.appConfigs);
				}
			}
			appConfigWindow.Destroy ();
			this.Sensitive = true;
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
			this.QuitIt ();
		}
		
		protected virtual void JackdExited (object sender, System.EventArgs args)
		{
			this.stopJackdAction.Sensitive = false;
			this._statusbar.Push(0, this._jackdStatusStopped);
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
		
		protected void InfoMessage(string message)
		{
			MessageDialog popup = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, message);
			popup.Run();
			popup.Destroy();
		}
		
		public void OnDelete(object o, EventArgs args)
		{
			this.QuitIt();
		}
	}
}