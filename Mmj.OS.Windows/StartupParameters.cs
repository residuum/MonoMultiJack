//
// StartupParameters.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2014-2015 Thomas Mayer
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
using System.IO;

namespace Mmj.OS
{
	public class StartupParameters : IStartupParameters
	{
		public StartupParameters (string[] startupArgs)
		{
			for (int i = 0; i < startupArgs.Length; i++) {
				string argument = startupArgs [i];
				switch (argument) {
				case "/h":
				case "/?":
					ShowHelp = true;
					break;
				case "/j":
					StartWithJackd = true;
					break;
				case "/f":
					StartWithFullScreen = true;
					break;
				default:
					if (argument.StartsWith ("/l=")) {
						LogFile = GetStringParameter (argument);
					} else if (argument.StartsWith ("/c=")) {
						ConfigDirectory = GetStringParameter (argument);
					}
					break;
				}
			}
			if (!string.IsNullOrEmpty (ConfigDirectory)) { 
				return;
			}				
			ConfigDirectory = Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData),
				"MonoMultiJack");
		}

		static string GetStringParameter (string argument)
		{
			return argument.Substring (3);
		}
		#region IStartupParameters implementation
		public bool ShowHelp { get; private set; }

		public bool StartWithJackd { get; private set; }

		public bool StartWithFullScreen { get; private set; }

		public string ConfigDirectory { get; private set; }

		public string LogFile { get; private set; }

		public string GetHelpText ()
		{
			return _helpText;
		}
		#endregion
		static readonly string _helpText = @"**MonoMultJack (MMJ)** aims to be an application for musicians, who regularly have to deal with multiple programs to start and create and maintain audio connections via Jackd.

**Startup Parameters**  
*/h*, */?*: Show this help on startup.  
*/j*: Launches Jackd on startup.  
*/f*: Starts in fullscreen mode.  
*/c=<dir>*: Loads configuration from the specified directory.  
*/l=<file>*: Logs messages and debugging information to the specified file.

**Keyboard Shortcuts**  
*F1*: Show this help.  
*Alt+F4*, *Ctrl+Q*: Quits the program and closes all started applications.  
*Ctrl+C*: Connects the selected inlets / outlets and or clients.  
*Ctrl+D*: Disconnects the selected inlets / outlets and or clients.  
";
	}
}