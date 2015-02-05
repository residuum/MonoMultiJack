//
// MessageDialog.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2015 Thomas Mayer
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
using Xwt.Formats;

namespace MonoMultiJack.Windows
{
	public static class Dialog
	{
		public static void ShowErrorMessage (string message)
		{
			//MessageDialog.ShowError (message);
			DisplayMessage (message, Icons.Warning);
		}

		public static void ShowInfoMessage (string message)
		{
			//MessageDialog.ShowMessage (message);
			DisplayMessage (message, Icons.Info);
		}

		static void DisplayMessage(string message, Image icon){
			Window window = new Window ();
			window.Icon = icon;
			VBox box = new VBox ();
			box.PackStart (BuildMessage (message, icon));
			HBox buttonBox = new HBox();
			Button button = new Button{Label = "OK"};
			button.Clicked += (sender, e) => {
				window.Close();
				window.Dispose();
			};
			buttonBox.PackEnd (button);
			box.PackStart (buttonBox);
			window.Content = box;
			window.ShowInTaskbar = false;
			window.Show ();
			window.Present ();
		}

		static Widget BuildMessage(string message, Image icon){
			HBox messageBox = new HBox ();
			ImageView iconView = new ImageView (icon.WithSize (IconSize.Medium));
			iconView.VerticalPlacement = WidgetPlacement.Start;
			messageBox.PackStart(iconView);
			RichTextView textView = new RichTextView ();
			textView.LoadText (message, TextFormat.Markdown);
			textView.MinWidth = 200;
			textView.WidthRequest = 400;
			messageBox.PackStart (textView);
			return messageBox;
		}
	}
}