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
using JackdCIL;
using System.Collections.Generic;
using Mono.Unix;

namespace JackdCIL
{
	/// <summary>
	/// Wrapper class for libjack
	/// </summary>
	public class LibJackWrapper : IDisposable
	{		
		private const string _jackLibName = "libjack.so.0";
		private IntPtr _jackdClient;
		
		/// <summary>
		/// Creates a new instance of the LibJackWrapper class.
		/// </summary>
		/// <param name="clientName">
		/// A <see cref="System.String"/> containing the name of the new jack client.
		/// </param>
		public LibJackWrapper(string clientName)
		{
			_jackdClient = jack_client_open(clientName, IntPtr.Zero, IntPtr.Zero);
			if (_jackdClient == IntPtr.Zero)
			{
				throw new JackdClientException("Could not create Jackd client.");
			}
			else
			{
				Activate();
			}
		}
		
		/// <summary>
		/// Desctructor for LibJackWrapper class.
		/// </summary>
		~LibJackWrapper()
		{
			Dispose();
		}
		
		#region IDisposable implementation
		/// <summary>
		/// Disposes the instance
		/// </summary>
		public void Dispose ()
		{
			Close();
			_jackdClient = IntPtr.Zero;
		}
		#endregion

		/// <summary>
		/// Closes jack client
		/// </summary>
		private void Close()
		{
			jack_client_close(_jackdClient);
		}
		
		/// <summary>
		/// Activates jack client
		/// </summary>
		private void Activate()
		{
			var result = jack_activate(_jackdClient);			
		}
		
		/// <summary>
		/// Gets all jackd ports
		/// </summary>
		/// <returns>
		/// A <see cref="IEnumerable<System.String>"/>
		/// </returns>
		public IEnumerable<string> GetPorts()
		{
			if (_jackdClient == IntPtr.Zero)
			{
				return null;
			}
			IntPtr ports = jack_get_ports(_jackdClient, IntPtr.Zero, IntPtr.Zero, 0);
			return UnixMarshal.PtrToStringArray(ports);		
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
		                                     IntPtr port_name_pattern,
		                                     IntPtr type_name_pattern,
		                                     long flags);
		
		//[DllImport(_jackLibName)]
		//private static extern string jack_get_ports();
	}
}
