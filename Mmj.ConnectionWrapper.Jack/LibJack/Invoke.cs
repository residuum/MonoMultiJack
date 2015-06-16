// 
// LibJackWrapper.Pinvoke.cs
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
using System.Runtime.InteropServices;

namespace Mmj.ConnectionWrapper.Jack.LibJack
{
	/// <summary>
	/// Wrapper class for libjack, this file contains the wrapped API function of libjack.
	/// </summary>
	internal static class Invoke
	{
				
		/// <summary>
		/// For a full list of functions see http://jackaudio.org/files/docs/html/index.html
		/// </summary>
		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jack_activate (IntPtr jack_client_t);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jack_client_close (IntPtr jack_client_t);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jack_client_open (string client_name, byte jack_options_t, IntPtr jack_status_t);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jack_port_by_id (IntPtr jack_client_t, uint port_id);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jack_port_by_name (IntPtr jack_client_t, 
		                                               string port_name);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jack_get_ports (IntPtr jack_client_t, string port_name_pattern, string type_name_pattern, ulong flags);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jack_port_name (IntPtr jack_port_t);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jack_port_flags (IntPtr jack_port_t);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jack_port_type (IntPtr jack_port_t);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr jack_port_get_all_connections (IntPtr jack_client_t, IntPtr jack_port_t);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jack_connect (IntPtr jack_client_t, string source_port, string destination_port);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jack_disconnect (IntPtr jack_client_t, string source_port, string destination_port);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jack_set_port_registration_callback (IntPtr jack_client_t, Definitions.JackPortRegistrationCallback registration_callback, IntPtr args);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jack_set_port_connect_callback (IntPtr jack_client_t, Definitions.JackPortConnectCallback connect_callback, IntPtr args);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jack_set_xrun_callback (IntPtr jack_client_t, Definitions.JackXRunCallback function, IntPtr args);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern float jack_get_xrun_delayed_usecs (IntPtr jack_client_t);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void jack_on_shutdown (IntPtr jack_client_t, Definitions.JackShutdownCallback function, IntPtr args);

		[DllImport(Definitions.JACK_LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void jack_free (IntPtr ptr);
	}
}
