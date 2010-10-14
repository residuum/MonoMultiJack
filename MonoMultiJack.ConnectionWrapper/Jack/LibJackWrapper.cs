// 
// LibJackWrapper.cs
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
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Mono.Unix;

namespace MonoMultiJack.ConnectionWrapper.Jack
{
	[Flags]
	public enum JackPortFlags : ulong
	{
		JackPortIsInput = 0x1,
		JackPortIsOutput = 0x2,
		JackPortIsPhysical = 0x4,
		JackPortCanMonitor = 0x8,
		JackPortIsTerminal = 0x10
	}
	/// <summary>
	/// Wrapper class for libjack
	/// </summary>
	internal static class LibJackWrapper
	{		
		private const string JACK_LIB_NAME = "libjack.so.0";
		private const string JACK_DEFAULT_AUDIO_TYPE = "32 bit float mono audio";
		private const string JACK_DEFAULT_MIDI_TYPE = "32 bit float mono audio";
		
		private static IntPtr _jackdClient;
		
		public static void ConnectToServer ()
		{
			if (_jackdClient == IntPtr.Zero)
			{
				_jackdClient = jack_client_new ("MonoMultiJack");
			}
			if (_jackdClient != IntPtr.Zero)
			{
				Activate ();
			}
		}

		/// <summary>
		/// Closes jack client
		/// </summary>
		private static void Close()
		{
			jack_client_close(_jackdClient);
		}
		
		/// <summary>
		/// Activates jack client
		/// </summary>
		private static void Activate ()
		{
			var result = jack_activate (_jackdClient);
		}
		
		public static bool IsActive
		{
			get 
			{
				return _jackdClient != IntPtr.Zero;
			}
		}
		
		/// <summary>
		/// Gets all jackd ports
		/// </summary>
		/// <returns>
		/// A <see cref="IEnumerable<System.String>"/>
		/// </returns>
		internal static IEnumerable<string> GetPorts(PortType portType, bool isAudio)
		{
			if (_jackdClient == IntPtr.Zero)
			{
				ConnectToServer();
			}
			ulong flags = 0;
			switch(portType)
			{
				case PortType.Input:
					flags = (long)JackPortFlags.JackPortIsInput;
					break;
				case PortType.Output:
					flags = (long)JackPortFlags.JackPortIsOutput;
					break;
			}
			string typeName = isAudio ? JACK_DEFAULT_AUDIO_TYPE : JACK_DEFAULT_MIDI_TYPE;
			IntPtr ports = jack_get_ports(_jackdClient, null, typeName, flags);
			return UnixMarshal.PtrToStringArray(ports);		
		}
		
		
		
		/// <summary>
		/// http://jackaudio.org/files/docs/html/group__ClientFunctions.html
		/// </summary>
		[DllImport(JACK_LIB_NAME)]
		private static extern IntPtr jack_client_new(string client_name);
		
		[DllImport(JACK_LIB_NAME)]
		private static extern int jack_activate(IntPtr jack_client_t);
		
		[DllImport(JACK_LIB_NAME)]
		private static extern int jack_client_close(IntPtr jack_client_t);
		
		[DllImport(JACK_LIB_NAME)]
		private static extern IntPtr jack_client_open(string client_name, 
		                                             IntPtr jack_options_t, 
		                                             IntPtr jack_status_t);		
		[DllImport(JACK_LIB_NAME)]
		private static extern string jack_client_thread_id(IntPtr jack_client_t);
		
		[DllImport(JACK_LIB_NAME)]
		private static extern int jack_deactivate(IntPtr jack_client_t);
		
		[DllImport(JACK_LIB_NAME)]
		private static extern IntPtr jack_get_ports(IntPtr jack_client_t, 
		                                     string port_name_pattern,
		                                     string type_name_pattern,
		                                     ulong flags);
	}
}
