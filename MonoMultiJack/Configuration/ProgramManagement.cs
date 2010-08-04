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

namespace MonoMultiJack.Configuration
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
		/// Path to shell script
		/// </summary>
		private string _shellScriptFile;
		
		/// <summary>
		/// Process ID of program
		/// </summary>
		private string _pid;
		
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
			BuildShellScript();
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
		{
			
		}
		
		/// <summary>
		/// Builds and saves the shell script for starting the program.
		/// </summary>
		private void BuildShellScript()
		{
			StringBuilder bashScript = new StringBuilder();
			bashScript.AppendLine("#!/bin/sh");
			if (_isSingleton)
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
			
			_shellScriptFile = Path.GetTempFileName();
			try
			{				
				File.WriteAllText(_shellScriptFile, bashScript.ToString());
				                  
			}
			catch (Exception ex)
			{
				new IOException("Unable to write to temporary file.", ex);
			}
		}
		
		/// <summary>
		/// Starts the program.
		/// </summary>
		public void StartProgram()
		{
			Process shellStartProcess = new Process ();
			shellStartProcess.StartInfo.FileName = "sh";
			shellStartProcess.StartInfo.Arguments = _shellScriptFile;
			shellStartProcess.StartInfo.RedirectStandardOutput = true;
			shellStartProcess.EnableRaisingEvents = true;
			shellStartProcess.StartInfo.UseShellExecute = false;
			shellStartProcess.OutputDataReceived += HandleStartOutputDataReceived;	
			if (shellStartProcess.Start())
			{
				shellStartProcess.BeginOutputReadLine();
			}
		}

		/// <summary>
		/// Event handler for data received from bash script.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="DataReceivedEventArgs"/>
		/// </param>
		void HandleStartOutputDataReceived (object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				_pid = e.Data;
				HasStarted(this, new EventArgs());
			}
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
			}
		}		
	}
}

