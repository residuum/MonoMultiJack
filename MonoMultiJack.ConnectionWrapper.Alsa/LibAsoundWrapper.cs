// 
// LibAsoundWrapper.cs
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
using System.Collections.Generic;
using System.Linq;
using Mono.Unix;
using System.Runtime.InteropServices;

namespace MonoMultiJack.ConnectionWrapper.Alsa
{
    internal static partial class LibAsoundWrapper
    {		
	private static IntPtr _alsaClient = IntPtr.Zero;
	private static List<AlsaPort> _portMapper = new List<AlsaPort> ();
	private static List<IConnection> _connections = new List<IConnection> ();
	private static int _clientInfoSize;
	private static int _portInfoSize;
		
	internal static bool Activate ()
	{
	    int activation = snd_seq_open (
		out _alsaClient,
		"default",
		SND_SEQ_OPEN_DUPLEX,
		SND_SEQ_NONBLOCK
	    );
#if DEBUG
	    Console.WriteLine ("Alsa Activation: " + activation);
#endif
	    if (activation == 0) {
		snd_seq_set_client_name (_alsaClient, "MonoMultiJack");
		return true;
	    }
	    return false;
	}
		
	internal static void DeActivate ()
	{
	    if (_alsaClient != IntPtr.Zero) {
		snd_seq_close (_alsaClient);
	    }
	}
		
	internal static IEnumerable<Port> GetPorts ()
	{
	    if (_alsaClient != IntPtr.Zero || Activate ()) {
		IntPtr clientInfo = IntPtr.Zero;
		IntPtr portInfo = IntPtr.Zero;
		var ports = new List<Port> ();
				
		try {
		    if (_clientInfoSize == 0) {
			_clientInfoSize = snd_seq_client_info_sizeof ();
		    }
		    if (_portInfoSize == 0) {
			_portInfoSize = snd_seq_port_info_sizeof ();
		    }
		    clientInfo = Marshal.AllocHGlobal (_clientInfoSize);
		    portInfo = Marshal.AllocHGlobal (_portInfoSize);
		    snd_seq_client_info_set_client (clientInfo, -1);
		    while (snd_seq_query_next_client(_alsaClient, clientInfo) >= 0) {
			int clientId = snd_seq_client_info_get_client (clientInfo);
			snd_seq_port_info_set_client (portInfo, clientId);
			snd_seq_port_info_set_port (portInfo, -1);
					
			while (snd_seq_query_next_port(_alsaClient, portInfo) >= 0) {
			    IntPtr portAddrPtr = snd_seq_port_info_get_addr (portInfo);
			    Port newPort = CreatePort (portAddrPtr);
			    if (newPort != null) {
				ports.Add (newPort);
			    }
			}				
		    }
		} catch (Exception ex) {
		    Console.WriteLine (ex.Message);
		    return new Port[0];
		} finally {
		    if (clientInfo != IntPtr.Zero) {
			Marshal.FreeHGlobal (clientInfo);
		    }
		    if (portInfo != IntPtr.Zero) {
			Marshal.FreeHGlobal (portInfo);
		    }
		}
		return ports;
	    }
	    return new Port[0];
	}
		
	private static Port CreatePort (IntPtr addrPtr)
	{
	    IntPtr clientInfo = IntPtr.Zero;
	    IntPtr portInfo = IntPtr.Zero;
	    try {
		var portAddress = (SndSeqAddr)Marshal.PtrToStructure (
		    addrPtr,
		    typeof(SndSeqAddr)
		);
		clientInfo = Marshal.AllocHGlobal (_clientInfoSize);
		portInfo = Marshal.AllocHGlobal (_portInfoSize);
		snd_seq_client_info_set_client (clientInfo, portAddress.client);
		snd_seq_get_any_client_info (_alsaClient, portAddress.client, clientInfo);

		snd_seq_port_info_set_client (portInfo, portAddress.client);
		snd_seq_port_info_set_port (portInfo, portAddress.port);				
		snd_seq_get_any_port_info (_alsaClient, portAddress.client, portAddress.port, portInfo);

		IntPtr clientNamePtr = snd_seq_client_info_get_name (clientInfo);
		string clientName = UnixMarshal.PtrToString (clientNamePtr);
		IntPtr portNamePtr = snd_seq_port_info_get_name (portInfo);
		string portName = UnixMarshal.PtrToString (portNamePtr);
				//TODO: Find Capabilities etc.
		Port newPort = new Port (
		portName,
		clientName,
		PortType.Output,
		ConnectionType.AlsaMidi
		);
		return newPort;
	    } finally {
		if (clientInfo != IntPtr.Zero) {
		    Marshal.FreeHGlobal (clientInfo);
		}
		if (portInfo != IntPtr.Zero) {
		    Marshal.FreeHGlobal (portInfo);
		}
	    }
	}

    }
}