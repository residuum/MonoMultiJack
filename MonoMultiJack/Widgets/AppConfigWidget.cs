// 
// AppConfigWidget.cs
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
using Gtk;
using System;
using MonoMultiJack.Configuration;

namespace MonoMultiJack.Widgets
{
	/// <summary>
	/// Widget for configuring an application for use with jackd
	/// </summary>
	public class AppConfigWidget : Fixed, IAppConfigWidget
	{
		Entry _appNameEntry;
		Entry _appCommandEntry;
		Entry _appArgumentsEntry;
		Button _removeApp;
						
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="appConfig">
		/// The <see cref="AppConfiguration"/> to edit
		/// </param>
		public AppConfigWidget ()
		{
			BuildWidget ();
		}
		
		/// <summary>
		/// builds subwidgets and layout
		/// </summary>
		void BuildWidget ()
		{
			Table table = new Table (3, 4, false);
			table.RowSpacing = 3;
			table.ColumnSpacing = 3;
			table.BorderWidth = 1;
			
			Label label = new Label ("Application");
			label.Xalign = 0;
			table.Attach (label, 0, 2, 0, 1);

			label = new Label ("Name");
			label.Xalign = 0;
			table.Attach (label, 0, 1, 1, 2);
			_appNameEntry = new Entry ();
			table.Attach (_appNameEntry, 1, 2, 1, 2);
			label.MnemonicWidget = _appNameEntry;
			
			label = new Label ("Command");
			label.Xalign = 0;
			table.Attach (label, 0, 1, 2, 3);
			_appCommandEntry = new Entry ();
			table.Attach (_appCommandEntry, 1, 2, 2, 3);
			label.MnemonicWidget = _appCommandEntry;
			
			label = new Label ("Command Arguments");
			label.Xalign = 0;
			table.Attach (label, 0, 1, 3, 4);
			_appArgumentsEntry = new Entry ();
			table.Attach (_appArgumentsEntry, 1, 2, 3, 4);
			label.MnemonicWidget = _appArgumentsEntry;
			
			_removeApp = new Button (Stock.Delete);
			_removeApp.Clicked += HandleClicked;
			;
			table.Attach (_removeApp, 2, 3, 1, 2);

			Put (table, 0, 0);
			table.Show ();
		}

		void HandleClicked (object sender, EventArgs e)
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
			this.ShowAll ();
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