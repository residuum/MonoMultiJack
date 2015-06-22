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
	public class StartupParameters: IStartupParameters
	{
		public StartupParameters (string[] startupArgs)
		{
			for (int i = 0; i < startupArgs.Length; i++) {
				string argument = startupArgs [i];
				switch (argument) {
				case "-h":
				case "--help":
					ShowHelp = true;
					break;
				case "-j":
				case "--jack":
					StartWithJackd = true;
					break;
				case "-f":
				case "--fullscreen":
					StartWithFullScreen = true;
					break;
				case "-l":
				case "--log":
					LogFile = GetStringParameter (startupArgs, true, ref i);
					break;
				case "-c":
				case "--config":
					ConfigDirectory = GetStringParameter (startupArgs, true, ref i);
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

		static string GetStringParameter (string[] startupArgs, bool isPathParameter, ref int currentPosition)
		{
			if (currentPosition < startupArgs.Length - 1) {
				string configString = startupArgs [++currentPosition];
				if (!isPathParameter || !configString.StartsWith ("~")) {
					return configString;
				}
				return Environment.GetFolderPath (Environment.SpecialFolder.UserProfile) + configString.Substring (1);
			}
			return null;
		}

		#region IStartupParameters implementation

		public bool ShowHelp { get; private set; }

		public bool StartWithJackd { get; private set; }

		public bool StartWithFullScreen { get; private set; }

		public string ConfigDirectory { get; private set; }

		public string LogFile { get; private set; }

		public string GetHelpText ()
		{
			return HelpText;
		}

		#endregion

		static readonly string HelpText = I18N._ ("**MonoMultJack (MMJ)** aims to be an application for musicians, who regularly have to deal with multiple programs to start and create and maintain audio connections via Jackd.") + @"


**" + I18N._ ("Startup Parameters") + @"**  
*-h*, *--help*: " + I18N._ ("Show this help on startup.") + @"  
*-j*, *--jack*: " + I18N._ ("Launches Jackd on startup.") + @"  
*-f*, *--fullscreen*: " + I18N._ ("Starts in fullscreen mode.") + @"  
*-c <dir>*, *--config <dir>*: " + I18N._ ("Loads configuration from the specified directory.") + @"  
*-l <file>*, *--log <file>*: " + I18N._ ("Logs messages and debugging information to the specified file.") + @"  

**" + I18N._ ("Keyboard Shortcuts") + @"**  
*F1*: " + I18N._ ("Show this help.") + @"  
*Alt+F4*, *Ctrl+Q*: " + I18N._ ("Quits the program and closes all started applications.") + @"  
*Ctrl+C*: " + I18N._ ("Connects the selected inlets / outlets and or clients.") + @"  
*Ctrl+D*: " + I18N._ ("Disconnects the selected inlets / outlets and or clients.") + @"  
";
	}
}
