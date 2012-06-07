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
	private static extern int snd_seq_info_get_client(IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_set_client_name (IntPtr seq, string name);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_query_next_client (IntPtr seq, IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_client_info_get_client (IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern void snd_seq_client_info_set_client (IntPtr info, int client);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern void snd_seq_port_info_set_client (IntPtr info, int client);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern void snd_seq_port_info_set_port (IntPtr info, int val);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_query_next_port (IntPtr seq, IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern IntPtr snd_seq_client_info_get_name (IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern IntPtr snd_seq_port_info_get_name (IntPtr info);		
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern IntPtr snd_seq_get_any_client_info (IntPtr seq, int client, IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern IntPtr snd_seq_get_any_port_info (IntPtr seq, int client, int port, IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern IntPtr snd_seq_port_info_get_addr (IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_client_info_sizeof ();
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_port_info_sizeof ();
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_port_info_get_capability (IntPtr info);
		
	[DllImport(ASOUND_LIB_NAME)]
	private static extern int snd_seq_port_info_get_type (IntPtr info);
    }
}