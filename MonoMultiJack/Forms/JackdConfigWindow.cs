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
using MonoMultiJack.BusinessLogic.Configuration;

namespace MonoMultiJack.Forms
{
    /// <summary>
    /// Jackd Config Window
    /// </summary>
    public class JackdConfigWindow : Gtk.Dialog
    {
	private Gtk.Entry _jackdPathEntry;
	private Gtk.Entry _jackdGeneralOptionsEntry;
	private Gtk.Entry _jackdDriverEntry;
	private Gtk.Entry _jackdDriverOptionsEntry;
		
	//// <value>
	/// returns values of entry fields as jackdConfiguration
	/// </value>
	public JackdConfiguration JackdConfig { 
	    get {
		return new JackdConfiguration (
		    _jackdPathEntry.Text.Trim (),
		    _jackdGeneralOptionsEntry.Text.Trim (),
		    _jackdDriverEntry.Text.Trim (),
		    _jackdDriverOptionsEntry.Text.Trim ()
		);
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
	    Table table = new Table (5, 2, false);
	    table.RowSpacing = 2;
	    table.ColumnSpacing = 3;
	    VBox.PackStart (table, false, false, 0);
			
	    Label label = new Label ("Jackd Startup Path");
	    table.Attach (label, 0, 1, 0, 1);
	    _jackdPathEntry = new Entry ();
	    table.Attach (_jackdPathEntry, 1, 2, 0, 1);
	    label.MnemonicWidget = _jackdPathEntry;
	    _jackdPathEntry.Text = jackdConfig.Path;
			
	    label = new Label ("General Options");
	    table.Attach (label, 0, 1, 1, 2);
	    _jackdGeneralOptionsEntry = new Entry ();
	    table.Attach (_jackdGeneralOptionsEntry, 1, 2, 1, 2);
	    label.MnemonicWidget = _jackdGeneralOptionsEntry;
	    _jackdGeneralOptionsEntry.Text = jackdConfig.GeneralOptions;
			
	    label = new Label ("Driver Infrastructure");
	    table.Attach (label, 0, 1, 2, 3);
	    _jackdDriverEntry = new Entry ();
	    table.Attach (_jackdDriverEntry, 1, 2, 2, 3);
	    label.MnemonicWidget = _jackdDriverEntry;
	    _jackdDriverEntry.Text = jackdConfig.Driver;
			
	    label = new Label ("Driver Options");
	    table.Attach (label, 0, 1, 3, 4);
	    _jackdDriverOptionsEntry = new Entry ();
	    table.Attach (_jackdDriverOptionsEntry, 1, 2, 3, 4);
	    label.MnemonicWidget = _jackdDriverOptionsEntry;
	    _jackdDriverOptionsEntry.Text = jackdConfig.DriverOptions;
			
	    AddButton (Stock.Ok, ResponseType.Ok);
	    AddButton (Stock.Cancel, ResponseType.Cancel);
	}
    }
}