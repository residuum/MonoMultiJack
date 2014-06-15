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
using Xwt.Drawing;

namespace MonoMultiJack.Widgets
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

		private void BindEvents ()
		{
			_removeApp.Clicked += HandleRemoveClick;
		}

		/// <summary>
		/// builds subwidgets and layout
		/// </summary>
		void BuildWidget ()
		{
			Table table = new Table ();

			Label label = new Label ("Application");
			label.Font = label.Font.WithWeight (FontWeight.Bold);
			table.Add (label, 0, 0, 1, 3);

			_appNameEntry = BuildRow (table, 1, "Name");
			_appCommandEntry = BuildRow (table, 2, "Command");
			_appArgumentsEntry = BuildRow (table, 3, "Command Arguments");

			_removeApp = new Button ("Delete") { Image = StockIcons.Remove };
			table.Add (_removeApp, 2, 2);
			Content = table;
		}

		private TextEntry BuildRow (Table table, int index, string labelText)
		{
			Label label = new Label (labelText);
			table.Add (label, 0, index);
			TextEntry entry = new TextEntry { MultiLine = false };
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
			this.Dispose ();
		}
		#endregion
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