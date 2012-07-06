// 
// AppWidget.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2012 Thomas Mayer
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
using System.Linq;
using Gtk;
using MonoMultiJack.BusinessLogic.Common;
using MonoMultiJack.BusinessLogic.Configuration;

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
		
		/// <summary>
		/// Manages the running program.
		/// </summary>
		private ProgramManagement _appInstance;
		
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="appConfig">
		/// A <see cref="AppConfiguration"/>
		/// </param>
		public AppWidget(AppConfiguration appConfig)
		{
			_startButton = new ToggleButton();
			_startButton.Label = appConfig.Name;
			Name = appConfig.Name;
			_startButton.Name = appConfig.Command;
			_startButton.WidthRequest = 100;
			_startButton.Clicked += StartApplication;
			Put(_startButton, 0, 0);
			if (!string.IsNullOrEmpty(appConfig.Command)) {
				string[] appConfigValues = appConfig.Command.Split(new char[] { ' ' }, 2);
				_appInstance = new ProgramManagement(
		    appConfigValues [0], 
		    appConfigValues.Count() > 1 ? appConfigValues [1] : string.Empty
				);
				_appInstance.HasStarted += OnAppInstanceHasStarted;
				_appInstance.HasExited += OnAppInstanceHasExited;
			}
		
		}

		/// <summary>
		/// Handles the Edited event of the application.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		void OnAppInstanceHasExited(object sender, EventArgs e)
		{
			ResetWidget();			
		}

		/// <summary>
		/// Handles the start event of the application.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// </param>
		void OnAppInstanceHasStarted(object sender, EventArgs e)
		{
			_startButton.Active = true;
			_startButton.Clicked -= StopApplication;
			_startButton.Clicked -= StartApplication;
			_startButton.Clicked += StopApplication;			
		}
		
		/// <summary>
		/// stops application, if running
		/// </summary>
		public void StopApplication()
		{
			_appInstance.StopProgram();
		}
		
		/// <summary>
		/// starts application, updates action for togglebutton
		/// </summary>
		private void StartApplication()
		{
			_appInstance.StartProgram();
		}
		
		/// <summary>		
		/// resets ToggleButton state and clears appProcess
		/// </summary>
		private void ResetWidget()
		{
			_startButton.Active = false;
			_startButton.Clicked -= StopApplication;
			_startButton.Clicked -= StartApplication;
			_startButton.Clicked += StartApplication;
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
		private void StartApplication(object obj, EventArgs args)
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
		private void StopApplication(object obj, EventArgs args)
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
		private void ResetWidget(object obj, EventArgs args)
		{
			ResetWidget();
		}
	}
}