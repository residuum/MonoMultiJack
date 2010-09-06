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
using System.Runtime.InteropServices;
using JackdCIL;

namespace JackdCIL
{
	public class LibJackWrapper
	{		
		private const string _jackLibName = "libjack.so.0";
		private IntPtr _jackdClient;
		
		public LibJackWrapper(string clientName)
		{
			_jackdClient = jack_client_open(clientName, IntPtr.Zero, IntPtr.Zero);
			if (_jackdClient == IntPtr.Zero)
			{
				throw new JackdClientException("Could not create Jackd client.");
			}
		}
		
		~LibJackWrapper()
		{
			Close();
		}
		
		public void Close()
		{
			jack_client_close(_jackdClient);
		}
		
		public void Activate()
		{
			var result = jack_activate(_jackdClient);			
		}
		
		public string[] GetPorts()
		{
			//TODO: Not really working
			var ports = jack_get_ports(_jackdClient, null, null, 0);
			return (string[])Marshal.PtrToStructure(ports, typeof(string[]));
		}
		
		
		
		/// <summary>
		/// http://jackaudio.org/files/docs/html/group__ClientFunctions.html
		/// </summary>
		[DllImport(_jackLibName)]
		private static extern IntPtr jack_client_new(string client_name);
		
		[DllImport(_jackLibName)]
		private static extern int jack_activate(IntPtr jack_client_t);
		
		[DllImport(_jackLibName)]
		private static extern int jack_client_close(IntPtr jack_client_t);
		
		[DllImport(_jackLibName)]
		private static extern IntPtr jack_client_open(string client_name, 
		                                             IntPtr jack_options_t, 
		                                             IntPtr jack_status_t);		
		[DllImport(_jackLibName)]
		private static extern string jack_client_thread_id(IntPtr jack_client_t);
		
		[DllImport(_jackLibName)]
		private static extern int jack_deactivate(IntPtr jack_client_t);
		
		[DllImport(_jackLibName)]
		private static extern IntPtr jack_get_ports(IntPtr jack_client_t, 
		                                     string port_name_pattern,
		                                     string type_name_pattern,
		                                     long flags);
		
		//[DllImport(_jackLibName)]
		//private static extern string jack_get_ports();
	}
}
