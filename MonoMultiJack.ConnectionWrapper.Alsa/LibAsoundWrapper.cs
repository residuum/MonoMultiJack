// 
// LibAsoundWrapper.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2012 Thomas Mayer
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
using MonoMultiJack.ConnectionWrapper.Alsa.Types;

namespace MonoMultiJack.ConnectionWrapper.Alsa
{
	internal static partial class LibAsoundWrapper
	{		
		static uint _portNumberMax = 0;
		static IntPtr _alsaClient = IntPtr.Zero;
		static int _clientInfoSize;
		static int _portInfoSize;
		static int _subscriberInfoSize;

		static int GetClientInfoSize ()
		{
			if (_clientInfoSize == 0) {
				_clientInfoSize = snd_seq_client_info_sizeof ();
			}
			return _clientInfoSize;
		}

		static int GetPortInfoSize ()
		{
			if (_portInfoSize == 0) {
				_portInfoSize = snd_seq_port_info_sizeof ();
			}
			return _portInfoSize;
		}

		static int GetSubscriberInfoSize ()
		{
			if (_subscriberInfoSize == 0) {
				_subscriberInfoSize = snd_seq_port_subscribe_sizeof ();
			}
			return _subscriberInfoSize;
		}
		
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
				var ports = new List<AlsaPort> ();
				try {
					using (PointerWrapper clientInfo = new PointerWrapper(GetClientInfoSize ()))
					using (PointerWrapper portInfo = new PointerWrapper(GetPortInfoSize ())) {
						snd_seq_client_info_set_client (clientInfo.Pointer, -1);
						while (snd_seq_query_next_client(_alsaClient, clientInfo.Pointer) >= 0) {
							int clientId = snd_seq_client_info_get_client (clientInfo.Pointer);
							snd_seq_port_info_set_client (portInfo.Pointer, clientId);
							snd_seq_port_info_set_port (portInfo.Pointer, -1);
					
							while (snd_seq_query_next_port(_alsaClient, portInfo.Pointer) >= 0) {
								IEnumerable<AlsaPort> newPorts = CreatePorts (snd_seq_port_info_get_addr (portInfo.Pointer));
								if (newPorts != null) {
									ports.AddRange (newPorts);
								}
							}				
						}
					} 
				} catch (Exception ex) {
					Console.WriteLine (ex.Message);
					return new AlsaPort[0];
				} 
				return ports;
			}
			return new AlsaPort[0];
		}
		
		static IEnumerable<AlsaPort> CreatePorts (IntPtr portAddressPtr)
		{
			using (PointerWrapper clientInfo = new PointerWrapper(GetClientInfoSize ()))
			using (PointerWrapper portInfo = new PointerWrapper(GetPortInfoSize ())) {
				SndSeqAddr portAddress = portAddressPtr.PtrToSndSeqAddr ();
				snd_seq_client_info_set_client (clientInfo.Pointer, portAddress.Client);
				snd_seq_get_any_client_info (_alsaClient, portAddress.Client, clientInfo.Pointer);

				snd_seq_port_info_set_client (portInfo.Pointer, portAddress.Client);
				snd_seq_port_info_set_port (portInfo.Pointer, portAddress.Port);				
				snd_seq_get_any_port_info (
					_alsaClient,
		            portAddress.Client,
					portAddress.Port,
					portInfo.Pointer
				);

				IntPtr clientNamePtr = snd_seq_client_info_get_name (clientInfo.Pointer);
				string clientName = clientNamePtr.PtrToString ();
				IntPtr portNamePtr = snd_seq_port_info_get_name (portInfo.Pointer);
				string portName = portNamePtr.PtrToString ();

				int portCaps = snd_seq_port_info_get_capability (portInfo.Pointer);
				int portType = snd_seq_port_info_get_type (portInfo.Pointer);

				if ((portCaps & SND_SEQ_PORT_CAP_NO_EXPORT) != 0 
					|| ((snd_seq_client_info_get_type (clientInfo.Pointer) != SND_SEQ_USER_CLIENT)
					&& ((portType == SND_SEQ_PORT_SYSTEM_TIMER) || portType == SND_SEQ_PORT_SYSTEM_ANNOUNCE))) {
					return new List<AlsaPort> ();
				}

				bool isInput = (portCaps & SND_SEQ_PORT_CAP_WRITE) != 0;
				bool isOutput = (portCaps & SND_SEQ_PORT_CAP_READ) != 0;

				var ports = new List<AlsaPort> ();
				if (isOutput) {
					ports.Add (new AlsaPort (
						portAddress,
						portName,
						clientName,
						FlowDirection.Out, 
						_portNumberMax)
					);
					_portNumberMax++;
				}
				if (isInput) {
					ports.Add (new AlsaPort (
						portAddress,
						portName,
						clientName,
						FlowDirection.In,
						_portNumberMax)
					);
					_portNumberMax++;
				}
				return ports;
			}
		}

		internal static IEnumerable<AlsaMidiConnection> GetConnections (IEnumerable<AlsaPort> ports)
		{
			if ((_alsaClient != IntPtr.Zero || Activate ()) && ports.Any ()) {
				var connections = new List<AlsaMidiConnection> ();
				var inPorts = ports.Where (p => p.FlowDirection == FlowDirection.In);
				var outPorts = ports.Where (p => p.FlowDirection == FlowDirection.Out);
				foreach (AlsaPort port in outPorts) {
					connections.AddRange (GetConnectionsForPort (port, inPorts));
				}
				return connections;
			}
			return new AlsaMidiConnection[0];
		}

		static IEnumerable<AlsaMidiConnection> GetConnectionsForPort (AlsaPort outPort, IEnumerable<AlsaPort> allInPorts)
		{
			if (outPort == null || !allInPorts.Any ()) {
				return new AlsaMidiConnection[0];
			}
			using (PointerWrapper subscriberInfo = new PointerWrapper (GetSubscriberInfoSize ()))
			using (PointerWrapper alsaAddress = new PointerWrapper (outPort.AlsaAddress.SndSeqAddrToPtr())) {
				List<AlsaMidiConnection> connections = new List<AlsaMidiConnection> ();
				snd_seq_query_subscribe_set_index (subscriberInfo.Pointer, 0);
				snd_seq_query_subscribe_set_root (subscriberInfo.Pointer, alsaAddress.Pointer);
				snd_seq_query_subscribe_set_type (subscriberInfo.Pointer, SND_SEQ_QUERY_SUBS_READ);
				while (snd_seq_query_port_subscribers(_alsaClient, subscriberInfo.Pointer) >= 0) {
					IntPtr connectedAddressPtr = snd_seq_query_subscribe_get_addr (subscriberInfo.Pointer);
					if (connectedAddressPtr == IntPtr.Zero) {
						continue;
					}
					SndSeqAddr connectedAddress = connectedAddressPtr.PtrToSndSeqAddr ();
					AlsaPort connectedPort = allInPorts.FirstOrDefault (p => p.AlsaAddress.Client == connectedAddress.Client 
						&& p.AlsaAddress.Port == connectedAddress.Port
					);
					if (connectedPort != null) {
						connections.Add (new AlsaMidiConnection (){OutPort = outPort, InPort = connectedPort});
					}
					snd_seq_query_subscribe_set_index (
						subscriberInfo.Pointer,
						snd_seq_query_subscribe_get_index (subscriberInfo.Pointer) + 1
					);
				}

				return connections;
			}
		}

		public static bool Connect (AlsaPort outPort, AlsaPort inPort)
		{
			return ChangeConnection (outPort, inPort, false);
		}

		public static bool Disconnect (AlsaPort outPort, AlsaPort inPort)
		{
			return ChangeConnection (outPort, inPort, true);
		}

		static bool ChangeConnection (AlsaPort outPort, AlsaPort inPort, bool disconnect)
		{
			try {
				using (PointerWrapper subscriberInfo = new PointerWrapper (GetSubscriberInfoSize ()))
				using (PointerWrapper outPortAddr = new PointerWrapper (outPort.AlsaAddress.SndSeqAddrToPtr ()))
				using (PointerWrapper inPortAddr = new PointerWrapper (inPort.AlsaAddress.SndSeqAddrToPtr ())) {
					snd_seq_port_subscribe_set_sender (subscriberInfo.Pointer, outPortAddr.Pointer);
					snd_seq_port_subscribe_set_dest (subscriberInfo.Pointer, inPortAddr.Pointer);
					snd_seq_port_subscribe_set_exclusive (subscriberInfo.Pointer, 0);
					snd_seq_port_subscribe_set_time_update (subscriberInfo.Pointer, 0);
					snd_seq_port_subscribe_set_time_real (subscriberInfo.Pointer, 0);
					if (disconnect) {
						return  snd_seq_unsubscribe_port (_alsaClient, subscriberInfo.Pointer) == 0;
					} else {
						return snd_seq_subscribe_port (_alsaClient, subscriberInfo.Pointer) == 0;
					}
				}
			} catch (Exception e) {
				#if DEBUG
				Console.WriteLine (e.Message);
				#endif
				return false;
			}
		}
	}
}