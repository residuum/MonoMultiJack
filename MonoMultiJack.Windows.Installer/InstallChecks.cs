using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mmj.Windows.Installer
{
	[RunInstaller (true)]
	public partial class InstallChecks : System.Configuration.Install.Installer
	{
		public InstallChecks ()
		{
			InitializeComponent ();
		}

		protected override void OnBeforeInstall (IDictionary savedState)
		{
			base.OnBeforeInstall (savedState);
			if (!IsInstalled ("libjack")) {
				DisplayJackdInfo (@"I could not find Jack on your system or it is not accessible. 

MonoMultiJack is used for managing Jack and software using libjack.

Please download and install: http://jackaudio.org/downloads/");
			} 
		}

		void DisplayJackdInfo (string message)
		{
			WindowWrapper wrapper = null;

			string productName = Context.Parameters["ProductName"].Trim ();
			IntPtr hwnd = IntPtr.Zero;
			Process[] procs = Process.GetProcessesByName ("msiexec");
			if (procs.Length > 0) {
				foreach (Process proc in procs) {
					if (proc.MainWindowTitle == productName) {
						hwnd = proc.MainWindowHandle;
						break;
					}
				}
			}
			if (hwnd != IntPtr.Zero) {
				wrapper = new WindowWrapper (hwnd);
			}
			if (wrapper != null) {
				MessageBox.Show (wrapper, message);
			} else {
				MessageBox.Show (message);
			}
		}

		[DllImport("kernel32")]
		static extern bool FreeLibrary (int hLibModule);

		[DllImport("kernel32")]
		static extern int LoadLibrary (string lpLibFileName);

		bool IsInstalled (string dllName)
		{
			int libId = LoadLibrary (dllName);
			if (libId > 0) {
				FreeLibrary(libId);
			}
			return (libId > 0);
		}

		private class WindowWrapper : IWin32Window
		{
			public WindowWrapper (IntPtr handle)
			{
				_hwnd = handle;
			}

			public IntPtr Handle {
				get { return _hwnd; }
			}

			readonly IntPtr _hwnd;
		}
	}
}
