// 
// JackdInterop.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009 Thomas Mayer
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
using System.Diagnostics;

namespace MonoMultiJack.Common
{
	/// <summary>
	/// static class for interoperation with libjack
	/// </summary>
	public static class JackdInterop
	{		
		public const string JackLibName = "libjack.so.0";
		
		/// <summary>
		/// http://jackaudio.org/files/docs/html/group__ClientFunctions.html
		/// </summary>
		[DllImport(JackLibName)]
		public static extern int jack_activate(ref IntPtr jack_client_t);
		[DllImport(JackLibName)]
		public static extern int jack_client_close(ref IntPtr jack_client_t);
		[DllImport(JackLibName)]
		public static extern IntPtr jack_client_open(ref StringBuilder client_name, 
		                                             IntPtr jack_options_t, 
		                                             ref IntPtr jack_status_t);		
		[DllImport(JackLibName)]
		public static extern string jack_client_thread_id(ref IntPtr jack_client_t);
		[DllImport(JackLibName)]
		public static extern int jack_deactivate(ref IntPtr jack_client_t);
		[DllImport(JackLibName)]
		public static extern void jack_internal_client_close(ref StringBuilder client_name);
		[DllImport(JackLibName)]
		public static extern int jack_internal_client_new(ref StringBuilder client_name,
		                                                  ref StringBuilder load_name,
		                                                  ref StringBuilder load_init);
		
		//[DllImport(JackLibName)]
		//public static extern string jack_get_ports();
	}
}
