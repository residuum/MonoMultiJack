//
// WidgetExtensions.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xwt;

namespace Mmj.Views
{
	static class WidgetExtensions
	{
		public static void FileSelector (this TextEntry entry)
		{
			entry.GotFocus += ShowFileDialog;
		}

		static void ShowFileDialog (object sender, EventArgs eventArgs)
		{
			TextEntry entry = sender as TextEntry;
			if (entry == null) {
				return;
			}
			string filePath = entry.Text;
			string directory = Environment.GetFolderPath (Environment.SpecialFolder.ProgramFiles);
			string fileName = "";
			if (!string.IsNullOrEmpty (filePath)) {
				FileInfo info = new FileInfo (filePath);
				directory = info.DirectoryName;
				fileName = info.Name;
			}
			FileDialog dialog = new OpenFileDialog {
				CurrentFolder = directory,
					      InitialFileName = fileName
			};
			if (dialog.Run ()) {
				entry.Text = Path.Combine (dialog.CurrentFolder, dialog.FileName);
			}
		}

		public static string CreateWidgetName (this string name)
		{
			return Regex.Replace (name, @"[^a-zA-Z0-9]",
					m => {
					byte[] bytes = Encoding.UTF8.GetBytes (m.Value);
					IEnumerable<string> hexs = bytes.Select (b => string.Format ("_{0:x2}", b));
					return string.Concat (hexs);
					});
		}
	}
}
