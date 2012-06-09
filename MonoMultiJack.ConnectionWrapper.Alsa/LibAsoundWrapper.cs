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
using MonoMultiJack.ConnectionWrapper.Alsa.Types;

namespace MonoMultiJack.ConnectionWrapper.Alsa
{
    internal static partial class LibAsoundWrapper
    {		
	private static IntPtr _alsaClient = IntPtr.Zero;
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
		
	internal static IEnumerable<AlsaPort> GetPorts ()
	{
	    if (_alsaClient != IntPtr.Zero || Activate ()) {
		IntPtr clientInfo = IntPtr.Zero;
		IntPtr portInfo = IntPtr.Zero;
		var ports = new List<AlsaPort> ();
				
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
			    IEnumerable<AlsaPort> newPorts = CreatePorts (portAddrPtr);
			    if (newPorts != null) {
				ports.AddRange (newPorts);
			    }
			}				
		    }
		} catch (Exception ex) {
		    Console.WriteLine (ex.Message);
		    return new AlsaPort[0];
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
	    return new AlsaPort[0];
	}
		
	private static IEnumerable<AlsaPort> CreatePorts (IntPtr addrPtr)
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
		snd_seq_client_info_set_client (clientInfo, portAddress.Client);
		snd_seq_get_any_client_info (_alsaClient, portAddress.Client, clientInfo);

		snd_seq_port_info_set_client (portInfo, portAddress.Client);
		snd_seq_port_info_set_port (portInfo, portAddress.Port);				
		snd_seq_get_any_port_info (
		    _alsaClient,
		    portAddress.Client,
		    portAddress.Port,
		    portInfo
		);

		IntPtr clientNamePtr = snd_seq_client_info_get_name (clientInfo);
		string clientName = Marshal.PtrToStringAnsi (clientNamePtr);
		IntPtr portNamePtr = snd_seq_port_info_get_name (portInfo);
		string portName = Marshal.PtrToStringAnsi (portNamePtr);

		int portCaps = snd_seq_port_info_get_capability (portInfo);
		//int portType = snd_seq_port_info_get_type (portInfo);

		if ((portCaps & SND_SEQ_PORT_CAP_NO_EXPORT) != 0)
		    return new List<AlsaPort> ();

		bool isInput = (portCaps & SND_SEQ_PORT_CAP_WRITE) != 0;
		bool isOutput = (portCaps & SND_SEQ_PORT_CAP_READ) != 0;

		var ports = new List<AlsaPort> ();
		if (isOutput) {
		    ports.Add (new AlsaPort (
						portAddress,
						portName,
						clientName,
						PortType.Output
		    )
		    );

		}
		if (isInput) {
		    ports.Add (new AlsaPort (
						portAddress,
						portName,
						clientName,
						PortType.Input
		    )
		    );

		}
		return ports;
	    } finally {
		if (clientInfo != IntPtr.Zero) {
		    Marshal.FreeHGlobal (clientInfo);
		}
		if (portInfo != IntPtr.Zero) {
		    Marshal.FreeHGlobal (portInfo);
		}
	    }
	}

	public static void MonitorPortChanges ()
	{
		throw new NotImplementedException ();
	}

	internal static IEnumerable<AlsaMidiConnection> GetConnections ()
	{
	    if (_alsaClient != IntPtr.Zero || Activate ()) {
		var connections = new List<AlsaMidiConnection> ();
		return connections;
	    }
	    return new AlsaMidiConnection[0];
	}
    }
}