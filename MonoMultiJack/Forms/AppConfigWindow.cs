// 
// AppConfigWindow.cs
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
using System.Collections.Generic;
using Gtk;
using MonoMultiJack;

/// <summary>
/// dialog for application configuration
/// </summary>
public class AppConfigWindow : Gtk.Dialog
{
	//// <value>
	/// table for layout
	/// </value>
	protected Table configTable;
	
	//// <value>
	/// button for adding config widgets
	/// </value>
	protected Button addWidget;
	
	//// <value>
	/// getter for new application configurations
	/// </value>
	public List<AppConfiguration> appConfigs
	{
		get 
		{
			List<AppConfiguration> newAppConfigs = new List<AppConfiguration>();
			AppConfiguration newAppConfig;
			
			foreach (Widget appConfigWidget in this.configTable.Children)
			{
				if (appConfigWidget is AppConfigWidget)
				{
					newAppConfig = ((AppConfigWidget)appConfigWidget).appConfig;
					if ( !newAppConfig.name.Equals(string.Empty) && !newAppConfig.command.Equals(string.Empty))
					{
						newAppConfigs.Add(newAppConfig);
					}
				}
			}
			return newAppConfigs;
		}
	}
			
	public AppConfigWindow (List<AppConfiguration> appConfigs)
	{
		this.Title = "Configure Applications";
		this.Resizable = false;
		this.BuildDialog(appConfigs);
	}
	
	/// <summary>
	/// builds layout of dialog
	/// </summary>
	/// <param name="appConfigs">
	/// A <see cref="List"/> current application configuration
	/// </param>
	protected void BuildDialog (List<AppConfiguration> appConfigs)
	{
		this.configTable = new Table ((uint)appConfigs.Count + 1, 1, false);
		this.configTable.ColumnSpacing = 10;
		this.configTable.RowSpacing = 10;
		this.VBox.PackStart (this.configTable, false, false, 0);
		AppConfigWidget appConfigWidget;
		uint count = 0;
		foreach (AppConfiguration appConfig in appConfigs)
		{
			appConfigWidget = new AppConfigWidget (appConfig);
			this.configTable.Attach (appConfigWidget, 0, 1, count, count + 1);
			count++;
		}
		this.CreateAddButton ();
		this.configTable.Attach (this.addWidget, 0, 1, count, count + 1);
		this.AddButton (Stock.Ok, ResponseType.Ok);
		this.AddButton (Stock.Cancel, ResponseType.Cancel);
	}
	
	/// <summary>
	/// (re-)creates add button
	/// </summary>
	protected void CreateAddButton ()
	{
		if (this.addWidget != null)
		{
			this.addWidget.Destroy ();
		}
		this.addWidget = new Button ("Add Application");
		this.addWidget.Clicked += addNewConfigWidget;
	}
	
	/// <summary>
	/// creates and attaches new application configuration widget
	/// </summary>
	protected void AddNewConfigWidget()
	{
		this.configTable.NRows++;
		AppConfigWidget appConfigWidget = new AppConfigWidget();
		this.configTable.Attach (appConfigWidget, 0, 1, this.configTable.NRows - 2, this.configTable.NRows -1);
		this.CreateAddButton ();
		this.configTable.Attach (this.addWidget, 0, 1, this.configTable.NRows - 1, this.configTable.NRows);
		this.configTable.ShowAll ();
	}
	
	/// <summary>
	/// event handler for creating and attaching new application config widget
	/// </summary>
	/// <param name="sender">
	/// A <see cref="System.Object"/>
	/// </param>
	/// <param name="args">
	/// A <see cref="System.EventArgs"/>
	/// </param>
	protected void addNewConfigWidget (object sender, System.EventArgs args)
	{
		this.AddNewConfigWidget ();
	}
}
