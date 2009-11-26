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

namespace MonoMultiJack.Configuration
{
	/// <summary>
	/// jackd configuration
	/// </summary>
	public class JackdConfiguration
	{
		/// <summary>
		/// path to jackd executable
		/// </summary>
		public string Path {get; protected set;}
		
		/// <summary>
		/// driver infrastructure for jacdk
		/// </summary>
		public string Driver {get; protected set;}
		
		/// <summary>
		/// audiorate to run jackd at
		/// </summary>
		public string Audiorate {get; protected set;}
		
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="newPath">
		/// A <see cref="System.String"/> indicating path to jackd executable
		/// </param>
		/// <param name="newDriver">
		/// A <see cref="System.String"/> indicating driver infrastructure for jackd
		/// </param>
		/// <param name="newAudiorate">
		/// A <see cref="System.String"/> indicating audiorate for jackd
		/// </param>
		public JackdConfiguration (string newPath, string newDriver, string newAudiorate)
		{			
			Path = newPath;
			Driver = newDriver;
			Audiorate = newAudiorate;
		}
		
		public JackdConfiguration () : this(String.Empty, String.Empty, String.Empty)
		{}
	}
}
