// 
// WindowConfiguration.cs
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

using System.Xml.Serialization;

namespace Mmj.FileOperations.Configuration
{
	[XmlType ("window")]
	public class WindowConfiguration
	{
		/// <summary>
		/// Gets the X position.
		/// </summary>
		/// <value>
		/// The X position.
		/// </value>
		[XmlElement ("x-position")]
		public double XPosition { get; set; }

		/// <summary>
		/// Gets the Y position.
		/// </summary>
		/// <value>
		/// The Y position.
		/// </value>
		[XmlElement ("y-position")]
		public double YPosition { get; set; }

		/// <summary>
		/// Gets the size of the X.
		/// </summary>
		/// <value>
		/// The size of the X.
		/// </value>
		[XmlElement ("x-size")]
		public double Width { get; set; }

		/// <summary>
		/// Gets the size of the Y.
		/// </summary>
		/// <value>
		/// The size of the Y.
		/// </value>
		[XmlElement ("y-size")]
		public double Height { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WindowConfiguration"/> struct.
		/// </summary>
		/// <param name='xPosition'>
		/// X position.
		/// </param>
		/// <param name='yPosition'>
		/// Y position.
		/// </param>
		/// <param name='xSize'>
		/// X size.
		/// </param>
		/// <param name='ySize'>
		/// Y size.
		/// </param>
		public WindowConfiguration (double xPosition, double yPosition, double xSize, double ySize)
		{
			XPosition = xPosition;
			YPosition = yPosition;
			Width = xSize;
			Height = ySize;
		}

		public WindowConfiguration ()
		{
		}
	}
}

