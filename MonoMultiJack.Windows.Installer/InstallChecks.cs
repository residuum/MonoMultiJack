using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MonoMultiJack.Windows.Installer
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
			if (!IsInstalled ("Jack")) {
				DisplayJackdInfo (@"I could not find Jack on your system. 

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

		static bool IsInstalled (string program)
		{
#if DEBUG
			return false;
#endif
			// search in: CurrentUser
			RegistryKey key = Registry.CurrentUser.OpenSubKey (@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
			if (CheckForSubkey (program, key)) return true;

			// search in: LocalMachine_32
			key = Registry.LocalMachine.OpenSubKey (@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

			if (CheckForSubkey (program, key)) return true;

			// search in: LocalMachine_64
			key = Registry.LocalMachine.OpenSubKey (@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

			if (CheckForSubkey (program, key)) return true;

			// NOT FOUND
			return false;
		}

		static bool CheckForSubkey (string program, RegistryKey key)
		{
			return key.GetSubKeyNames ().Select (key.OpenSubKey)
			    .Select (subkey => subkey.GetValue ("DisplayName") as string)
			    .Any (displayName => program.Equals (displayName, StringComparison.OrdinalIgnoreCase));
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
