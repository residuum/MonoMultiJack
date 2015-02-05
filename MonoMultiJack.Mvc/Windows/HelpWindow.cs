//
// HelpWindow.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2014 Thomas Mayer
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
using Xwt;
using Xwt.Drawing;

namespace MonoMultiJack.Windows
{
	public class HelpWindow : Window, IHelpWindow
	{
		readonly RichTextView _messageDisplay;

		public HelpWindow ()
		{
			VBox mainContent = new VBox ();			
			_messageDisplay = new RichTextView { WidthRequest = 400, HeightRequest = 300 };
		    mainContent.PackStart (_messageDisplay);
			HBox buttonRow = new HBox ();
			Button ok = new Button { Label = "Close", Image = Icons.Ok };
			ok.Clicked += (sender, args) => Close ();
			buttonRow.PackEnd (ok);
			mainContent.PackEnd (buttonRow);
			Content = mainContent;
			Width = 400;
		}
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			Dispose ();
		}
		#endregion
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
		public event EventHandler Closing;
		#region IHelpWindow implementation
		string IHelpWindow.ProgramName {
			set {
				Title = string.Format ("Help for {0}", value);
			}
		}

		string IHelpWindow.HelpContent {
			set {
				_messageDisplay.LoadText (value, Xwt.Formats.TextFormat.Markdown);
			}
		}

		Image IWindow.Icon {
			set {
				Icon = value;
			}
		}
		#endregion
	}
}

