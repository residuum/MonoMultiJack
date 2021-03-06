// 
// Icons.cs
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
using System.IO;
using System.Reflection;
using Xwt;
using Xwt.Drawing;

namespace Mmj.Views
{
	public class Icons
	{
		static Assembly _assembly;

		static Assembly Assembly {
			get {
				if (_assembly == null) {
					_assembly = Assembly.GetExecutingAssembly ();
				}
				return _assembly;
			}
		}

		public static Image Program {
			get { return LoadImage ("Mmj.Views.Icons.program.png"); }
		}

		static Image LoadImage (string imageNamespace)
		{
			using (Stream s = Assembly.GetManifestResourceStream (imageNamespace)) {
				return Image.FromStream (s);
			}
		}

		static Image LoadOk ()
		{
			return LoadImage ("Mmj.Views.Icons.ok.png");
		}

		static Image LoadCancel ()
		{
			return LoadImage ("Mmj.Views.Icons.cancel.png");
		}

		static Image LoadAdd ()
		{
			return LoadImage ("Mmj.Views.Icons.add.png");
		}

		static Image LoadHelp ()
		{
			return LoadImage ("Mmj.Views.Icons.help.png");
		}

		static Image LoadInfo ()
		{
			return LoadImage ("Mmj.Views.Icons.info.png");
		}

		static Image LoadWarning ()
		{
			return LoadImage ("Mmj.Views.Icons.warning.png");
		}

		static Image LoadDelete ()
		{
			return LoadImage ("Mmj.Views.Icons.remove.png");
		}

		static Image LoadUndo ()
		{
			return LoadImage ("Mmj.Views.Icons.undo.png");
		}

		static Image LoadUp ()
		{
			return LoadImage ("Mmj.Views.Icons.up.png");
		}

		static Image LoadDown ()
		{
			return LoadImage ("Mmj.Views.Icons.down.png");
		}

		public static Image Connect {
			get { return LoadImage ("Mmj.Views.Icons.connect.png").WithSize (IconSize.Small); }
		}

		public static Image Disconnect {
			get { return LoadImage ("Mmj.Views.Icons.disconnect.png").WithSize (IconSize.Small); }
		}

		public static Image Start {
			get { return LoadImage ("Mmj.Views.Icons.start.png").WithSize (IconSize.Small); }
		}

		public static Image Stop {
			get { return LoadImage ("Mmj.Views.Icons.stop.png").WithSize (IconSize.Small); }
		}

		public static Image Ok {
			get { return LoadOk ().WithSize (IconSize.Small); }
		}

		public static Image Cancel {
			get { return LoadCancel ().WithSize (IconSize.Small); }
		}

		public static Image Delete {
			get { return LoadDelete ().WithSize (IconSize.Small); }
		}

		public static Image Add {
			get { return LoadAdd ().WithSize (IconSize.Small); }
		}

		public static Image Help {
			get { return LoadHelp ().WithSize (IconSize.Small); }
		}

		public static Image Info {
			get { return LoadInfo ().WithSize (IconSize.Small); }
		}

		public static Image Warning {
			get { return LoadWarning ().WithSize (IconSize.Small); }
		}

		public static Image Undo {
			get { return LoadUndo ().WithSize (IconSize.Small); }
		}

		public static Image Up {
			get { return LoadUp ().WithSize (IconSize.Small); }
		}

		public static Image Down {
			get { return LoadDown ().WithSize (IconSize.Small); }
		}
	}
}
