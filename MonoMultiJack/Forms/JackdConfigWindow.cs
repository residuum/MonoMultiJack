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
using MonoMultiJack.Configuration;

namespace MonoMultiJack
{
	/// <summary>
	/// Jackd Config Window
	/// </summary>
	public class JackdConfigWindow : Gtk.Dialog
	{
		private Gtk.Entry _jackdPathEntry;
		private Gtk.Entry _jackdAudiorateEntry;
		private Gtk.Entry _jackdDriverEntry;
		
		//// <value>
		/// returns values of entry fields as jackdConfiguration
		/// </value>
		public JackdConfiguration JackdConfig 
		{ 
			get 
			{
				return new JackdConfiguration(_jackdPathEntry.Text.Trim(), _jackdDriverEntry.Text.Trim(), _jackdAudiorateEntry.Text.Trim());
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
			Title = "Configure Jackd";
			Resizable = false;
			BuildDialog (jackdConfig);
		}
		
		/// <summary>
		/// builds dialog window, fills entry fields
		/// </summary>
		/// <param name="jackdConfig">
		/// A <see cref="JackdConfiguration"/>
		/// </param>
		private void BuildDialog (JackdConfiguration jackdConfig)
		{
			Table table = new Table (3, 2, false);
			table.RowSpacing = 2;
		    table.ColumnSpacing = 3;
			VBox.PackStart (table, false, false, 0);
			
			Label label = new Label ("Jackd Startup Path");
			table.Attach (label, 0, 1, 0, 1);
			_jackdPathEntry = new Entry();
			table.Attach (_jackdPathEntry, 1, 2, 0, 1);
			label.MnemonicWidget = _jackdPathEntry;
			_jackdPathEntry.Text = jackdConfig.Path;
			
			label = new Label ("Audiorate");
			table.Attach (label, 0, 1, 1, 2);
			_jackdAudiorateEntry = new Entry ();
			table.Attach (_jackdAudiorateEntry, 1, 2, 1, 2);
			label.MnemonicWidget = _jackdAudiorateEntry;
			_jackdAudiorateEntry.Text = jackdConfig.Audiorate;
			
			label = new Label ("Driver Infrastructure");
			table.Attach (label, 0, 1, 2, 3);
			_jackdDriverEntry = new Entry ();
			table.Attach (_jackdDriverEntry, 1, 2, 2, 3);
			label.MnemonicWidget = _jackdDriverEntry;
			_jackdDriverEntry.Text = jackdConfig.Driver;
			
			AddButton(Stock.Ok, ResponseType.Ok);
			AddButton(Stock.Cancel, ResponseType.Cancel);
		}
	}
}