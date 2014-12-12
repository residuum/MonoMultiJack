//
// StartupParameters.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2014 Thomas Mayer
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

namespace MonoMultiJack.OS.Linux
{
	public class StartupParameters: IStartupParameters
	{
		public StartupParameters (string[] startupArgs)
		{
			ConfigDirectory = Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData),
				"MonoMultiJack");
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
					LogFile = GetStringParameter (startupArgs, ref i);
					break;
				case "-c":
				case "--config":
					ConfigDirectory = GetStringParameter (startupArgs, ref i);
					if (ConfigDirectory.StartsWith ("~")) {
						ConfigDirectory = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile) + ConfigDirectory.Substring (1);
					}
					break;
				}
			}
		}

		static string GetStringParameter (string[] startupArgs, ref int currentPosition)
		{
			if (currentPosition < startupArgs.Length - 1) {
				return startupArgs [++currentPosition];
			}
			return null;
		}
		#region IStartupParameters implementation
		public bool ShowHelp { get; private set; }

		public bool StartWithJackd { get; private set; }

		public bool StartWithFullScreen { get; private set; }

		public string ConfigDirectory { get; private set; }

		public string LogFile { get; private set; }
		#endregion
	}
}

