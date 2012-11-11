//
// InfoMessage.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2012 Thomas Mayer
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
using Gtk;
using System.IO;
using Gdk;

namespace MonoMultiJack.Forms
{
	public class InfoWindow : MessageDialog, IInfoWindow
	{

		public InfoWindow()
		{
			this.MessageType = MessageType.Info;
			this.Close += HandleClose;
			this.Response += HandleClose;
			this.AddButton("Close", ResponseType.Close);
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
		void MonoMultiJack.Widgets.IWidget.Show()
		{
			this.Show();
		}

		void MonoMultiJack.Widgets.IWidget.Destroy()
		{
			this.Destroy();
		}

		void MonoMultiJack.Widgets.IWidget.Hide()
		{
			this.Hide();
		}
		#endregion

		#region IWindow implementation
		public event EventHandler Closing;

		string IWindow.IconPath {
			set {				
				if (File.Exists(value)) {
					this.Icon = new Pixbuf(value);
				}
			}
		}

		bool IWindow.Sensitive {
			set{
				this.Sensitive = value;
			}
		}
		#endregion

		#region IInfoMessage implementation
		string IInfoWindow.Message {
			set {
				this.Text = value;
			}
		}
		#endregion
	}
}
