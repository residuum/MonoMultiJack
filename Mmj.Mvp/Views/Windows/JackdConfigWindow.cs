// 
// JackdConfigWindow.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2015 Thomas Mayer
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
using Mmj.OS;
using Xwt;
using Xwt.Drawing;

namespace Mmj.Views.Windows
{
	/// <summary>
	/// Jackd Config Window
	/// </summary>
	public class JackdConfigWindow : Window, IJackdConfigWindow
	{
		TextEntry _jackdPathEntry;
		TextEntry _jackdGeneralOptionsEntry;
		TextEntry _jackdDriverEntry;
		TextEntry _jackdDriverOptionsEntry;
		Button _okButton;
		Button _cancelButton;

#region IWidget implementation

		void Widgets.IWidget.Show ()
		{
			Show ();
		}

		void Widgets.IWidget.Hide ()
		{
			Hide ();
		}

#endregion

#region IWindow implementation

		public event EventHandler Closing;

		Image IWindow.Icon {
			set {
				Icon = value;
			}
		}

		bool IWindow.Sensitive {
			set {
				Sensitive = value;
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

		public event EventHandler Save;

#endregion

		/// <summary>
		/// constructor
		/// </summary>
		public JackdConfigWindow ()
		{
			Title = I18N._ ("Configure Jackd");
			Resizable = false;
			BuildContent ();
			BindEvents ();
		}

		~JackdConfigWindow()
		{
			Dispose(false);
		}

		public new void Dispose()
		{
			Dispose(true);
		}

		protected new void Dispose(bool isDisposing)
		{
			Closed -= HandleClose;
			_okButton.Clicked -= HandleOk;
			_cancelButton.Clicked -= HandleCancel;
			base.Dispose(isDisposing);
		}

		void BindEvents ()
		{
			Closed += HandleClose;
			_okButton.Clicked += HandleOk;
			_cancelButton.Clicked += HandleCancel; 
		}

		void HandleCancel (object sender, EventArgs e)
		{
			Close ();
		}

		void HandleOk (object o, EventArgs args)
		{
			if (Save != null) {
				Save (this, new EventArgs ());
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
		void BuildContent ()
		{
			Table table = new Table ();

			_jackdPathEntry = BuildRow (table, 0, I18N._ ("Jackd Startup Path"), I18N._ ("e.g. /usr/bin/jackd"));
			_jackdPathEntry.FileSelector ();
			_jackdGeneralOptionsEntry = BuildRow (table, 1, I18N._ ("General Options"), I18N._ ("optional"));
			_jackdDriverEntry = BuildRow (table, 2, I18N._ ("Driver Infrastructure"), I18N._ ("e.g. alsa"));
			_jackdDriverOptionsEntry = BuildRow (table, 3, I18N._ ("Driver Options"), I18N._ ("optional"));

			HBox buttonBox = new HBox ();
			_okButton = new Button (I18N._ ("Save")) { Image = Icons.Ok };
			_cancelButton = new Button (I18N._ ("Cancel")) {
				Image = Icons.Cancel,
				      Style = ButtonStyle.Flat
			};
			buttonBox.PackStart (_cancelButton);
			buttonBox.PackEnd (_okButton);

			VBox box = new VBox ();
			box.PackStart (table);
			box.PackEnd (buttonBox);
			Content = box;
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
	}
}
