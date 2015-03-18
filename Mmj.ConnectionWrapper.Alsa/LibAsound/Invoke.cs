using System;
using System.Runtime.InteropServices;

namespace Mmj.ConnectionWrapper.Alsa.LibAsound
{
	internal static class Invoke
	{	
		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_open (out IntPtr handle, string name, int streams, int mode);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_close (IntPtr seq);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		static extern int snd_seq_info_get_client (IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_set_client_name (IntPtr seq, string name);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_query_next_client (IntPtr seq, IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_client_info_get_client (IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_client_info_set_client (IntPtr info, int client);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_port_info_set_client (IntPtr info, int client);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_port_info_set_port (IntPtr info, int val);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_query_next_port (IntPtr seq, IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr snd_seq_client_info_get_name (IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_client_info_get_type (IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr snd_seq_port_info_get_name (IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr snd_seq_get_any_client_info (IntPtr seq, int client, IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr snd_seq_get_any_port_info (IntPtr seq, int client, int port, IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr snd_seq_port_info_get_addr (IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_client_info_sizeof ();

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_port_info_sizeof ();

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_port_subscribe_sizeof ();

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_port_info_get_capability (IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_port_info_get_type (IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_query_subscribe_set_root (IntPtr info, IntPtr addr);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_query_subscribe_set_type (IntPtr info, int type);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_query_subscribe_set_index (IntPtr info, int index);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_query_subscribe_get_index (IntPtr info);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_query_port_subscribers (IntPtr seq, IntPtr subs);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr snd_seq_query_subscribe_get_addr (IntPtr subs);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_port_subscribe_set_sender (IntPtr info, IntPtr addr);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_port_subscribe_set_dest (IntPtr info, IntPtr addr);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_port_subscribe_set_exclusive (IntPtr info, int val);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_port_subscribe_set_time_update (IntPtr info, int val);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void snd_seq_port_subscribe_set_time_real (IntPtr info, int val);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_subscribe_port (IntPtr handle, IntPtr sub);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int snd_seq_unsubscribe_port (IntPtr handle, IntPtr sub);

		[DllImport(Definitions.ASOUND_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr snd_strerror (int errno);
	}
}