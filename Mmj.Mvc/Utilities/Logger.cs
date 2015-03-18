//
// Logger.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2015 Thomas Mayer
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
using Mmj.OS;
using Mmj.ConnectionWrapper;

namespace Mmj.Utilities
{
	static class Logger
	{
		static ILogger _logging;

		static ILogger GetLogging ()
		{
			return _logging ?? (_logging = DependencyResolver.GetImplementation<ILogger> ("ILogger"));
		}

		public static void LogException (Exception ex)
		{
			GetLogging ().LogException (ex);
		}

		public static void SetLogFile (string file)
		{
			GetLogging ().SetLogFile (file);
		}

		public static void LogMessage (string message, LogLevel level)
		{
			GetLogging ().LogMessage (message, level);
		}

		public static void LogMessage (ConnectionEventArgs args)
		{
			GetLogging ().LogConnectionWrapper (args);
		}
	}
}

