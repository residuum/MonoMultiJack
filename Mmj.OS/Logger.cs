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
using NLog.Config;
using NLog.Targets;
using NLog;
using Mmj.ConnectionWrapper;

namespace Mmj.OS
{
	public class Logger : ILogger
	{
		NLog.Logger _logger;

		#region ILogger implementation
		void ILogger.LogConnectionWrapper (ConnectionEventArgs args)
		{
			((ILogger)this).LogMessage (string.Format ("({0}, {1}), {2}", args.MessageType, args.ChangeType, args.Message), LogLevel.Info);
		}

		void ILogger.SetLogFile (string logFile)
		{
			if (string.IsNullOrEmpty (logFile)) {
				LogManager.DisableLogging ();
				_logger = null;
				return;
			}
			_logger = GetLogger (logFile);
		}

		void ILogger.LogException (Exception ex)
		{
			((ILogger)this).LogMessage (ex.Message, LogLevel.Error);
		}

		void ILogger.LogMessage (string message, LogLevel level)
		{
			#if DEBUG
			Console.WriteLine ("{0:yyyy-MM-dd HH:mm:ss.fff} {1} {2}", DateTime.Now, level, message);
			#else
			if (level <= LogLevel.Debug){
				return;
			}
			#endif
			if (_logger == null) {
				return;
			}
			switch (level) {
			case LogLevel.Debug:
				_logger.Debug (message);
				break;
			case LogLevel.Info:
				_logger.Info (message);
				break;
			case LogLevel.Error:
				_logger.Error (message);
				break;
			}
		}
		#endregion
		NLog.Logger GetLogger (string logFile)
		{
			LoggingConfiguration config = new LoggingConfiguration ();
			FileTarget target = new FileTarget ();
			config.AddTarget ("file", target);
			target.FileName = logFile;
			target.Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff}\t${level}\t${message}";
			LoggingRule rule = new LoggingRule ("*", NLog.LogLevel.Debug, target);
			config.LoggingRules.Add (rule);
			LogManager.Configuration = config;
			return LogManager.GetLogger ("MonoMultiJack");
		}
	}
}

