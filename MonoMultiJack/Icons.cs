﻿// 
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
using Xwt.Drawing;

namespace MonoMultiJack
{
	public static class Icons
	{
		static Assembly _assembly;

		static Assembly Assembly {
			get{
				if (_assembly == null) {
					_assembly = Assembly.GetExecutingAssembly ();
				}
				return _assembly;
			}
		}

		public static Image Program {
			get {
				using (Stream s = Assembly.GetManifestResourceStream("MonoMultiJack.Icons.program.png")){
					return Image.FromStream (s);
				}
			}
		}

		public static Image Ok {
			get {
				using (Stream s = Assembly.GetManifestResourceStream("MonoMultiJack.Icons.ok.png")){
					return Image.FromStream (s).WithSize(Xwt.IconSize.Small);
				}
			}
		}

		public static Image Cancel{
			get {
				using (Stream s = Assembly.GetManifestResourceStream("MonoMultiJack.Icons.cancel.png")){
					return Image.FromStream (s).WithSize(Xwt.IconSize.Small);
				}
			}
		}

		public static Image Connect {
			get {
				using (Stream s = Assembly.GetManifestResourceStream("MonoMultiJack.Icons.connect.png")){
					return Image.FromStream (s).WithSize(Xwt.IconSize.Small);
				}
			}
		}

		public static Image Disconnect {
			get {
				using (Stream s = Assembly.GetManifestResourceStream("MonoMultiJack.Icons.disconnect.png")){
					return Image.FromStream (s).WithSize(Xwt.IconSize.Small);
				}
			}
		}
	}
}