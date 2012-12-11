// 
// AppConfigWindow.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2012 Thomas Mayer
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
	public partial class AppConfigWindow : Dialog, IAppConfigWindow
	{
		Table _configTable;
				
		public AppConfigWindow()
		{
			BuildDialog();
			this.Close += HandleClose;
			this.Response += HandleResponse;
			
			Title = "Configure Applications";
			Resizable = true;
		}

		void HandleResponse (object o, ResponseArgs args)
		{
			if (args.ResponseId == ResponseType.Ok && SaveApplicationConfigs != null) {
				SaveApplicationConfigs(this, new EventArgs());
			}
			HandleClose(o, args);
		}

		void HandleClose (object sender, EventArgs e)
		{
			if (Closing != null){
				Closing(this, new EventArgs());
			}
		}

		#region IDisposable implementation
		void IDisposable.Dispose()
		{
			this.Dispose();
		}
		#endregion

		#region IWidget implementation
		void IWidget.Show()
		{
			this.Show();
		}

		void IWidget.Destroy()
		{
			this.Destroy();
		}

		void IWidget.Hide()
		{
			this.Hide();
		}
		#endregion

		#region IWindow implementation
		string IWindow.IconPath {
			set {
				throw new System.NotImplementedException();
			}
		}

		bool IWindow.Sensitive {
			set {
				this.Sensitive = value;
			}
		}
		
		public event EventHandler Closing;
		#endregion

		#region IAppConfigWindow implementation
		public event EventHandler SaveApplicationConfigs;
		public event EventHandler AddApplication;

		void IAppConfigWindow.AddAppConfigWidget (IAppConfigWidget widget){
			uint count = _configTable.NRows;
			_configTable.NRows += 1;
			_configTable.Attach((Widget) widget, 0, 1, count, count +1);
			widget.Show();
		}

		void IAppConfigWindow.RemoveAppConfigWidget(IAppConfigWidget widget) {
			_configTable.Remove((Widget)widget);
		}
		#endregion		

		void BuildDialog()
		{
			Modal = true;
			this.VBox.BorderWidth = ((uint)(2));
			this.ActionArea.Spacing = 10;
			this.ActionArea.BorderWidth = (uint)(5);
			this.ActionArea.LayoutStyle = ButtonBoxStyle.End;
			Button addButton = new Button ();
			addButton.CanFocus = true;
			addButton.UseUnderline = true;
			addButton.Image = new Gtk.Image("gtk-add",IconSize.Button);
			addButton.Label = "Add Application";
			addButton.Clicked += AddButtonClicked;
			this.ActionArea.Add(addButton);
			Button cancelButton = new Gtk.Button ();
			cancelButton.CanDefault = true;
			cancelButton.CanFocus = true;
			cancelButton.Name = "buttonCancel";
			cancelButton.UseStock = true;
			cancelButton.UseUnderline = true;
			cancelButton.Label = "gtk-cancel";
			this.AddActionWidget (cancelButton, ResponseType.Cancel);
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			Button okButton = new Button ();
			okButton.CanDefault = true;
			okButton.CanFocus = true;
			okButton.Name = "buttonOk";
			okButton.UseStock = true;
			okButton.UseUnderline = true;
			okButton.Label = "gtk-ok";
			this.AddActionWidget (okButton, ResponseType.Ok);
			this.DefaultWidth = 466;
			this.DefaultHeight = 300;
			this.Show ();
			_configTable = new Table(1, 1, false);
			_configTable.ColumnSpacing = 10;
			_configTable.RowSpacing = 10;
			ScrolledWindow appScrolledWindow = new ScrolledWindow ();
			appScrolledWindow.AddWithViewport(_configTable);
			this.VBox.Add (appScrolledWindow);
			this.VBox.ShowAll();
		}
				
		void CallAddNewConfigWidget()
		{
			if (AddApplication != null)
			{
				AddApplication(this, new EventArgs());
			}
		}

		protected void AddButtonClicked(object sender, EventArgs e)
		{
			CallAddNewConfigWidget();
		}

	}
}