// 
// JackdConfiguration.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009 Thomas Mayer
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

namespace MonoMultiJack
{
	/// <summary>
	/// Configuration of Jackd
	/// </summary>
	public class JackdConfiguration
	{
		//// <value>
		/// startup path for jackd
		/// </value>
		public string path {get; protected set;}
		
		//// <value>
		/// driver infrastructure for jackd
		/// </value>
		public string driver {get; protected set;}
		
		//// <value>
		/// audiorate
		/// </value>
		public string audiorate {get; protected set;}

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="newPath">
		/// A <see cref="System.String"/> startup path
		/// </param>
		/// <param name="newDriver">
		/// A <see cref="System.String"/> driver infrastructure
		/// </param>
		/// <param name="newAudiorate">
		/// A <see cref="System.String"/> audiorate
		/// </param>
		public JackdConfiguration (string newPath, string newDriver, string newAudiorate)
		{
			this.path = newPath;
			this.driver = newDriver;
			this.audiorate = newAudiorate;
		}
	}
}
