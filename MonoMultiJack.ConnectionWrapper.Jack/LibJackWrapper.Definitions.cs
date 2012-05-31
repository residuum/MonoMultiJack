// 
// LibJackWrapperClasses.cs
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

namespace MonoMultiJack.ConnectionWrapper.Jack
{
    /// <summary>
    /// Wrapper class for libjack. This file contains definitions for private classes, enums and constants.
    /// </summary>
    internal static partial class LibJackWrapper
    {		
	private const string JACK_LIB_NAME = "libjack.so.0";
	private const string JACK_DEFAULT_AUDIO_TYPE = "32 bit float mono audio";
	private const string JACK_DEFAULT_MIDI_TYPE = "8 bit raw midi";
		
	[Flags]
	private enum JackPortFlags
	{
	    JackPortIsInput = 0x1,
	    JackPortIsOutput = 0x2,
	    JackPortIsPhysical = 0x4,
	    JackPortCanMonitor = 0x8,
	    JackPortIsTerminal = 0x10
	}
			
	private delegate void JackPortRegistrationCallback (uint port,int register,IntPtr args);

	private delegate void JackPortConnectCallback (int a,int b,int connect,IntPtr args);

	private delegate void JackShutdownCallback (IntPtr args);
		
	private class JackPort : Port
	{
	    public uint JackPortId {
		get;
		private set;
	    }
			
	    public IntPtr JackPortPointer {
		get;
		private set;
	    }
			
	    public string JackPortName {
		get;
		private set;
	    }
		
	    public JackPort (string jackPortName, uint jackPortId, IntPtr jackPortPtr, PortType portType, ConnectionType connectionType)
	    {				
		JackPortName = jackPortName;
		JackPortId = jackPortId;
		JackPortPointer = jackPortPtr;
		PortType = portType;
		ConnectionType = connectionType;
		string[] splittedName = jackPortName.Split (new[] { ':' });
		ClientName = splittedName [0];
		Name = splittedName [1];				
	    }
			
	}
    }
}