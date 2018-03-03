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
using Mmj.OS;
using Xwt;
using Xwt.Drawing;
using Mmj.Views.Widgets;

namespace Mmj.Views.Windows
{
	public class AppConfigWindow : Window, IAppConfigWindow
	{
		Table _configTable;
		Button _addButton;
		Button _okButton;
		Button _cancelButton;
		Button _undoButton;

		public AppConfigWindow ()
		{
			BuildContent ();
			BindEvents ();

			Title = I18N._ ("Configure Applications");
			Resizable = true;
		}

		~AppConfigWindow()
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
			_addButton.Clicked -= HandleAdd;
			_undoButton.Clicked -= HandleUndo;
			base.Dispose(isDisposing);
		}

		void BindEvents ()
		{
			Closed += HandleClose;
			_okButton.Clicked += HandleOk;
			_cancelButton.Clicked += HandleCancel;
			_addButton.Clicked += HandleAdd;
			_undoButton.Clicked += HandleUndo;
		}

		void HandleUndo (object sender, EventArgs e)
		{
			if (Undo != null) {
				Undo (this, new EventArgs ());
			}
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
			Dispose ();
		}

#endregion

#region IWidget implementation

		void IWidget.Show ()
		{
			Show ();
		}

		void IWidget.Hide ()
		{
			Hide ();
		}

#endregion

#region IWindow implementation

		Image IWindow.Icon {
			set {
				this.Icon = value;
			}
		}

		bool IWindow.Sensitive {
			set {
				this.Sensitive = value;
			}
		}

		void IAppConfigWindow.ClearWidgets ()
		{
			_configTable.Clear ();
		}

		bool IAppConfigWindow.UndoEnabled {
			set { _undoButton.Visible = value; }
		}

		public event EventHandler Closing;

#endregion

#region IAppConfigWindow implementation

		public event EventHandler Save;
		public event EventHandler Add;
		public event EventHandler Undo;

		void IAppConfigWindow.AddAppConfigWidget (IAppConfigWidget widget)
		{
			_configTable.Add ((Widget)widget, 0, _configTable.Children.Count ());
		}

		void IAppConfigWindow.RemoveAppConfigWidget (IAppConfigWidget widget)
		{
			_configTable.Remove ((Widget)widget);
		}

#endregion

		void BuildContent ()
		{
			_configTable = new Table { MinWidth = 300 };
			ScrollView scrollView = new ScrollView (_configTable) {
				ExpandHorizontal = true,
						 ExpandVertical = true,
						 HorizontalScrollPolicy = ScrollPolicy.Never,
						 MinHeight = 300,
						 MinWidth = 300
			};
			HBox buttonBox = new HBox ();
			_addButton = new Button (I18N._ ("Add")) { Image = Icons.Add };
			_okButton = new Button (I18N._ ("Save")) { Image = Icons.Ok };
			_cancelButton = new Button (I18N._ ("Cancel")) {
				Image = Icons.Cancel,
				      Style = ButtonStyle.Flat
			};
			_undoButton = new Button (I18N._ ("Undo")) {
				Image = Icons.Undo,
				      Style = ButtonStyle.Flat,
				      Visible = false
			};
			buttonBox.PackEnd (_okButton);
			buttonBox.PackEnd (_addButton);
			buttonBox.PackStart (_cancelButton);
			buttonBox.PackStart (_undoButton);

			VBox box = new VBox ();
			box.PackStart (scrollView, true, true);
			box.PackEnd (buttonBox);
			Content = box;
		}

		void CallAddNewConfigWidget ()
		{
			if (Add != null) {
				Add (this, new EventArgs ());
			}
		}

		void HandleAdd (object sender, EventArgs e)
		{
			CallAddNewConfigWidget ();
		}
	}
}