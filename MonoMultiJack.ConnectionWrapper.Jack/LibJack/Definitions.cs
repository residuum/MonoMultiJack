// 
// LibJackWrapper.Definitions.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2013 Thomas Mayer
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

namespace MonoMultiJack.ConnectionWrapper.Jack.LibJack
{
	/// <summary>
	/// Wrapper class for libjack. This file contains definitions for private classes, enums and constants.
	/// </summary>
	internal static class Definitions
	{
		public const string JACK_LIB_NAME = "libjack";
		public const string JACK_DEFAULT_AUDIO_TYPE = "32 bit float mono audio";
		public const string JACK_DEFAULT_MIDI_TYPE = "8 bit raw midi";

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        public delegate void JackPortRegistrationCallback (uint port,int register,IntPtr args);

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        public delegate void JackPortConnectCallback (uint a,uint b,int connect,IntPtr args);

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        public delegate void JackShutdownCallback (IntPtr args);
	}
}