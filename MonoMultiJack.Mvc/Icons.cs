// 
// Icons.cs
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
using System.IO;
using System.Reflection;
using Xwt;
using Xwt.Drawing;

namespace MonoMultiJack
{
	public static class Icons
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
			get { return LoadImage ("MonoMultiJack.Icons.program.png"); }
		}

		private static Image LoadImage (string imageNamespace)
		{
			using (Stream s = Assembly.GetManifestResourceStream(imageNamespace)) {
				return Image.FromStream (s);
			}
		}

		public static Image Ok {
			get { return LoadImage ("MonoMultiJack.Icons.ok.png").WithSize (IconSize.Small); }
		}

		public static Image Cancel {
			get { return LoadImage ("MonoMultiJack.Icons.cancel.png").WithSize (IconSize.Small); }
		}

		public static Image Connect {
			get { return LoadImage ("MonoMultiJack.Icons.connect.png").WithSize (IconSize.Small); }
		}

		public static Image Disconnect {
			get { return LoadImage ("MonoMultiJack.Icons.disconnect.png").WithSize (IconSize.Small); }
		}

		public static Image Start {
			get { return LoadImage ("MonoMultiJack.Icons.start.png").WithSize (IconSize.Small); }
		}

		public static Image Stop {
			get { return LoadImage ("MonoMultiJack.Icons.stop.png").WithSize (IconSize.Small); }
		}

		public static Image Delete {
			get { return LoadImage ("MonoMultiJack.Icons.remove.png").WithSize (IconSize.Small); }
		}

		public static Image Add {
			get { return LoadImage ("MonoMultiJack.Icons.add.png").WithSize (IconSize.Small); }
		}

		public static Image Help {
			get { return LoadImage ("MonoMultiJack.Icons.help.png").WithSize (IconSize.Small); }
		}

		public static Image Info {
			get { return LoadImage ("MonoMultiJack.Icons.info.png").WithSize (IconSize.Small); }
		}

		public static Image Warning {
			get { return LoadImage ("MonoMultiJack.Icons.warning.png").WithSize (IconSize.Small); }
		}
	}
}
