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

using System;
using Gtk;

namespace MonoMultiJack
{
	/// <summary>
	/// Widget for configuring an application for use with jackd
	/// </summary>
	public class AppConfigWidget : Gtk.Fixed
	{
		//// <value>
		/// Entry field for application name
		/// </value>
		protected Gtk.Entry appNameEntry;
		
		//// <value>
		/// Entry field for application startup command
		/// </value>
		protected Gtk.Entry appCommandEntry;
		
		//// <value>
		/// button for destroying widget
		/// </value>
		protected Gtk.Button removeApp;
		
		//// <value>
		/// edited configuration
		/// </value>
		public AppConfiguration appConfig
		{
			get 
			{
				return new AppConfiguration(this.appNameEntry.Text.Trim(), this.appCommandEntry.Text.Trim());
			}
		}
		
		public AppConfigWidget()
		{
			this.BuildWidget();
		}
		
		public AppConfigWidget(AppConfiguration appConfig)
		{
			this.BuildWidget();
			this.appNameEntry.Text = appConfig.name;
			this.appCommandEntry.Text = appConfig.command;
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
			this.appNameEntry = new Entry();
			table.Attach (this.appNameEntry, 1, 2, 1, 2);
			label.MnemonicWidget = this.appNameEntry;
			
			label = new Label ("Command");
			label.Xalign = 0;
			table.Attach (label, 0, 1, 2, 3);
			this.appCommandEntry = new Entry();
			table.Attach (this.appCommandEntry, 1, 2, 2, 3);
			label.MnemonicWidget = this.appCommandEntry;
			
			this.removeApp = new Button(Stock.Delete);
			this.removeApp.Clicked += RemoveApp;
			table.Attach(this.removeApp,2, 3, 1, 2);
			
			
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
