// 
// JackdConfigWindow.cs
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
using MonoMultiJack;

namespace MonoMultiJack
{
	/// <summary>
	/// Jackd Config Window
	/// </summary>
	public class JackdConfigWindow : Gtk.Dialog
	{
		protected Gtk.Entry _jackdPathEntry;
		protected Gtk.Entry _jackdAudiorateEntry;
		protected Gtk.Entry _jackdDriverEntry;
		
		//// <value>
		/// returns values of entry fields as jackdConfiguration
		/// </value>
		public JackdConfiguration jackdConfig 
		{ 
			get 
			{
				return new JackdConfiguration(this._jackdPathEntry.Text.Trim(), this._jackdDriverEntry.Text.Trim(), this._jackdAudiorateEntry.Text.Trim());
			}
		}
		
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="jackdConfig">
		/// A <see cref="JackdConfiguration"/>
		/// </param>
		public JackdConfigWindow (JackdConfiguration jackdConfig)
		{
			this.Title = "Configure Jackd";
			this.Resizable = false;
			this.BuildDialog (jackdConfig);
		}
		
		/// <summary>
		/// builds dialog window, fills entry fields
		/// </summary>
		/// <param name="jackdConfig">
		/// A <see cref="JackdConfiguration"/>
		/// </param>
		protected void BuildDialog (JackdConfiguration jackdConfig)
		{
			Table table = new Table (3, 2, false);
			table.RowSpacing = 2;
		    table.ColumnSpacing = 3;
			this.VBox.PackStart (table, false, false, 0);
			
			Label label = new Label ("Jackd Startup Path");
			table.Attach (label, 0, 1, 0, 1);
			this._jackdPathEntry = new Entry();
			table.Attach (this._jackdPathEntry, 1, 2, 0, 1);
			label.MnemonicWidget = this._jackdPathEntry;
			if (jackdConfig != null)
			{
				this._jackdPathEntry.Text = jackdConfig.path;
			}
			
			label = new Label ("Audiorate");
			table.Attach (label, 0, 1, 1, 2);
			this._jackdAudiorateEntry = new Entry ();
			table.Attach (this._jackdAudiorateEntry, 1, 2, 1, 2);
			label.MnemonicWidget = this._jackdAudiorateEntry;
			if (jackdConfig != null)
			{
				this._jackdAudiorateEntry.Text = jackdConfig.audiorate;
			}
			
			label = new Label ("Driver Infrastructure");
			table.Attach (label, 0, 1, 2, 3);
			this._jackdDriverEntry = new Entry ();
			table.Attach (this._jackdDriverEntry, 1, 2, 2, 3);
			label.MnemonicWidget = this._jackdDriverEntry;
			if (jackdConfig != null)
			{
				this._jackdDriverEntry.Text = jackdConfig.driver;
			}
			
			this.AddButton(Stock.Ok, ResponseType.Ok);
			this.AddButton(Stock.Cancel, ResponseType.Cancel);
		}
	}
}