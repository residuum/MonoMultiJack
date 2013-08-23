// 
// WindowConfiguration.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2012 Thomas Mayer
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
namespace MonoMultiJack.Configuration
{
	public struct WindowConfiguration
	{
		/// <summary>
		/// Gets the X position.
		/// </summary>
		/// <value>
		/// The X position.
		/// </value>
		public int XPosition { get; private set; }

		/// <summary>
		/// Gets the Y position.
		/// </summary>
		/// <value>
		/// The Y position.
		/// </value>
		public int YPosition { get; private set; }

		/// <summary>
		/// Gets the size of the X.
		/// </summary>
		/// <value>
		/// The size of the X.
		/// </value>
		public int XSize { get; private set; }

		/// <summary>
		/// Gets the size of the Y.
		/// </summary>
		/// <value>
		/// The size of the Y.
		/// </value>
		public int YSize { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoMultiJack.BusinessLogic.WindowConfiguration"/> struct.
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
		public WindowConfiguration(int xPosition, int yPosition, int xSize, int ySize) : this()
		{
			XPosition = xPosition;
			YPosition = yPosition;
			XSize = xSize;
			YSize = ySize;
		}
	}
}

