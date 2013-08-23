// 
// MainWindow.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2013 Thomas Mayer
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
using System.Collections.Generic;
using System.IO;
using Gdk;
using Gtk;
using MonoMultiJack.Configuration;
using MonoMultiJack.Widgets;

namespace MonoMultiJack.Forms
{
	/// <summary>
	/// Main Window 
	/// </summary>
	public partial class MainWindow: Gtk.Window, IMainWindow
	{		
		readonly string JackdStatusRunning = "Jackd is running.";
		readonly string JackdStatusStopped = "Jackd is stopped.";

		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindow () : base(Gtk.WindowType.Toplevel)
		{
			Build ();
			BuildWindowContent ();
			DeleteEvent += OnDelete;
		}

		#region IWidget implementation
		void IWidget.Show ()
		{
			this.Show ();
		}

		void IWidget.Destroy ()
		{
			this.Destroy ();
		}

		void IWidget.Hide ()
		{
			this.Hide ();
		}
		#endregion

		#region IWindow implementation
		string IWindow.IconPath {
			set {
				if (File.Exists (value)) {
					this.Icon = new Pixbuf (value);
				}
			}
		}

		bool IWindow.Sensitive {
			set {
				this.Sensitive = value;
			}
		}

		public event EventHandler Closing;
		#endregion

		#region IMainWindow implementation		
		IEnumerable<IAppStartWidget> IMainWindow.AppStartWidgets {
			set {
				UpdateAppWidgets (value);
			}
		}

		IEnumerable<IConnectionWidget> IMainWindow.ConnectionWidgets {
			set {
				CreateTabs (value);
			}
		}

		WindowConfiguration IMainWindow.WindowConfiguration {
			get {
				
				int xPosition, yPosition, xSize, ySize;
				GetPosition (out xPosition, out yPosition);
				GetSize (out xSize, out ySize);
				return new WindowConfiguration (
		xPosition,
		yPosition,
		xSize,
		ySize
				);
			}
			set {
				UpdateWindowSize (value);
			}
		}

		bool IMainWindow.JackdIsRunning {
			set {
				stopAction.Sensitive = value;
				_statusbar.Push (0, value ? JackdStatusRunning : JackdStatusStopped);
			}
		}

		bool IMainWindow.AppsAreRunning {
			set {
				stopAllAction.Sensitive = value;
			}
		}
		
		public event EventHandler StartJackd;
		public event EventHandler StopJackd;
		public event EventHandler StopAll;
		public event EventHandler ShowConfigureJackd;
		public event EventHandler ShowConfigureApps;
		public event EventHandler ShowAbout;
		public event EventHandler ShowHelp;
		public event EventHandler QuitApplication;
		#endregion
		
		/// <summary>
		/// builds window content
		/// </summary>
		void BuildWindowContent ()
		{
			Title = "MonoMultiJack";

			((IMainWindow)this).JackdIsRunning = false;
		}

        /// <summary>
        /// Updates appWidgets
        /// </summary>
        /// <param name="appWidgets">The app widgets.</param>
		void UpdateAppWidgets (IEnumerable<IAppStartWidget> appWidgets)
		{
			foreach (Widget widget in _appButtonBox.Children) {
				IAppStartWidget appWidget = widget as IAppStartWidget;
				if (appWidget != null) {
					appWidget.Destroy ();
				}
			}
			foreach (IAppStartWidget appWidget in appWidgets) {
				_appButtonBox.Add ((Widget)appWidget);
				appWidget.Show ();
			}
			_appButtonBox.ShowAll ();
		}

		void CreateTabs (IEnumerable<IConnectionWidget> connectionWidgets)
		{
			foreach (IConnectionWidget widget in connectionWidgets) {
				_connectionNotebook.AppendPage ((Widget)widget, new Label (widget.ConnectionManagerName));
				widget.Show ();
			}
			_connectionNotebook.ShowAll ();
		}
		
		void UpdateWindowSize (WindowConfiguration windowConfig)
		{
			Resize (windowConfig.XSize, windowConfig.YSize);
			Move (windowConfig.XPosition, windowConfig.YPosition);
		}
				
		void CallQuitApplication ()
		{
			if (QuitApplication != null) {
				QuitApplication (this, new EventArgs ());
			}
		}

		void CallStopJackd ()
		{
			if (StopJackd != null) {
				StopJackd (this, new EventArgs ());
			}
		}
		
		void CallStopAll ()
		{
			if (StopAll != null) {
				StopAll (this, new EventArgs ());
			}
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			CallQuitApplication ();
		}
		
		protected virtual void CallStartJackd (object sender, EventArgs e)
		{
			CallStartJackd ();
		}

		void CallStartJackd ()
		{
			if (StartJackd != null) {
				StartJackd (this, new EventArgs ());
			}
		}
	
		protected virtual void CallStopJackd (object sender, EventArgs e)
		{
			CallStopJackd ();
		}
	
		protected virtual void CallStopAll (object sender, EventArgs e)
		{
			CallStopAll ();
		}
		
		protected virtual void CallShowConfigureJackd (object sender, EventArgs e)
		{
			if (ShowConfigureJackd != null) {
				ShowConfigureJackd (this, new EventArgs ());
			}
		}
		
		protected virtual void CallShowConfigureApps (object sender, EventArgs e)
		{
			if (ShowConfigureApps != null) {
				ShowConfigureApps (this, new EventArgs ());
			}

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
			CallQuitApplication ();
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
		protected virtual void CallShowAbout (object sender, EventArgs e)
		{
			if (ShowAbout != null) {
				ShowAbout (this, new EventArgs ());
			}

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
		public void OnDelete (object o, EventArgs args)
		{
			CallQuitApplication ();
		}

	}
}