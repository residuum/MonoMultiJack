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
using System.Diagnostics;
using System.Collections.Generic;
using Gtk;
using MonoMultiJack;

/// <summary>
/// Main Window 
/// </summary>
public partial class MainWindow: Gtk.Window
{		
	/// <summary>
	/// Xml Configuration
	/// </summary>
	protected XmlConfiguration config;
	
	/// <summary>
	/// Jackd startup command
	/// </summary>
	protected string jackdStartup;
	
	/// <summary>
	/// Count of elements in appTable
	/// </summary>
	protected uint tableCounter;
	
	/// <summary>
	/// Rows in appTable
	/// </summary>
	protected uint tableRows = 10;
	
	/// <summary>
	/// Table for appWidgets
	/// </summary>
	protected Gtk.Table appTable;
	
	private Gtk.Fixed _ConnectorArea;
	
	/// <summary>
	/// jackd process
	/// </summary>
	protected Process jackd;
	
	/// <summary>
	/// Constructor
	/// </summary>
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		this.tableCounter = 0;
		this.Build ();
		this.BuildWindowContent ();
	}
	
	private void BuildWindowContent ()
	{
		this.Title = "MonoMultiJack";
		this.Icon = new Gdk.Pixbuf("monomultijack.png");
		Gtk.HBox NewHBox = new Gtk.HBox (false, 0);
		this.mainVbox.Add (NewHBox);
        this.appTable = new Gtk.Table((this.tableRows), ((uint)(2)), false);
        this.appTable.Name = "appTable";
        this.appTable.RowSpacing = ((uint)(2));
        this.appTable.ColumnSpacing = ((uint)(10));
		NewHBox.Add (this.appTable);
		this.ReadConfiguration ();
		this._ConnectorArea = MakeConnectorArea ();
		NewHBox.Add (_ConnectorArea);
		this.appTable.ShowAll();
	}
	
	private Fixed MakeConnectorArea ()
	{
		Gtk.Fixed Area = new Gtk.Fixed ();
		return Area;
	}
	
	/// <summary>
	/// read configuration
	/// </summary>
	protected void ReadConfiguration ()
	{
		this.config = new XmlConfiguration();
		if (this.config.jackdConfig == null && this.config.appConfigs.Count == 0)
		{
			this.InfoMessage("Configuration file is not readable, does not exist or is corrupt.");
		}
		this.UpdateAppWidgets(this.config.appConfigs);
		this.UpdateJackd(this.config.jackdConfig);
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
		if (this.appTable.Children != null)
		{
			foreach (Gtk.Widget app in this.appTable.Children)
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
		if (this.tableCounter < this.tableRows)
		{
			this.appTable.Attach(newApp, 0, 1, this.tableCounter, this.tableCounter + 1);
		}
		else
		{
			this.appTable.Attach(newApp, 1, 2, this.tableCounter - this.tableRows, this.tableCounter - this.tableRows + 1);
		}
		this.tableCounter++;
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
			this.jackdStartup = jackdConfig.path 
				+ " -d " + jackdConfig.driver 
				+ " -r " + jackdConfig.audiorate;
			this.ReStartJackdAction.Sensitive = true;
		}
	}
	
	/// <summary>
	/// Quits window
	/// </summary>
	protected void QuitIt()
	{
		if (this.jackd != null && this.jackd.HasExited == false)
		{
			try
			{
				this.jackd.Kill();
			}
			catch (Exception ex)
			{
				this.InfoMessage(ex.Message);
			}
		}
		foreach (Widget appPart in this.appTable.Children)
		{
			if (appPart is AppWidget)
			{
				((AppWidget)appPart).stopApplication();
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
	protected virtual void restartJackd (object sender, System.EventArgs e)
	{
		if (this.jackd != null && this.jackd.HasExited == false)
		{
			this.jackd.CloseMainWindow ();
		}
		this.jackd = new Process ();
		this.jackd.StartInfo.FileName = jackdStartup;
		if (this.jackd.Start ())
		{
			this.jackd.EnableRaisingEvents = true;
			jackd.Exited += jackdExited;
			StopJackdAction.Sensitive = true;
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
	protected virtual void stopJackd (object sender, System.EventArgs e)
	{
		if (this.jackd != null || this.jackd.HasExited == false)
		{
			this.jackd.CloseMainWindow ();
		}	
		StopJackdAction.Sensitive = false;
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
	protected virtual void stopAll (object sender, System.EventArgs e)
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
		JackdConfigWindow jackdConfigWindow = new JackdConfigWindow (this.config.jackdConfig);
		this.Sensitive = false;
		jackdConfigWindow.ShowAll ();
		ResponseType response = (ResponseType)jackdConfigWindow.Run ();
		if (response == ResponseType.Ok)
		{
			JackdConfiguration jackdConfig = jackdConfigWindow.jackdConfig;
			if (!this.config.updateConfiguration(jackdConfig))
			{
				this.InfoMessage("Configuration file is not writable.");
			}
			else
			{
				this.UpdateJackd (this.config.jackdConfig);
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
		AppConfigWindow appConfigWindow = new AppConfigWindow (this.config.appConfigs);
		this.Sensitive = false;		
		appConfigWindow.ShowAll ();
		ResponseType response = (ResponseType)appConfigWindow.Run ();
		if (response == ResponseType.Ok)
		{
			//TODO: stop all
			List<AppConfiguration> newAppConfigs = appConfigWindow.appConfigs;
			newAppConfigs.Reverse ();
			if (!this.config.updateConfiguration(newAppConfigs))
			{
				this.InfoMessage("Configuration file is not writable.");
			}
			else
			{
			//this.UpdateAppWidgets (this.config.appConfigs);
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
	
	protected virtual void jackdExited (object sender, System.EventArgs args)
	{
		this.StopJackdAction.Sensitive = false;
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
}