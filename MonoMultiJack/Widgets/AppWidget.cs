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
using MonoMultiJack.Configuration;

namespace MonoMultiJack.Widgets
{	
	/// <summary>
	/// Widget for applications to start / stop
	/// </summary>
	public class AppWidget : Gtk.Fixed
	{
		//// <value>
		/// toggle Button for starting / stopping
		/// </value>
		private Gtk.ToggleButton _startButton;
		
		//// <value>
		/// the application process
		/// </value>
		private Process _appProcess;
		
		//// <value>
		/// the command to start application
		/// </value>
		public string appCommand {get; protected set;}
		
		
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="appConfig">
		/// A <see cref="AppConfiguration"/>
		/// </param>
		public AppWidget (AppConfiguration appConfig)
		{
			this.appCommand = appConfig.command;
			
			this._startButton = new Gtk.ToggleButton ();
			this._startButton.Label = appConfig.name;
			this.Name = appConfig.name;
			this._startButton.Name = appCommand;
			this._startButton.WidthRequest = 100;
			this._startButton.Clicked += StartApplication;
			this.Put (_startButton, 0, 0);
		}
		
		/// <summary>
		/// stops application, if running
		/// </summary>
		public void StopApplication ()
		{
			if (this._appProcess != null && !this._appProcess.HasExited)
			{
				this._appProcess.CloseMainWindow ();
			}				
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
			if (this._appProcess == null || this._appProcess.HasExited)
			{
				this._appProcess = new Process ();
				this._appProcess.StartInfo.FileName = ((Gtk.ToggleButton)obj).Name;
				if (_appProcess.Start ())
				{
					this._appProcess.EnableRaisingEvents = true;
					this._appProcess.Exited += ResetButton;
					((Gtk.ToggleButton)obj).Clicked -= StartApplication;
					((Gtk.ToggleButton)obj).Clicked += StopApplication;
				}
			}
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
			if (!this._appProcess.HasExited)
			{
				this._appProcess.CloseMainWindow ();
			}
			((Gtk.ToggleButton)obj).Clicked -= StopApplication;
			((Gtk.ToggleButton)obj).Clicked += StartApplication;
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
		private void ResetButton (object obj, EventArgs args)
		{
			this._startButton.Active = false;
			this._startButton.Clicked -= StopApplication;
			this._startButton.Clicked += StartApplication;
		}
	}
}
