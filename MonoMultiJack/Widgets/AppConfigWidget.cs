// 
// AppConfigWidget.cs
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

using Gtk;
using System;

namespace MonoMultiJack.Widgets
{
	/// <summary>
	/// Widget for configuring an application for use with jackd
	/// </summary>
	public class AppConfigWidget : Gtk.Fixed
	{
		//// <value>
		/// Entry field for application name
		/// </value>
		protected Gtk.Entry _appNameEntry;
		
		//// <value>
		/// Entry field for application startup command
		/// </value>
		protected Gtk.Entry _appCommandEntry;
		
		//// <value>
		/// button for destroying widget
		/// </value>
		protected Gtk.Button _removeApp;
		
		//// <value>
		/// edited configuration
		/// </value>
		public AppConfiguration appConfig
		{
			get 
			{
				return new AppConfiguration(this._appNameEntry.Text.Trim(), this._appCommandEntry.Text.Trim());
			}
		}
		
		public AppConfigWidget()
		{
			this.BuildWidget();
		}
		
		public AppConfigWidget(AppConfiguration appConfig)
		{
			this.BuildWidget();
			this._appNameEntry.Text = appConfig.name;
			this._appCommandEntry.Text = appConfig.command;
		}
		
		/// <summary>
		/// builds subwidgets and layout
		/// </summary>
		protected void BuildWidget()
		{
			Table table = new Table(3, 3, false);
			table.RowSpacing = 3;
			table.ColumnSpacing = 3;
			table.BorderWidth = 1;
			
			Label label = new Label ("Application");
			label.Xalign = 0;
			table.Attach (label, 0, 2, 0, 1);

			label = new Label ("Name");
			label.Xalign = 0;
			table.Attach (label, 0, 1, 1, 2);
			this._appNameEntry = new Entry();
			table.Attach (this._appNameEntry, 1, 2, 1, 2);
			label.MnemonicWidget = this._appNameEntry;
			
			label = new Label ("Command");
			label.Xalign = 0;
			table.Attach (label, 0, 1, 2, 3);
			this._appCommandEntry = new Entry();
			table.Attach (this._appCommandEntry, 1, 2, 2, 3);
			label.MnemonicWidget = this._appCommandEntry;
			
			this._removeApp = new Button(Stock.Delete);
			this._removeApp.Clicked += RemoveApp;
			table.Attach(this._removeApp,2, 3, 1, 2);
			
			
			this.Put(table, 0, 0);
		}
		
		/// <summary>
		/// destroys widget
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected void RemoveApp(object sender, System.EventArgs args)
		{
			this.Destroy ();
		}
	}
}
