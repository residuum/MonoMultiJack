// 
// JackdConfiguration.cs
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
	/// <summary>
	/// jackd configuration
	/// </summary>
	[XmlType ("jackd")]
	public class JackdConfiguration
	{
		/// <summary>
		/// path to jackd executable
		/// </summary>
		[XmlElement ("path")]
		public string Path { get; set; }

		/// <summary>
		/// General oprions for jackd
		/// </summary>
		[XmlElement ("general-options")]
		public string GeneralOptions { get; set; }

		/// <summary>
		/// Driver infrastructure for jackd
		/// </summary>
		[XmlElement ("driver")]
		public string Driver { get; set; }

		/// <summary>
		/// Options for jackd driver
		/// </summary>
		[XmlElement ("driver-options")]
		public string DriverOptions { get; set; }

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
		public JackdConfiguration (string newPath, string newGeneralOptions, string newDriver, string newDriverOptions)
		{			
			Path = newPath;
			GeneralOptions = newGeneralOptions;
			Driver = newDriver;
			DriverOptions = newDriverOptions;
		}

		public JackdConfiguration ()
		{
		}
	}
}