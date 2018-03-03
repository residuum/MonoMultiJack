//
// AboutWindow.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2009-2015 Thomas Mayer
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
using Mmj.OS;
using Xwt;
using Xwt.Drawing;

namespace Mmj.Views.Windows
{
	public partial class AboutWindow : Window, IAboutWindow
	{
		string _nameText;
		string _versionText;
		Label _programName;
		Label _copyright;
		Label _comments;
		LinkLabel _website;
		string _authors;
		string _license;

		public AboutWindow ()
		{
			BuildContent ();
			Closed += HandleClose;
		}

		~AboutWindow ()
		{
			Dispose (false);
		}

		public new void Dispose ()
		{
			Dispose (true);
		}

		protected new void Dispose (bool isDisposing)
		{
			Closed -= HandleClose;
			base.Dispose (isDisposing);
		}

		void BuildContent ()
		{
			VBox mainContent = new VBox ();
			_programName = new Label ();
			_programName.Font = _programName.Font.WithScaledSize (1.5).WithWeight (FontWeight.Bold);
			mainContent.PackStart (_programName);
			_comments = new Label ();
			_comments.Wrap = WrapMode.Word;
			mainContent.PackStart (_comments);
			_copyright = new Label ();
			_copyright.Font = _copyright.Font.WithScaledSize (0.8);
			mainContent.PackStart (_copyright);
			_website = new LinkLabel ();
			_website.Font = _website.Font.WithScaledSize (0.8);
			mainContent.PackStart (_website);
			HBox buttonRow = new HBox ();
			Button authors = new Button {
				Label = I18N._ ("Authors"),
				Image = Icons.Info
			};
			authors.Clicked += (sender, args) => ShowAuthors ();
			buttonRow.PackStart (authors);
			Button license = new Button {
				Label = I18N._ ("License"),
				Image = Icons.Info
			};
			license.Clicked += (sender, args) => ShowLicense ();
			buttonRow.PackStart (license);
			Button ok = new Button { Label = I18N._ ("Close"), Image = Icons.Ok };
			ok.Clicked += (sender, args) => Close ();
			buttonRow.PackEnd (ok);
			mainContent.PackEnd (buttonRow);
			Content = mainContent;
		}

		void SetProgramName ()
		{
			_programName.Text = string.Format ("{0} {1}", _nameText, _versionText);
		}

		void ShowLicense ()
		{
			LicenseDialog dialog = new LicenseDialog (_license);
			dialog.Icon = Icon;
			this.Sensitive = false;
			dialog.Show ();
			dialog.Closed += (sender, e) => {
				this.Sensitive = true;
			};
		}

		void ShowAuthors ()
		{
			this.ShowInfoMessage (_authors);
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

		#region IAboutWindow implementation

		string IAboutWindow.ProgramName {
			set {
				Title = string.Format (I18N._ ("Info about {0}", value));
				_nameText = value;
				SetProgramName ();
			}
		}

		string IAboutWindow.Copyright {
			set {
				_copyright.Text = value;
			}
		}

		string IAboutWindow.Comments {
			set {
				_comments.Text = value;
			}
		}

		string IAboutWindow.Version {
			set {
				_versionText = value;
				SetProgramName ();
			}
		}

		string IAboutWindow.Website {
			set {
				_website.Text = value;
				_website.Uri = new Uri (value);
			}
		}

		string[] IAboutWindow.Authors {
			set {
				_authors = string.Join ("\n", value);
			}
		}

		string IAboutWindow.License {
			set {
				_license = value;
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
