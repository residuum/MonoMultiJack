// 
// AppConfigWindow.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2011 Thomas Mayer
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
using MonoMultiJack.BusinessLogic.Configuration;
using MonoMultiJack.Widgets;

namespace MonoMultiJack.Forms
{
	public partial class AppConfigWindow : Gtk.Dialog
	{
		//// <value>
		/// table for layout
		/// </value>
		private Table _configTable;
		
		//// <value>
		/// button for adding config widgets
		/// </value>
		private Button _addWidget;
		
		//// <value>
		/// getter for new application configurations
		/// </value>
		public List<AppConfiguration> AppConfigs
		{
			get
			{
				List<AppConfiguration > newAppConfigs = new List<AppConfiguration> ();
				AppConfiguration newAppConfig;
				
				foreach (Widget child in _configTable.Children)
				{
					AppConfigWidget appConfigWidget = child as AppConfigWidget;
					if (appConfigWidget != null)
					{
						newAppConfig = appConfigWidget.appConfig;
						if (!string.IsNullOrEmpty (newAppConfig.Name) && !string.IsNullOrEmpty (newAppConfig.Command))
						{
							newAppConfigs.Add (newAppConfig);
						}
					}
				}
				return newAppConfigs;
			}
		}
		
		public AppConfigWindow ()
		{
			this.Build ();
		}
				
		public AppConfigWindow (List<AppConfiguration> appConfigs) : this()
		{
			Title = "Configure Applications";
			Resizable = true;
			BuildDialog (appConfigs);
		}
		
		/// <summary>
		/// builds layout of dialog
		/// </summary>
		/// <param name="appConfigs">
		/// A <see cref="List"/> current application configuration
		/// </param>
		private void BuildDialog (List<AppConfiguration> appConfigs)
		{
			_configTable = new Table ((uint)appConfigs.Count + 1, 1, false);
			_configTable.ColumnSpacing = 10;
			_configTable.RowSpacing = 10;
			AppConfigWidget appConfigWidget;
			uint count = 0;
			foreach (AppConfiguration appConfig in appConfigs)
			{
				appConfigWidget = new AppConfigWidget (appConfig);
				_configTable.Attach (appConfigWidget, 0, 1, count, count + 1);
				count++;
			}
			CreateAddButton ();
			_configTable.Attach (_addWidget, 0, 1, count, count + 1);
			_appScrolledWindow.AddWithViewport (_configTable);
		}
		
		/// <summary>
		/// (re-)creates add button
		/// </summary>
		private void CreateAddButton ()
		{
			if (_addWidget != null)
			{
				_addWidget.Destroy ();
			}
			_addWidget = new Button ("Add Application");
			_addWidget.Clicked += AddNewConfigWidget;
		}
		
		/// <summary>
		/// creates and attaches new application configuration widget
		/// </summary>
		private void AddNewConfigWidget ()
		{
			_configTable.NRows++;
			AppConfigWidget appConfigWidget = new AppConfigWidget ();
			_configTable.Attach (appConfigWidget, 0, 1, _configTable.NRows - 2, _configTable.NRows - 1);
			CreateAddButton ();
			_configTable.Attach (_addWidget, 0, 1, _configTable.NRows - 1, _configTable.NRows);
			_configTable.ShowAll ();
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
		private void AddNewConfigWidget (object sender, System.EventArgs args)
		{
			AddNewConfigWidget ();
		}
	}
}

