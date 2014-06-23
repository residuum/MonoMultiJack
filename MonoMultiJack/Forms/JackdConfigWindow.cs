// 
// JackdConfigWindow.cs
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

namespace MonoMultiJack.Forms
{
	/// <summary>
	/// Jackd Config Window
	/// </summary>
	public class JackdConfigWindow : Dialog, IJackdConfigWindow
	{
		TextEntry _jackdPathEntry;
		TextEntry _jackdGeneralOptionsEntry;
		TextEntry _jackdDriverEntry;
		TextEntry _jackdDriverOptionsEntry;
		#region IWidget implementation
		void Widgets.IWidget.Show ()
		{
			this.Show ();
		}

		void Widgets.IWidget.Hide ()
		{
			this.Hide ();
		}
		#endregion
		#region IWindow implementation
		public event EventHandler Closing;

		string IWindow.IconPath {
			set {				
				//if (File.Exists (value)) {
				//    this.Icon = new Pixbuf (value);
				//}
			}
		}

		bool IWindow.Sensitive {
			set {
				//this.Sensitive = value;
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
			Title = "Configure Jackd";
			Resizable = false;
			BuildDialog ();
			BindEvents ();
		}

		void BindEvents ()
		{
			Closed += HandleClose;
			this.Buttons.GetCommandButton (Command.Ok).Clicked += HandleOkClick;
			this.Buttons.GetCommandButton (Command.Cancel).Clicked += HandleCancelClick; 
		}

		void HandleCancelClick (object sender, EventArgs e)
		{
			this.Close ();
		}

		void HandleOkClick (object o, EventArgs args)
		{
			if (SaveJackd != null) {
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
		/// builds dialog window
		/// </summary>
		void BuildDialog ()
		{
			Table table = new Table ();

			_jackdPathEntry = BuildRow (table, 0, "Jackd Startup Path");
			_jackdGeneralOptionsEntry = BuildRow (table, 1, "General Options");
			_jackdDriverEntry = BuildRow (table, 2, "Driver Infrastructure");
			_jackdDriverOptionsEntry = BuildRow (table, 3, "Driver Options");

			this.Content = table;

			this.Buttons.Add (new DialogButton (Command.Ok));
			this.Buttons.Add (new DialogButton (Command.Cancel));
		}

		TextEntry BuildRow (Table table, int index, string labelText)
		{
			Label label = new Label (labelText);
			table.Add (label, 0, index);
			TextEntry entry = new TextEntry { MultiLine = false };
			table.Add (entry, 1, index);
			label.LinkClicked += (sender, args) => entry.SetFocus ();
			return entry;
		}
	}
}