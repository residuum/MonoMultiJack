using System;
using System.Text;
using System.Runtime.InteropServices;

namespace MonoMultiJack.ConnectionWrapper.Alsa
{
    internal static partial class LibAsoundWrapper
    {	
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_open (out IntPtr handle, string name, int streams, int mode);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_close (IntPtr seq);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_set_client_info (IntPtr seq, out IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_set_client_name (IntPtr seq, string name);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_query_next_client (IntPtr seq, out IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern IntPtr snd_seq_client_info_get_client (out IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern void snd_seq_client_info_set_client (out IntPtr info, int client);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern void snd_seq_port_info_set_client (out IntPtr info, int client);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern void snd_seq_port_info_set_port (out IntPtr info, int val);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_query_next_port (IntPtr seq, out IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern IntPtr snd_seq_client_info_get_name (out IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern IntPtr snd_seq_port_info_get_name (out IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern IntPtr snd_seq_port_info_get_addr (IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_client_info_sizeof ();
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_port_info_sizeof ();
    }
}