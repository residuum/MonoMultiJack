using System;
using System.Runtime.InteropServices;

namespace MonoMultiJack.ConnectionWrapper.Alsa
{
	internal static partial class LibAsoundWrapper
	{	
		[DllImport(ASOUND_LIB_NAME)]
		private static extern int snd_seq_open(out IntPtr handle, string name, int streams, int mode);
		
		[DllImport(ASOUND_LIB_NAME)]
		private static extern int snd_seq_close(IntPtr seq);
		
		[DllImport(ASOUND_LIB_NAME)]
		private static extern int snd_seq_set_client_name(IntPtr seq, string name);
	}
}

