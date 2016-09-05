// 
// Colors.cs
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
using Xwt.Drawing;

namespace Mmj.Views.Widgets
{
	static class Colors
	{
		public static Color GetColor (this int index, Color bg)
		{
			switch (index % 13) {
				case 0:
					return new Color (0, 0, 0).FromBackground (bg);
				case 1:
					return new Color (0.3, 0, 0).FromBackground (bg);
				case 2:
					return new Color (0, 0.3, 0).FromBackground (bg);
				case 3:
					return new Color (0, 0, 0.3).FromBackground (bg);
				case 4:
					return new Color (0.3, 0.3, 0).FromBackground (bg);
				case 5:
					return new Color (0, 0.3, 0.3).FromBackground (bg);
				case 6:
					return new Color (0.3, 0, 0.3).FromBackground (bg);
				case 7:
					return new Color (0.6, 0.3, 0).FromBackground (bg);
				case 8:
					return new Color (0, 0.6, 0.3).FromBackground (bg);
				case 9:
					return new Color (0.3, 0, 0.6).FromBackground (bg);
				case 10:
					return new Color (0.3, 0.6, 0).FromBackground (bg);
				case 11:
					return new Color (0, 0.3, 0.6).FromBackground (bg);
				case 12:
					return new Color (0.6, 0, 0.3).FromBackground (bg);

			}
			return new Color (0, 0, 0).FromBackground (bg);
		}

		static Color FromBackground (this Color baseColor, Color bg)
		{
			return new Color ((baseColor.Red - bg.Red).Wrap (),
					(baseColor.Blue - bg.Blue).Wrap (),
					(baseColor.Green - bg.Green).Wrap ());
		}

		static double Wrap (this double value)
		{
			while (value < 0) {
				value++;
			}
			while (value > 1) {
				value--;
			}
			return value;
		}
	}
}
