// 
// ProgramManagement.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2012 Thomas Mayer
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
using System.Diagnostics;
using System.Text;
using MonoMultiJack.Configuration;

namespace MonoMultiJack.OS.Linux
{
	/// <summary>
	/// Class for management of external programs.
	/// </summary>
	public class Program : IProgram
	{
		/// <summary>
		/// Name of the command to start
		/// </summary>
		private readonly string _commandName;
		
		/// <summary>
		/// Arguments for program
		/// </summary>
		private readonly string _commandArguments;
		
		/// <summary>
		/// Path to shell script for starting
		/// </summary>
		private string _startScriptFile;
		
		/// <summary>
		/// Path to shell script for testing
		/// </summary>
		private string _testingScriptFile;
		
		/// <summary>
		/// Process ID of program
		/// </summary>
		private string _pid;
				
		/// <summary>
		/// Signals the exit of program.
		/// </summary>
		public event ProgramEventHandler HasExited;
		
		/// <summary>
		/// Signals the start of program
		/// </summary>
		public event ProgramEventHandler HasStarted;
		
		/// <summary>
		/// Returns true if program is running.
		/// </summary>
		public bool IsRunning {
			get {
				TestForStillRunning();
				return !string.IsNullOrEmpty(_pid) && _pid != "0";
			}
		}
		
		public Program(JackdConfiguration jackdConfig)
		{
			_commandName = jackdConfig.Path;
			_commandArguments = jackdConfig.GeneralOptions + " -d " + jackdConfig.Driver + " " + jackdConfig.DriverOptions;
			BuildStartScript(true);
			TestForRunningSingleton();
		}

		public Program(AppConfiguration appConfig)
		{
			if (string.IsNullOrEmpty(appConfig.Command)) {
				return;
			}

			_commandName = appConfig.Command;
			_commandArguments = appConfig.Arguments;
			BuildStartScript(false);
		}
		
		/// <summary>
		/// Destructs instance and cleans up temporary files.
		/// </summary>
		~Program ()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	
		protected virtual void Dispose(bool isDisposing)
		{
			Stop();
			if (!string.IsNullOrEmpty(_startScriptFile) && File.Exists(_startScriptFile)) {
				File.Delete(_startScriptFile);
			}
			if (!string.IsNullOrEmpty(_testingScriptFile) && File.Exists(_testingScriptFile)) {
				File.Delete(_testingScriptFile);
			}
			
		}
		
		/// <summary>
		/// Builds and saves the shell script for starting the program.
		/// </summary>
		private void BuildStartScript(bool isJackd)
		{
			StringBuilder bashScript = new StringBuilder();
			bashScript.AppendLine("#!/bin/sh");
			if (isJackd && !string.IsNullOrEmpty(_commandName)) {
				string[] commandPaths = _commandName.Split(Path.DirectorySeparatorChar);
				bashScript.AppendLine("if pgrep " + commandPaths [commandPaths.Length - 1]);
				bashScript.AppendLine("then true");
				bashScript.AppendLine("else");
			}
			bashScript.AppendLine(_commandName + " " + _commandArguments + " >> /dev/null 2>&1&");
			bashScript.AppendLine("echo $!");
			if (isJackd) {
				bashScript.AppendLine("fi");
			}
			
			_startScriptFile = Path.GetTempFileName();
			try {				
				File.WriteAllText(_startScriptFile, bashScript.ToString());				                  
			} catch (Exception ex) {
				#if DEBUG
				Console.WriteLine (ex.Message);
				#endif
				new IOException("Unable to write to temporary file.", ex);
			}
		}
		
		/// <summary>
		/// Builds and saves the shell script for starting the program.
		/// </summary>
		private void BuildStillRunningScript()
		{
			StringBuilder bashScript = new StringBuilder();
			bashScript.AppendLine("#!/bin/sh");
			bashScript.AppendLine("if [ -e /proc/" + _pid + " ];");
			bashScript.AppendLine("then echo " + _pid);
			bashScript.AppendLine("else echo 0");
			bashScript.AppendLine("fi");
			
			if (string.IsNullOrEmpty(_testingScriptFile)) {
				_testingScriptFile = Path.GetTempFileName();
			}
			try {				
				File.WriteAllText(_testingScriptFile, bashScript.ToString());				                  
			} catch (Exception ex) {
				#if DEBUG
				Console.WriteLine (ex.Message);
				#endif
				new IOException("Unable to write to temporary file.", ex);
			}
		}
		
