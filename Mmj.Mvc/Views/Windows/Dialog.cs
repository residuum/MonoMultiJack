//
// Dialog.cs
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

using Mmj.OS;
using Xwt;
using System.IO;

namespace Mmj.Views.Windows
{
	public static class Dialog
	{
		public static void ShowErrorMessage (this IWindow window, string message)
		{
			MessageDialog.ShowError ((Window)window, I18N._ (message));
		}

		public static void ShowInfoMessage (this IWindow window, string message)
		{
			MessageDialog.ShowMessage ((Window)window, I18N._ (message));
		}

		public static string OpenFileDialog (this IWindow window, string folder, string title, string extension = null)
		{
			window.Sensitive = false;
			FileDialog dialog = new OpenFileDialog (I18N._ (title));
			dialog.CurrentFolder = folder;
			if (extension != null) {
				dialog.Filters.Add (new FileDialogFilter ("." + extension, "*\\." + extension));
				dialog.Filters.Add (new FileDialogFilter (I18N._("all"), "*"));
			}
			if (dialog.Run ()) {
				window.Sensitive = true;
				return Path.Combine (dialog.CurrentFolder, dialog.FileName);
			}
			window.Sensitive = true;
			return null;
		}

		public static string SaveFileDialog (this IWindow window, string folder, string title, string extension = null)
		{
			window.Sensitive = false;
			FileDialog dialog = new SaveFileDialog (I18N._ (title));
			dialog.CurrentFolder = folder;
			if (extension != null) {
				dialog.Filters.Add (new FileDialogFilter ("." + extension, "*\\." + extension));
			}
			if (dialog.Run ()) {
				window.Sensitive = true;
				string fileName = dialog.FileName;
				if (extension != null && !fileName.EndsWith ("." + extension)) {
					fileName += "." + extension;
				}
				return Path.Combine (dialog.CurrentFolder, fileName);
			}
			window.Sensitive = true;
			return null;
		}
	}
}
