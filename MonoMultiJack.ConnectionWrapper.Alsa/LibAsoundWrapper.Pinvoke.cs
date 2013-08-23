using System;
using System.Runtime.InteropServices;

namespace MonoMultiJack.ConnectionWrapper.Alsa
{
	internal static partial class LibAsoundWrapper
	{	
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_open (out IntPtr handle, string name, int streams, int mode);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_close (IntPtr seq);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_set_client_info (IntPtr seq, out IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_info_get_client (IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_set_client_name (IntPtr seq, string name);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_query_next_client (IntPtr seq, IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_client_info_get_client (IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_client_info_set_client (IntPtr info, int client);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_port_info_set_client (IntPtr info, int client);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_port_info_set_port (IntPtr info, int val);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_query_next_port (IntPtr seq, IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern IntPtr snd_seq_client_info_get_name (IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_client_info_get_type (IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern IntPtr snd_seq_port_info_get_name (IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern IntPtr snd_seq_get_any_client_info (IntPtr seq, int client, IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern IntPtr snd_seq_get_any_port_info (IntPtr seq, int client, int port, IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern IntPtr snd_seq_port_info_get_addr (IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_client_info_sizeof ();
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_port_info_sizeof ();
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_port_subscribe_sizeof ();
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_port_info_get_capability (IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_port_info_get_type (IntPtr info);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_query_subscribe_set_root (IntPtr info, IntPtr addr);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_query_subscribe_set_type (IntPtr info, int type);

		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_query_subscribe_set_index (IntPtr info, int index);

		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_query_subscribe_get_index (IntPtr info);

		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_query_port_subscribers (IntPtr seq, IntPtr subs);

		[DllImport(ASOUND_LIB_NAME)]
		static extern IntPtr snd_seq_query_subscribe_get_addr (IntPtr subs);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_port_subscribe_set_sender (IntPtr info, IntPtr addr);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_port_subscribe_set_dest (IntPtr info, IntPtr addr);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_port_subscribe_set_exclusive (IntPtr info, int val);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_port_subscribe_set_time_update (IntPtr info, int val);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern void snd_seq_port_subscribe_set_time_real (IntPtr info, int val);
		
		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_subscribe_port (IntPtr handle, IntPtr sub);

		[DllImport(ASOUND_LIB_NAME)]
		static extern int snd_seq_unsubscribe_port (IntPtr handle, IntPtr sub);

	}
}