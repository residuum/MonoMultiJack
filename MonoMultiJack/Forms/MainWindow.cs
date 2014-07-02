// 
// MainWindow.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoMultiJack.Configuration;
using MonoMultiJack.Widgets;
using Xwt;
using Xwt.Drawing;

namespace MonoMultiJack.Forms
{
	/// <summary>
	/// Main Window 
	/// </summary>
	public class MainWindow : Window, IMainWindow
	{
		readonly string JackdStatusRunning = "Jackd is running.";
		readonly string JackdStatusStopped = "Jackd is stopped.";
		VBox _appButtonBox;
		Notebook _connectionNotebook;
		Label _statusbar;
		MenuItem _stopAction;
		MenuItem _stopAllAction;

		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindow ()
		{
			BuildMenu ();
			BuildWindowContent ();
			this.Closed += OnCloseEvent;
		}

		void BuildMenu ()
		{
			this.MainMenu = new Menu ();
			MenuItem file = new MenuItem ("_File");
			file.SubMenu = new Menu ();
			foreach (MenuItem menuItem in BuildFileMenu()) {
				file.SubMenu.Items.Add (menuItem);
			}
			this.MainMenu.Items.Add (file);
			MenuItem configuration = new MenuItem ("_Configuration");
			configuration.SubMenu = new Menu ();
			foreach (MenuItem menuItem in BuildConfigMenu()) {
				configuration.SubMenu.Items.Add (menuItem);
			}
			this.MainMenu.Items.Add (configuration);
			MenuItem help = new MenuItem ("_Help");
			help.SubMenu = new Menu ();
			foreach (MenuItem menuItem in BuildHelpMenu()) {
				help.SubMenu.Items.Add (menuItem);
			}
			this.MainMenu.Items.Add (help);

		}

		IEnumerable<MenuItem> BuildHelpMenu ()
		{
			yield return CreateMenuItem ("About", CallShowAbout, StockIcons.Information);
		}

		IEnumerable<MenuItem> BuildConfigMenu ()
		{
			yield return CreateMenuItem ("Configure Jackd", CallShowConfigureJackd);
			yield return CreateMenuItem ("Add / Remove Applications", CallShowConfigureApps);
		}

		IEnumerable<MenuItem> BuildFileMenu ()
		{
			yield return CreateMenuItem ("(Re)Start _Jackd", CallStartJackd);
			_stopAction = CreateMenuItem ("_Stop Jackd", CallStopJackd);
			yield return _stopAction;
			_stopAllAction = CreateMenuItem ("Stop _All", CallStopAll);
			yield return _stopAllAction;
			yield return CreateMenuItem ("_Quit", OnQuitActionActivated);
		}

		static MenuItem CreateMenuItem (string name, EventHandler handler, Image icon = null)
		{
			MenuItem menuItem = new MenuItem (name);
			if (icon != null) {
				menuItem.Image = icon;
			}
			menuItem.Clicked += handler;
			return menuItem;
		}
		#region IWidget implementation
		void IWidget.Show ()
		{
			this.Show ();
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
					this.Icon = Image.FromFile (value);
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
				return new WindowConfiguration (X, Y, Width, Height);
			}
			set {
				UpdateWindowSize (value);
			}
		}

		bool IMainWindow.JackdIsRunning {
			set {
				Application.Invoke (() =>
				{
					_stopAction.Sensitive = value;
					_statusbar.Text = value ? JackdStatusRunning : JackdStatusStopped;
				});
			}
		}

		bool IMainWindow.AppsAreRunning {
			set {
				Application.Invoke (() =>
				{
					_stopAllAction.Sensitive = value;
				});
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
			_connectionNotebook = new Notebook {
				ExpandHorizontal = true,
				ExpandVertical = true
			};
			_appButtonBox = new VBox { ExpandVertical = true };
			HBox mainContent = new HBox {
				ExpandHorizontal = true,
				ExpandVertical = true
			};
			mainContent.PackStart (_appButtonBox);
			mainContent.PackStart (_connectionNotebook, true, true);

			VBox container = new VBox ();
			container.PackStart (mainContent, true, true);
			_statusbar = new Label ();
			container.PackEnd (_statusbar);

			this.Content = container;

			((IMainWindow)this).JackdIsRunning = false;
		}

		/// <summary>
		/// Updates appWidgets
		/// </summary>
		/// <param name="appWidgets">The app widgets.</param>
		void UpdateAppWidgets (IEnumerable<IAppStartWidget> appWidgets)
		{
			Application.Invoke (() =>
			{
				foreach (IAppStartWidget appWidget in _appButtonBox.Children.OfType<IAppStartWidget>()) {
					appWidget.Dispose ();
				}
				_appButtonBox.Clear ();
				foreach (IAppStartWidget appWidget in appWidgets) {
					_appButtonBox.PackStart ((Widget)appWidget);
					appWidget.Show ();
				}
			});
		}

		void CreateTabs (IEnumerable<IConnectionWidget> connectionWidgets)
		{
			foreach (IConnectionWidget widget in connectionWidgets) {
				_connectionNotebook.Add ((Widget)widget, widget.ConnectionManagerName);
			}
		}

		void UpdateWindowSize (WindowConfiguration windowConfig)
		{
			Height = windowConfig.Height;
			Width = windowConfig.Width;

			X = windowConfig.XPosition;
			Y = windowConfig.YPosition;
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

		protected void OnCloseEvent (object sender, EventArgs a)
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