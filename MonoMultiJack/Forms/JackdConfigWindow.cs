// 
// JackdConfigWindow.cs
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
using MonoMultiJack.Configuration;
using System;
using System.IO;
using Gdk;
using Mono.Unix;

namespace MonoMultiJack.Forms
{
	/// <summary>
	/// Jackd Config Window
	/// </summary>
	public class JackdConfigWindow : Dialog, IJackdConfigWindow
	{
		Entry _jackdPathEntry;
		Entry _jackdGeneralOptionsEntry;
		Entry _jackdDriverEntry;
		Entry _jackdDriverOptionsEntry;

		#region IWidget implementation
		void MonoMultiJack.Widgets.IWidget.Show ()
		{
			this.Show ();
		}

		void MonoMultiJack.Widgets.IWidget.Destroy ()
		{
			this.Destroy ();
		}

		void MonoMultiJack.Widgets.IWidget.Hide ()
		{
			this.Hide ();
		}
		#endregion

		#region IWindow implementation
		public event EventHandler Closing;

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
		#endregion

		#region IJackdConfigWindow implementation
		string IJackdConfigWindow.Path {
			get {
				return _jackdPathEntry.Text.Trim ();
			}
			set {
				_jackdPathEntry.Text = value;
			}
		}

		string IJackdConfigWindow.GeneralOptions {
			get {
				return _jackdGeneralOptionsEntry.Text.Trim ();
			}
			set {
				_jackdGeneralOptionsEntry.Text = value;
			}
		}

		string IJackdConfigWindow.Driver {
			get {
				return _jackdDriverEntry.Text.Trim ();
			}
			set {
				_jackdDriverEntry.Text = value;
			}
		}

		string IJackdConfigWindow.DriverOptions {
			get {
				return _jackdDriverOptionsEntry.Text.Trim ();
			}
			set {
				_jackdDriverOptionsEntry.Text = value;
			}
		}

		public event EventHandler SaveJackd;
		#endregion

		
		/// <summary>
		/// constructor
		/// </summary>
		public JackdConfigWindow ()
		{
			Title = Catalog.GetString ("Configure Jackd");
			Resizable = false;
			BuildDialog ();
			Close += HandleClose;
			Response += HandleResponse;
		}

		void HandleResponse (object o, ResponseArgs args)
		{
			if (args.ResponseId == ResponseType.Ok && SaveJackd != null) {
				SaveJackd (this, new EventArgs ());
			}
			HandleClose (o, args);
		}

		void HandleClose (object sender, EventArgs e)
		{
			if (Closing != null) {
				Closing (this, new EventArgs ());
			}
		}
		
		/// <summary>
		/// builds dialog window, fills entry fields
		/// </summary>
		/// <param name="jackdConfig">
		/// A <see cref="JackdConfiguration"/>
		/// </param>
		void BuildDialog ()
		{
			Table table = new Table (5, 2, false);
			table.RowSpacing = 2;
			table.ColumnSpacing = 3;
			VBox.PackStart (table, false, false, 0);
			
			Label label = new Label (Catalog.GetString ("Jackd Startup Path"));
			table.Attach (label, 0, 1, 0, 1);
			_jackdPathEntry = new Entry ();
			table.Attach (_jackdPathEntry, 1, 2, 0, 1);
			label.MnemonicWidget = _jackdPathEntry;
			
			label = new Label (Catalog.GetString ("General Options"));
			table.Attach (label, 0, 1, 1, 2);
			_jackdGeneralOptionsEntry = new Entry ();
			table.Attach (_jackdGeneralOptionsEntry, 1, 2, 1, 2);
			label.MnemonicWidget = _jackdGeneralOptionsEntry;
			
			label = new Label (Catalog.GetString ("Driver Infrastructure"));
			table.Attach (label, 0, 1, 2, 3);
			_jackdDriverEntry = new Entry ();
			table.Attach (_jackdDriverEntry, 1, 2, 2, 3);
			label.MnemonicWidget = _jackdDriverEntry;
			
			label = new Label (Catalog.GetString ("Driver Options"));
			table.Attach (label, 0, 1, 3, 4);
			_jackdDriverOptionsEntry = new Entry ();
			table.Attach (_jackdDriverOptionsEntry, 1, 2, 3, 4);
			label.MnemonicWidget = _jackdDriverOptionsEntry;
			
			VBox.ShowAll ();
			AddButton (Stock.Ok, ResponseType.Ok);
			AddButton (Stock.Cancel, ResponseType.Cancel);
		}
	}
}