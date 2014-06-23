//
// AboutWindow.cs
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
using System.IO;
using Xwt;
using Xwt.Drawing;

namespace MonoMultiJack.Forms
{
	public class AboutWindow : Window, IAboutWindow
	{
		string _nameText;
		string _versionText;
		Label _programName;
		Label _copyright;
		Label _comments;
		LinkLabel _website;
		Label _authors;
		Label _license;

		public AboutWindow ()
		{
			BuildWindowContent ();
			Closed += HandleClose;
		}

		void BuildWindowContent ()
		{
			VBox mainContent = new VBox ();
			_programName = new Label ();
			_programName.Font = _programName.Font.WithScaledSize (1.5).WithWeight(FontWeight.Bold);
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


			this.Content = mainContent;
		}

		void SetProgramName ()
		{
			_programName.Text = string.Format("{0} {1}", _nameText, _versionText);
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
		void MonoMultiJack.Widgets.IWidget.Show ()
		{
			this.Show ();
		}

		void MonoMultiJack.Widgets.IWidget.Hide ()
		{
			this.Hide ();
		}
		#endregion
		public event EventHandler Closing;
		#region IAboutWindow implementation
		string IAboutWindow.ProgramName {
			set {
				this.Title = string.Format ("Info about {0}", value);
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
				_website.Uri = new Uri(value);
			}
		}

		string[] IAboutWindow.Authors {
			set {
				//TODO
				//this.Authors = value;
			}
		}

		string IAboutWindow.License {
			set {
				//TODO
				//this.License = value;
			}
		}

		string IWindow.IconPath {
			set {
				if (File.Exists (value)) {
					this.Icon = Image.FromFile (value);
				}
			}
		}

		public bool Sensitive { set; private get; }
		#endregion
	}
}

