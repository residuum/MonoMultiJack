// 
// Program.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2014 Thomas Mayer
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mmj.FileOperations.Configuration;

namespace Mmj.OS
{
	public class Program : IProgram
	{
		readonly string _commandName;
		readonly string _commandArguments;
		readonly BackgroundWorker _worker;
		Process _process;

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
				if (_process == null) {
					return false;
				}
				try {
					return !_process.HasExited;
				} catch (InvalidOperationException) {
					return false;
				}
			}
		}

		public Program (JackdConfiguration jackdConfig)
		{
			_worker = new BackgroundWorker { WorkerSupportsCancellation = true };
			_worker.DoWork += Worker_Work;

			_commandName = jackdConfig.Path;
			_commandArguments = jackdConfig.GeneralOptions + " -d " + jackdConfig.Driver + " " + jackdConfig.DriverOptions;
			TestForRunningSingleton ();
		}

		void Worker_Work (object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;
			if (worker == null) {
				return;
			}
			if (worker.CancellationPending) {
				e.Cancel = true;
				return;
			}
			_process = new Process {
				StartInfo = { FileName = _commandName, Arguments = _commandArguments },
					  EnableRaisingEvents = true
			};
			_process.Exited += Process_Exited;
			if (_process.Start ()) {
				_process.PriorityClass = ProcessPriorityClass.RealTime;
				if (HasStarted != null) {
					HasStarted(this, new EventArgs());
				}
			}
		}

		public Program (AppConfiguration appConfig)
		{
			_worker = new BackgroundWorker { WorkerSupportsCancellation = true };
			_worker.DoWork += Worker_Work;

			if (string.IsNullOrEmpty (appConfig.Command)) {
				return;
			}
			_commandName = appConfig.Command;
			_commandArguments = appConfig.Arguments;
		}

		/// <summary>
		/// Destructs instance and cleans up temporary files.
		/// </summary>
		~Program ()
		{ Dispose (false);
			GC.SuppressFinalize (this);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool isDisposing)
		{
			Stop ();
			if (_process != null) {
				_process.Exited -= Process_Exited;
				_process.Dispose ();
			}
			_worker.DoWork -= Worker_Work;
		}

		/// <summary>
		/// Starts the program.
		/// </summary>
		public void Start ()
		{
			StartProgram ();
		}

		/// <summary>
		/// Stops the program.
		/// </summary>
		public void Stop ()
		{
			if (!IsRunning) {
				return;
			}
			_worker.CancelAsync ();
			if (!_process.CloseMainWindow ()) {
				_process.Kill ();
			}
			_process.Close ();
			_process.Exited -= Process_Exited;
			_process.Dispose ();

			if (HasExited != null) {
				HasExited (this, new EventArgs ());
			}
		}

		void StartProgram ()
		{
			if (IsRunning) {
				return;
			}
			_worker.RunWorkerAsync ();
		}

		void Process_Exited (object sender, EventArgs args)
		{
			if (_process != null) {
				_process.Exited -= Process_Exited;
				_process.Dispose ();
			}
			if (HasExited != null) {
				HasExited (this, new EventArgs ());
			}
			_worker.CancelAsync ();
		}

		/// <summary>
		/// Tests, if singleton process is already running.
		/// </summary>
		void TestForRunningSingleton ()
		{
			if (string.IsNullOrEmpty (_commandName)) {
				return;
			}
			string[] processName = _commandName.Split (Path.DirectorySeparatorChar);
			Process[] processes = Process.GetProcessesByName (processName.Last ());
			if (processes.Any ()) {
				_process = processes.First ();
			}
		}
	}
}
