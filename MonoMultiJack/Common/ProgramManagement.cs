// 
// ProcessManagement.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2010 Thomas Mayer
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

namespace MonoMultiJack.Common
{
	/// <summary>
	/// Delegate to signal stopped program.
	/// </summary>
	public delegate void ProgramEventHandler(object sender, EventArgs e);
	
	/// <summary>
	/// Class for management of external programs.
	/// </summary>
	public class ProgramManagement
	{
		/// <summary>
		/// Name of the command to start
		/// </summary>
		private string _commandName;
		
		/// <summary>
		/// Arguments for program
		/// </summary>
		private string _commandArguments;
		
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
		/// Indicates if only one instance is allowed.
		/// </summary>
		private bool _isSingleton;
		
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
		public bool IsRunning
		{
			get
			{
				TestForStillRunning();
				return !string.IsNullOrEmpty(_pid);
			}
		}
		
		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="commandName">
		/// Name of the command.
		/// </param>
		/// <param name="commandArguments">
		/// Arguments for the command
		/// </param>
		/// <param name="isSingleton">
		/// If true, only one instance of the program may start
		/// </param>
		public ProgramManagement (string commandName, string commandArguments, bool isSingleton)
		{
			_commandName = commandName;
			_commandArguments = commandArguments;
			_isSingleton = isSingleton;
			BuildStartScript();
			if (_isSingleton)
			{
				TestForRunningSingleton();
			}
		}
		
		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="commandName">
		/// Name of the command.
		/// </param>
		/// <param name="commandArguments">
		/// Arguments for the command
		/// </param>
		public ProgramManagement(string commandName, string commandArgument) 
			: this(commandName, commandArgument, false)
		{}
		
		/// <summary>
		/// Destructs instance and cleans up temporary files.
		/// </summary>
		~ProgramManagement()
		{
			if (!string.IsNullOrEmpty(_startScriptFile) && File.Exists(_startScriptFile))
			{
				File.Delete(_startScriptFile);
			}
			if (!string.IsNullOrEmpty(_testingScriptFile) && File.Exists(_testingScriptFile))
			{
				File.Delete(_testingScriptFile);
			}
			
		}
		
		/// <summary>
		/// Builds and saves the shell script for starting the program.
		/// </summary>
		private void BuildStartScript()
		{
			StringBuilder bashScript = new StringBuilder();
			bashScript.AppendLine("#!/bin/sh");
			if (_isSingleton && !string.IsNullOrEmpty(_commandName))
			{
				string[] commandPaths = _commandName.Split(Path.DirectorySeparatorChar);
				bashScript.AppendLine("if pgrep " + commandPaths[commandPaths.Length - 1]);
				bashScript.AppendLine("then true");
				bashScript.AppendLine("else");
			}
			bashScript.AppendLine(_commandName + " " + _commandArguments + " >> /dev/null 2>&1&");
			bashScript.AppendLine("echo $!");
			if (_isSingleton)
			{
				bashScript.AppendLine("fi");
			}
			
			_startScriptFile = Path.GetTempFileName();
			try
			{				
				File.WriteAllText(_startScriptFile, bashScript.ToString());				                  
			}
			catch (Exception ex)
			{
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
			
			if (string.IsNullOrEmpty(_testingScriptFile))
			{
				_testingScriptFile = Path.GetTempFileName();
			}
			try
			{				
				File.WriteAllText(_testingScriptFile, bashScript.ToString());				                  
			}
			catch (Exception ex)
			{
				#if DEBUG
				Console.WriteLine (ex.Message);
				#endif
				new IOException("Unable to write to temporary file.", ex);
			}
		}
		
		/// <summary>
		/// Starts the program.
		/// </summary>
		public void StartProgram()
		{
			if (string.IsNullOrEmpty(_startScriptFile) || !File.Exists(_startScriptFile))
			{
				BuildStartScript();
			}
			ExecuteShellScript(_startScriptFile);
			GLib.Timeout.Add(1000, new GLib.TimeoutHandler(IsStillRunning));
		}
		
		/// <summary>
		/// Executes the indicated shell script.
		/// </summary>
		/// <param name="fileName">
		/// A <see cref="System.String"/> holding the path to the shell script.
		/// </param>
		private void ExecuteShellScript(string fileName)
		{
			Process shellStartProcess = new Process ();
			shellStartProcess.StartInfo.FileName = "sh";
			shellStartProcess.StartInfo.Arguments = fileName;
			shellStartProcess.StartInfo.RedirectStandardOutput = true;
			shellStartProcess.EnableRaisingEvents = true;
			shellStartProcess.StartInfo.UseShellExecute = false;
			if (shellStartProcess.Start())
			{
				_pid = shellStartProcess.StandardOutput.ReadToEnd().TrimEnd();
				if (_pid == "0" || string.IsNullOrEmpty(_pid))
				{
					_pid = null;
					HasExited(this, new EventArgs());
				}
				else
				{
					BuildStillRunningScript();
					HasStarted(this, new EventArgs());
				}
				shellStartProcess.WaitForExit();
			}
			shellStartProcess.Dispose();
		}
			

	
		/// <summary>
		/// Stops the program.
		/// </summary>
		public void StopProgram()
		{
			if (IsRunning)
			{
				Process killProgram = new Process();
				killProgram.StartInfo.FileName = "kill";
				killProgram.StartInfo.Arguments = _pid;
				if (killProgram.Start())
				{
					HasExited(this, new EventArgs());
					_pid = null;
				}
				killProgram.WaitForExit();
				killProgram.Dispose();
			}
		}
		
		/// <summary>
		/// Tests, if singleton process is already running.
		/// </summary>
		private void TestForRunningSingleton()
		{
			Process pgrepProgram = new Process();
			pgrepProgram.StartInfo.FileName = "pgrep";
			string[] commandPaths = _commandName.Split(Path.DirectorySeparatorChar);
			pgrepProgram.StartInfo.Arguments = commandPaths[commandPaths.Length - 1];
			pgrepProgram.StartInfo.RedirectStandardOutput = true;
			pgrepProgram.EnableRaisingEvents = true;
			pgrepProgram.StartInfo.UseShellExecute = false;
			if (pgrepProgram.Start())
			{
				_pid = pgrepProgram.StandardOutput.ReadToEnd().TrimEnd();
				if (_pid == "0" || string.IsNullOrEmpty(_pid))
				{
					_pid = null;
				}
				else
				{
					BuildStillRunningScript();
				}
				pgrepProgram.WaitForExit();
			}
			pgrepProgram.Dispose();
		}
		
		/// <summary>
		/// Tests, if process is still running.
		/// </summary>
		private void TestForStillRunning()
		{
			if (!string.IsNullOrEmpty(_testingScriptFile) && File.Exists(_testingScriptFile))
			{
				ExecuteShellScript(_testingScriptFile);
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
			if (!isRunning)
			{
				HasExited(this, new EventArgs());
			}
			return isRunning;
		}
	}
}

