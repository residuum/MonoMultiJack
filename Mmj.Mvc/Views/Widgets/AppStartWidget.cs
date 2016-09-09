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

namespace Mmj.Views.Widgets
{
	/// <summary>
	/// Widget for applications to start / stop
	/// </summary>
	public class AppStartWidget : Table, IAppStartWidget
	{
		public new void Dispose ()
		{
			Dispose(true);
		}
		~AppStartWidget()
		{
			Dispose(false);
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

		protected new void Dispose(bool isDisposing)
		{
			_startButton.Clicked -= CallStop;
			_startButton.Clicked -= CallStart;
			base.Dispose(isDisposing);
		}

#endregion

#region IAppWidget implementation

		void IAppStartWidget.SetApp (string name, string commandName)
		{
			_startButton.Label = name;
			Name = name.CreateWidgetName ();
			_startButton.Name = commandName.CreateWidgetName ();
		}

		bool IAppStartWidget.IsRunning {

			set {
				Application.Invoke (delegate {
					if (value) {
						_startButton.Active = true;
						_startButton.Clicked -= CallStop;
						_startButton.Clicked -= CallStart;
						_startButton.Clicked += CallStop;
						_startButton.Image = Icons.Stop;
					} else {
						_startButton.Active = false;
						_startButton.Clicked -= CallStop;
						_startButton.Clicked -= CallStart;
						_startButton.Clicked += CallStart;
						_startButton.Image = Icons.Start;
					}
				});
			}
		}

		public event EventHandler Start;
		public event EventHandler Stop;

#endregion

		readonly ToggleButton _startButton;

		/// <summary>
		/// constructor
		/// </summary>
		public AppStartWidget ()
		{
			_startButton = new ToggleButton { WidthRequest = 100 };
			_startButton.Clicked += CallStart;
			Add (_startButton, 0, 0);
			_startButton.Image = Icons.Start;
		}

		/// <summary>
		/// stops application, if running
		/// </summary>
		public void CallStop ()
		{
			if (Stop != null) {
				Stop (this, new EventArgs ());
			}
		}

		/// <summary>
		/// starts application, updates action for togglebutton
		/// </summary>
		void CallStart ()
		{
			if (Start != null) {
				Start (this, new EventArgs ());
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
		void CallStart (object obj, EventArgs args)
		{
			CallStart ();
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
		void CallStop (object obj, EventArgs args)
		{
			CallStop ();
		}
	}
}
