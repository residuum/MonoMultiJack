//
// AboutWindow.LicenseDialog.cs
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

using Xwt;
using Mmj.OS;
using Xwt.Formats;


namespace Mmj.Views.Windows
{
	public partial class AboutWindow
	{
		private class LicenseDialog : Window
		{
			public LicenseDialog (string license)
			{
				BuildContent (license);
			}

			void BuildContent (string license)
			{
				double textWidth = 480;
				double textHeight = 480;
				VBox mainContent = new VBox ();

				RichTextView textView = new RichTextView ();
				textView.LoadText (license, TextFormat.Markdown);
				textView.MinWidth = textWidth;
				ScrollView scroller = new ScrollView (textView);
				scroller.HorizontalScrollPolicy = ScrollPolicy.Never;
				scroller.VerticalScrollPolicy = ScrollPolicy.Automatic;
				scroller.MinHeight = textHeight;
				scroller.MinWidth = textWidth + textView.MarginLeft + textView.MarginRight + 20;
				mainContent.PackStart (scroller);

				HBox buttonRow = new HBox ();
				Button ok = new Button { Label = I18N._ ("Close"), Image = Icons.Ok };
				ok.Clicked += (sender, args) => Close ();
				buttonRow.PackEnd (ok);
				mainContent.PackEnd (buttonRow);

				Content = mainContent;
			}

		}
	}
}