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
using System.Threading;
using System.Diagnostics;

namespace MonoMultiJack
{	
	/// <summary>
	/// Widget for applications to start / stop
	/// </summary>
	public class AppWidget : Gtk.Fixed
	{
		//// <value>
		/// toggle Button for starting / stopping
		/// </value>
		private Gtk.ToggleButton startButton;
		
		//// <value>
		/// the application process
		/// </value>
		private Process appProcess;
		
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
			
			this.startButton = new Gtk.ToggleButton ();
			this.startButton.Label = appConfig.name;
			this.Name = appConfig.name;
			this.startButton.Name = appCommand;
			this.startButton.WidthRequest = 100;
			this.startButton.Clicked += startApplication;
			this.Put (startButton, 0, 0);
		}
		
		/// <summary>
		/// stops application, if running
		/// </summary>
		public void stopApplication ()
		{
			if (this.appProcess != null && this.appProcess.HasExited == false)
			{
				this.appProcess.CloseMainWindow ();
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
		private void startApplication (object obj, EventArgs args)
		{
			if (this.appProcess == null || this.appProcess.HasExited)
			{
				this.appProcess = new Process ();
				this.appProcess.StartInfo.FileName = ((Gtk.ToggleButton)obj).Name;
				if (appProcess.Start ())
				{
					this.appProcess.EnableRaisingEvents = true;
					this.appProcess.Exited += resetButton;
					((Gtk.ToggleButton)obj).Clicked -= startApplication;
					((Gtk.ToggleButton)obj).Clicked += stopApplication;
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
		private void stopApplication (object obj, EventArgs args)
		{
			if (this.appProcess.HasExited == false)
			{
				this.appProcess.CloseMainWindow ();
			}
			((Gtk.ToggleButton)obj).Clicked -= stopApplication;
			((Gtk.ToggleButton)obj).Clicked += startApplication;
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
		private void resetButton (object obj, EventArgs args)
		{
			this.startButton.Active = false;
			this.startButton.Clicked -= stopApplication;
			this.startButton.Clicked += startApplication;
		}
	}
}