		/// <summary>
		/// Starts the program.
		/// </summary>
		public void Start()
		{
			ExecuteShellScript(_startScriptFile, true);
			GLib.Timeout.Add(1000, new GLib.TimeoutHandler(IsStillRunning));
		}
		
		/// <summary>
		/// Executes the indicated shell script.
		/// </summary>
		/// <param name="fileName">
		/// A <see cref="System.String"/> holding the path to the shell script.
		/// </param>
		private void ExecuteShellScript(string fileName, bool sendEvents)
		{
			using (Process shellStartProcess = new Process ()) {
				shellStartProcess.StartInfo.FileName = "sh";
				shellStartProcess.StartInfo.Arguments = fileName;
				shellStartProcess.StartInfo.RedirectStandardOutput = true;
				shellStartProcess.EnableRaisingEvents = true;
				shellStartProcess.StartInfo.UseShellExecute = false;
				if (shellStartProcess.Start()) {
					_pid = shellStartProcess.StandardOutput.ReadToEnd().TrimEnd();
					
					if (sendEvents) {
						if (_pid == "0" || string.IsNullOrEmpty(_pid)) {
							_pid = null;
							if (HasExited != null) {
								HasExited(this, new EventArgs());
							}
						} else {
							BuildStillRunningScript();
							if (HasStarted != null) {
								HasStarted(this, new EventArgs());
							}
						}
					}
					shellStartProcess.WaitForExit();
				}
			}
		}
		
		/// <summary>
		/// Stops the program.
		/// </summary>
		public void Stop()
		{
			if (IsRunning) {
				using (Process killProgram = new Process ()) {
					killProgram.StartInfo.FileName = "kill";
					killProgram.StartInfo.Arguments = _pid;
					if (killProgram.Start()) {
						if (HasExited != null){
							HasExited(this, new EventArgs());
						}
						_pid = null;
					}
					killProgram.WaitForExit();
				}
			}
		}
		
		/// <summary>
		/// Tests, if singleton process is already running.
		/// </summary>
		private void TestForRunningSingleton()
		{
            //using (Process pgrepProgram = new Process()) {
            //    pgrepProgram.StartInfo.FileName = "pgrep";
            //    string[] commandPaths = _commandName.Split(Path.DirectorySeparatorChar);
            //    pgrepProgram.StartInfo.Arguments = commandPaths [commandPaths.Length - 1];
            //    pgrepProgram.StartInfo.RedirectStandardOutput = true;
            //    pgrepProgram.EnableRaisingEvents = true;
            //    pgrepProgram.StartInfo.UseShellExecute = false;
            //    if (pgrepProgram.Start()) {
            //        _pid = pgrepProgram.StandardOutput.ReadToEnd().TrimEnd();
            //        if (_pid == "0" || string.IsNullOrEmpty(_pid)) {
            //            _pid = null;
            //        } else {
            //            BuildStillRunningScript();
            //        }
            //        pgrepProgram.WaitForExit();
            //    }
            //}
		}
		
		/// <summary>
		/// Tests, if process is still running.
		/// </summary>
		private void TestForStillRunning()
		{
			if (!string.IsNullOrEmpty(_testingScriptFile) && File.Exists(_testingScriptFile)) {
				ExecuteShellScript(_testingScriptFile, false);
			}
		}
		
		/// <summary>
		/// Tests for still running and invokes the HasExited event, if not.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private bool IsStillRunning()
		{
			bool isRunning = IsRunning;
			if (!isRunning && HasExited != null) {
				HasExited(this, new EventArgs());
			}
			return isRunning;
		}
	}
}