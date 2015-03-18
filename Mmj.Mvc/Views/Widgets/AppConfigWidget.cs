// 
// AppConfigWidget.cs
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
	/// Widget for configuring an application for use with jackd
	/// </summary>
	public class AppConfigWidget : Widget, IAppConfigWidget
	{
		TextEntry _appNameEntry;
		TextEntry _appCommandEntry;
		TextEntry _appArgumentsEntry;
		Button _removeApp;

		/// <summary>
		/// constructor
		/// </summary>
		public AppConfigWidget ()
		{
			BuildWidget ();
			BindEvents ();
		}

		void BindEvents ()
		{
			_removeApp.Clicked += HandleRemoveClick;
		}

		/// <summary>
		/// builds subwidgets and layout
		/// </summary>
		void BuildWidget ()
		{
			Table table = new Table ();

			_appNameEntry = BuildRow (table, 0, "Name", "e.g. Ardour");
			_appCommandEntry = BuildRow (table, 1, "Command", "e.g. /usr/bin/ardour3");
			_appCommandEntry.FileSelector ();
			_appArgumentsEntry = BuildRow (table, 2, "Command Arguments", "optional");

			_removeApp = new Button ("Delete") { Image = Icons.Delete };
			table.Add (_removeApp, 2, 1);
			table.Margin = new WidgetSpacing (4, 8, 4, 8);
			Content = table;
		}

		TextEntry BuildRow (Table table, int index, string labelText, string placeholder)
		{
			Label label = new Label (labelText);
			table.Add (label, 0, index);
			TextEntry entry = new TextEntry {
				MultiLine = false,
				PlaceholderText = placeholder
			};
			table.Add (entry, 1, index);
			label.LinkClicked += (sender, args) => entry.SetFocus ();
			return entry;
		}

		void HandleRemoveClick (object sender, EventArgs e)
		{
			if (RemoveApplication != null) {
				RemoveApplication (this, new EventArgs ());
			}
		}
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			Dispose ();
		}
		#endregion
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
		#region IAppConfigWidget implementation
		string IAppConfigWidget.Name {
			get {
				return _appNameEntry.Text.Trim ();
			}
			set {
				_appNameEntry.Text = value;
			}
		}

		string IAppConfigWidget.Command {
			get {
				return _appCommandEntry.Text.Trim ();
			}
			set {
				_appCommandEntry.Text = value;
			}
		}

		string IAppConfigWidget.Arguments {
			get {
				return _appArgumentsEntry.Text.Trim ();
			}
			set {
				_appArgumentsEntry.Text = value;
			}
		}

		public event EventHandler RemoveApplication;
		#endregion
	}
}