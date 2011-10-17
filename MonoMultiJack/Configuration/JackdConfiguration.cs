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
	public struct JackdConfiguration
	{
		/// <summary>
		/// path to jackd executable
		/// </summary>
		public string Path {get; private set;}
		
		/// <summary>
		/// General oprions for jackd
		/// </summary>
		public string GeneralOptions {get;private set;}
		
		/// <summary>
		/// Driver infrastructure for jackd
		/// </summary>
		public string Driver {get; private set;}
		
		/// <summary>
		/// Options for jackd driver
		/// </summary>
		public string DriverOptions {get; private set;}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="newPath">
		/// A <see cref="System.String"/> indicating the path to jackd executable
		/// </param>
		/// <param name="newGeneralOptions">
		/// A <see cref="System.String"/> indicating the new general options
		/// </param>
		/// <param name="newDriver">
		/// A <see cref="System.String"/> indicating the driver infrastructure
		/// </param>
		/// <param name="newDriverOptions">
		/// A <see cref="System.String"/> indicating the new driver options
		/// </param>
		public JackdConfiguration (string newPath, string newGeneralOptions, string newDriver, string newDriverOptions): this()
		{			
			Path = newPath;
			GeneralOptions = newGeneralOptions;
			Driver = newDriver;
			DriverOptions = newDriverOptions;
		}
	}
}
