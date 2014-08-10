// 
// AppConfiguration.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2013 Thomas Mayer
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
	/// <summary>
	/// Configuration of an application
	/// </summary>
	public struct AppConfiguration
	{
		/// <summary>
		/// name of the application
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// command to launch the application
		/// </summary>
		public string Command { get; private set; }

		/// <summary>
		/// Gets the arguments.
		/// </summary>
		/// <value>
		/// The arguments.
		/// </value>
		public string Arguments { get; private set; }

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="newName">A <see cref="System.String" /> indicating name of application</param>
		/// <param name="newCommand">A <see cref="System.String" /> indicating command to lauch the application</param>
		/// <param name="newArguments">The new arguments.</param>
		public AppConfiguration (string newName, string newCommand, string newArguments) : this()
		{
			Name = newName;
			Command = newCommand;
			Arguments = newArguments;
		}
	}
}