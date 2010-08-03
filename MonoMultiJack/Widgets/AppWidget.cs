// 
// AppWidget.cs
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
using System.IO;
using MonoMultiJack.Configuration;
using MonoMultiJack;
using Gtk;
using System.Linq;

namespace MonoMultiJack.Widgets
{	
	/// <summary>
	/// Widget for applications to start / stop
	/// </summary>
	public class AppWidget : Fixed
	{
		//// <value>
		/// toggle Button for starting / stopping
		/// </value>
		private ToggleButton _startButton;
		
		//// <value>
		/// the application process
		/// </value>
		private string _appProcessPid;
		
		private string _appStartup;
		
		/// <summary>
		/// returns status of running application
		/// </summary>
		public bool IsAppRunning
		{
			get 
			{
				return !string.IsNullOrEmpty(_appProcessPid);
			}
		}
		
		//// <value>
		/// the command to start application
		/// </value>
		public string appCommand {get; private set;}
		
		
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="appConfig">
		/// A <see cref="AppConfiguration"/>
		/// </param>
		public AppWidget (AppConfiguration appConfig)
		{			
			_startButton = new ToggleButton ();
			_startButton.Label = appConfig.Name;
			Name = appConfig.Name;
			_startButton.Name = appCommand;
			_startButton.WidthRequest = 100;
			_startButton.Clicked += StartApplication;
			Put (_startButton, 0, 0);
			_appStartup = System.IO.Path.GetTempFileName();
			try
			{
				string[] appConfigValues = appConfig.Command.Split(new char[]{' '}, 2);
				
				File.WriteAllText(_appStartup, 
				                  XmlConfiguration.GetBashScript(appConfigValues[0], 
				                                                 appConfigValues.Count() > 1? appConfigValues[1] : string.Empty)
				                  );
			}
			catch (Exception ex)
			{
				new IOException("Unable to write to temporary file.", ex);
			}
		
		}
		
		/// <summary>
		/// stops application, if running
		/// </summary>
		public void StopApplication ()
		{
			if (IsAppRunning)
			{
				Process killApp = new Process();
				killApp.StartInfo.FileName = "kill";
				killApp.StartInfo.Arguments = _appProcessPid;
				if (killApp.Start())
				{
					ResetWidget ();					
				}
			}
		}
		
		/// <summary>
		/// starts application, updates action for togglebutton
		/// </summary>
		private void StartApplication()
		{			
			if (!IsAppRunning)
			{
				Process startAppBash = new Process();				
				startAppBash.StartInfo.FileName = "sh";
				startAppBash.StartInfo.Arguments = _appStartup;
				startAppBash.StartInfo.RedirectStandardOutput = true;
				startAppBash.EnableRaisingEvents = true;
				startAppBash.StartInfo.UseShellExecute = false;
				startAppBash.OutputDataReceived += HandleStartAppBashOutputDataReceived;	
				if (startAppBash.Start())
				{
					startAppBash.BeginOutputReadLine();
				}
			}
		}

		void HandleStartAppBashOutputDataReceived (object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				_appProcessPid = e.Data;
			_startButton.Clicked -= StartApplication;
			_startButton.Clicked += StopApplication;
			}
		}
		
		/// <summary>		
		/// resets ToggleButton state and clears appProcess
		/// </summary>
		private void ResetWidget ()
		{
			_startButton.Active = false;
			_startButton.Clicked -= StopApplication;
			_startButton.Clicked += StartApplication;
			_appProcessPid = null;
		}

		/// <summary>
		/// starts application, updates action for ToggleButton
		/// </summary>
		/// <param name="obj">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void StartApplication (object obj, EventArgs args)
		{
			StartApplication();
		}
		/// <summary>
		/// stops application, updates action for ToggleButton
		/// </summary>
		/// <param name="obj">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void StopApplication (object obj, EventArgs args)
		{
			StopApplication();
		}
		
		/// <summary>
		/// resets ToggleButton state
		/// </summary>
		/// <param name="obj">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void ResetWidget (object obj, EventArgs args)
		{
			ResetWidget();
		}
	}
}
