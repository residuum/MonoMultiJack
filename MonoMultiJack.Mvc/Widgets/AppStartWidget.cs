// 
// AppStartWidget.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2014 Thomas Mayer
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
using Xwt;
using MonoMultiJack.Utilities;

namespace MonoMultiJack.Widgets
{
	/// <summary>
	/// Widget for applications to start / stop
	/// </summary>
	public class AppStartWidget : Table, IAppStartWidget
	{
		public new void Dispose ()
		{
			base.Dispose ();
		}
		#region IWidget implementation
		void IWidget.Show ()
		{
			Show ();
		}

		void IWidget.Hide ()
		{
			Hide ();
		}
		#endregion
		#region IAppWidget implementation
		void IAppStartWidget.SetApp (string name, string commandName)
		{
			_startButton.Label = name;
			Name = name;
			_startButton.Name = commandName;
		}

		bool IAppStartWidget.IsRunning {

			set {
				Application.Invoke (delegate {
					if (value) {
						_startButton.Active = true;
						_startButton.Clicked -= CallStopApplication;
						_startButton.Clicked -= CallStartApplication;
						_startButton.Clicked += CallStopApplication;
						_startButton.Image = Icons.Stop;
					} else {
						_startButton.Active = false;
						_startButton.Clicked -= CallStopApplication;
						_startButton.Clicked -= CallStartApplication;
						_startButton.Clicked += CallStartApplication;
						_startButton.Image = Icons.Start;
					}
				}
				);
			}
		}

		public event EventHandler StartApplication;
		public event EventHandler StopApplication;
		#endregion
		readonly ToggleButton _startButton;

		/// <summary>
		/// constructor
		/// </summary>
		public AppStartWidget ()
		{
			_startButton = new ToggleButton { WidthRequest = 100 };
			_startButton.Clicked += CallStartApplication;
			Add (_startButton, 0, 0);
			_startButton.Image = Icons.Start;
		}

		/// <summary>
		/// stops application, if running
		/// </summary>
		public void CallStopApplication ()
		{
			if (StopApplication != null) {
				StopApplication (this, new EventArgs ());
			}
		}

		/// <summary>
		/// starts application, updates action for togglebutton
		/// </summary>
		void CallStartApplication ()
		{
			if (StartApplication != null) {
				StartApplication (this, new EventArgs ());
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
		void CallStartApplication (object obj, EventArgs args)
		{
			CallStartApplication ();
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
		void CallStopApplication (object obj, EventArgs args)
		{
			CallStopApplication ();
		}
	}
}