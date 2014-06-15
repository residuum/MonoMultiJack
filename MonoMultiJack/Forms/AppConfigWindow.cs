// 
// AppConfigWindow.cs
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
using System.Linq;
using MonoMultiJack.Widgets;
using Xwt;

namespace MonoMultiJack.Forms
{
	public class AppConfigWindow : Window, IAppConfigWindow
	{
		Table _configTable;
		private Button _addButton;
		private Button _okButton;
		private Button _cancelButton;

		public AppConfigWindow ()
		{
			BuildWindow ();
			BindEvents ();
			
			Title = "Configure Applications";
			Resizable = true;
		}

		private void BindEvents ()
		{
			Closed += HandleClose;
			_okButton.Clicked += HandleOkClick;
			_cancelButton.Clicked += HandleCancelClick;
			_addButton.Clicked += HandleAddClick;
		}

		private void HandleCancelClick (object sender, EventArgs e)
		{
			HandleClose (sender, e);
		}

		void HandleOkClick (object o, EventArgs args)
		{
			if (SaveApplicationConfigs != null) {
				SaveApplicationConfigs (this, new EventArgs ());
			}
			HandleClose (o, args);
		}

		void HandleClose (object sender, EventArgs e)
		{
			if (Closing != null) {
				Closing (this, new EventArgs ());
			}
		}
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			this.Dispose ();
		}
		#endregion
		#region IWidget implementation
		void IWidget.Show ()
		{
			this.Show ();
		}

		void IWidget.Hide ()
		{
			this.Hide ();
		}
		#endregion
		#region IWindow implementation
		string IWindow.IconPath {
			set {
				throw new System.NotImplementedException ();
			}
		}

		bool IWindow.Sensitive {
			set {
				//this.Sensitive = value;
			}
		}

		public event EventHandler Closing;
		#endregion
		#region IAppConfigWindow implementation
		public event EventHandler SaveApplicationConfigs;
		public event EventHandler AddApplication;

		void IAppConfigWindow.AddAppConfigWidget (IAppConfigWidget widget)
		{
			_configTable.Add ((Widget)widget, 0, _configTable.Children.Count ());
		}

		void IAppConfigWindow.RemoveAppConfigWidget (IAppConfigWidget widget)
		{
			_configTable.Remove ((Widget)widget);
		}
		#endregion
		void BuildWindow ()
		{
			_configTable = new Table ();
			ScrollView scrollView = new ScrollView (_configTable);
			scrollView.ExpandHorizontal = true;
			scrollView.ExpandVertical = true;
			scrollView.HorizontalScrollPolicy = ScrollPolicy.Never;
			HBox buttonBox = new HBox ();
			_addButton = new Button (Command.Add.Label) { Image = StockIcons.Add };
			_okButton = new Button (Command.Ok.Label);
			_cancelButton = new Button (Command.Cancel.Label);
			buttonBox.PackEnd (_addButton);
			buttonBox.PackEnd (_okButton);
			buttonBox.PackEnd (_cancelButton);
            
			VBox box = new VBox ();
			box.PackStart (scrollView);
			box.PackEnd (buttonBox);
			this.Content = box;
		}

		void CallAddNewConfigWidget ()
		{
			if (AddApplication != null) {
				AddApplication (this, new EventArgs ());
			}
		}

		protected void HandleAddClick (object sender, EventArgs e)
		{
			CallAddNewConfigWidget ();
		}
	}
}