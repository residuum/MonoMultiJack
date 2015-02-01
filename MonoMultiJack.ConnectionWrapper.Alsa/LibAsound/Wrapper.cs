// 
// LibAsoundWrapper.cs
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
using System.Collections.Generic;
using System.Linq;
using MonoMultiJack.ConnectionWrapper.Alsa.Types;

namespace MonoMultiJack.ConnectionWrapper.Alsa.LibAsound
{
	internal static class Wrapper
	{
		static IntPtr _alsaClient = IntPtr.Zero;
		static int _clientInfoSize;
		static int _portInfoSize;
		static int _subscriberInfoSize;

		static int GetClientInfoSize ()
		{
			if (_clientInfoSize == 0) {
				_clientInfoSize = Invoke.snd_seq_client_info_sizeof ();
			}
			return _clientInfoSize;
		}

		static int GetPortInfoSize ()
		{
			if (_portInfoSize == 0) {
				_portInfoSize = Invoke.snd_seq_port_info_sizeof ();
			}
			return _portInfoSize;
		}

		static int GetSubscriberInfoSize ()
		{
			if (_subscriberInfoSize == 0) {
				_subscriberInfoSize = Invoke.snd_seq_port_subscribe_sizeof ();
			}
			return _subscriberInfoSize;
		}

		internal static bool Activate ()
		{
			int activation = Invoke.snd_seq_open (
				out _alsaClient,
				"default",
				Definitions.SND_SEQ_OPEN_DUPLEX,
				Definitions.SND_SEQ_NONBLOCK
			);
			if (activation == 0) {
				Invoke.snd_seq_set_client_name (_alsaClient, "MonoMultiJack");
				return true;
			}
			return false;
		}

		internal static void DeActivate ()
		{
			if (_alsaClient != IntPtr.Zero) {
				Invoke.snd_seq_close (_alsaClient);
			}
		}

		internal static IEnumerable<AlsaPort> GetPorts ()
		{
			if (_alsaClient != IntPtr.Zero || Activate ()) {
				List<AlsaPort> ports = new List<AlsaPort> ();
				try {
					using (PointerWrapper clientInfo = new PointerWrapper(GetClientInfoSize ()))
					using (PointerWrapper portInfo = new PointerWrapper(GetPortInfoSize ())) {
						Invoke.snd_seq_client_info_set_client (clientInfo.Pointer, -1);
						while (Invoke.snd_seq_query_next_client(_alsaClient, clientInfo.Pointer) >= 0) {
							int clientId = Invoke.snd_seq_client_info_get_client (clientInfo.Pointer);
							Invoke.snd_seq_port_info_set_client (portInfo.Pointer, clientId);
							Invoke.snd_seq_port_info_set_port (portInfo.Pointer, -1);

							while (Invoke.snd_seq_query_next_port(_alsaClient, portInfo.Pointer) >= 0) {
								IEnumerable<AlsaPort> newPorts = CreatePorts (Invoke.snd_seq_port_info_get_addr (portInfo.Pointer));
								ports.AddRange (newPorts);
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
				Invoke.snd_seq_client_info_set_client (clientInfo.Pointer, portAddress.Client);
				Invoke.snd_seq_get_any_client_info (_alsaClient, portAddress.Client, clientInfo.Pointer);

				string clientName;
				string portName;
				int portCaps;
				int portType;
				GetPortData (portInfo, clientInfo, portAddress, out clientName, out portName, out portCaps, out portType);

				if ((portCaps & Definitions.SND_SEQ_PORT_CAP_NO_EXPORT) == Definitions.SND_SEQ_PORT_CAP_NO_EXPORT
					|| ((Invoke.snd_seq_client_info_get_type (clientInfo.Pointer) != Definitions.SND_SEQ_USER_CLIENT)
					&& ((portType == Definitions.SND_SEQ_PORT_SYSTEM_TIMER) || portType == Definitions.SND_SEQ_PORT_SYSTEM_ANNOUNCE))) {
					yield break;
				}

				bool isInput = (portCaps & Definitions.SND_SEQ_PORT_CAP_WRITE) == Definitions.SND_SEQ_PORT_CAP_WRITE;
				bool isOutput = (portCaps & Definitions.SND_SEQ_PORT_CAP_READ) == Definitions.SND_SEQ_PORT_CAP_READ;

				if (isOutput) {
					yield return new AlsaPort (
						portAddress,
						portName,
						clientName,
						FlowDirection.Out);
				}
				if (isInput) {
					yield return new AlsaPort (
						portAddress,
						portName,
						clientName,
						FlowDirection.In);
				}
			}
		}

		static void GetPortData (PointerWrapper portInfo, PointerWrapper clientInfo, SndSeqAddr portAddress, out string clientName, out string portName, out int portCaps, out int portType)
		{
			Invoke.snd_seq_port_info_set_client (portInfo.Pointer, portAddress.Client);
			Invoke.snd_seq_port_info_set_port (portInfo.Pointer, portAddress.Port);
			Invoke.snd_seq_get_any_port_info (_alsaClient, portAddress.Client, portAddress.Port, portInfo.Pointer);
			IntPtr clientNamePtr = Invoke.snd_seq_client_info_get_name (clientInfo.Pointer);
			clientName = clientNamePtr.PtrToString ();
			IntPtr portNamePtr = Invoke.snd_seq_port_info_get_name (portInfo.Pointer);
			portName = portNamePtr.PtrToString ();
			portCaps = Invoke.snd_seq_port_info_get_capability (portInfo.Pointer);
			portType = Invoke.snd_seq_port_info_get_type (portInfo.Pointer);
		}

		internal static IEnumerable<AlsaMidiConnection> GetConnections (IEnumerable<AlsaPort> ports)
		{
			IEnumerable<AlsaPort> alsaPorts = ports as IList<AlsaPort> ?? ports.ToList ();
			if ((_alsaClient != IntPtr.Zero || Activate ()) && alsaPorts.Any ()) {
				List<AlsaMidiConnection> connections = new List<AlsaMidiConnection> ();
				IEnumerable<AlsaPort> inPorts = alsaPorts.Where (p => p.FlowDirection == FlowDirection.In).ToList ();
				IEnumerable<AlsaPort> outPorts = alsaPorts.Where (p => p.FlowDirection == FlowDirection.Out);
				foreach (AlsaPort port in outPorts) {
					connections.AddRange (GetConnectionsForPort (port, inPorts));
				}
				return connections;
			}
			return new AlsaMidiConnection[0];
		}

		static IEnumerable<AlsaMidiConnection> GetConnectionsForPort (AlsaPort outPort, IEnumerable<AlsaPort> allInPorts)
		{
			IEnumerable<AlsaPort> alsaPorts = allInPorts as IList<AlsaPort> ?? allInPorts.ToList ();
			if (outPort == null || !alsaPorts.Any ()) {
				yield break;
			}
			using (PointerWrapper subscriberInfo = new PointerWrapper (GetSubscriberInfoSize ()))
			using (PointerWrapper alsaAddress = new PointerWrapper (outPort.AlsaAddress.SndSeqAddrToPtr())) {
				Invoke.snd_seq_query_subscribe_set_index (subscriberInfo.Pointer, 0);
				Invoke.snd_seq_query_subscribe_set_root (subscriberInfo.Pointer, alsaAddress.Pointer);
				Invoke.snd_seq_query_subscribe_set_type (subscriberInfo.Pointer, Definitions.SND_SEQ_QUERY_SUBS_READ);
				while (Invoke.snd_seq_query_port_subscribers(_alsaClient, subscriberInfo.Pointer) >= 0) {
					IntPtr connectedAddressPtr = Invoke.snd_seq_query_subscribe_get_addr (subscriberInfo.Pointer);
					if (connectedAddressPtr == IntPtr.Zero) {
						continue;
					}
					SndSeqAddr connectedAddress = connectedAddressPtr.PtrToSndSeqAddr ();
					AlsaPort connectedPort = alsaPorts.FirstOrDefault (p => p.AlsaAddress.Client == connectedAddress.Client 
						&& p.AlsaAddress.Port == connectedAddress.Port
					);
					if (connectedPort != null) {
						yield return new AlsaMidiConnection {
							OutPort = outPort,
							InPort = connectedPort
						};
					}
					Invoke.snd_seq_query_subscribe_set_index (
						subscriberInfo.Pointer,
						Invoke.snd_seq_query_subscribe_get_index (subscriberInfo.Pointer) + 1
					);
				}

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
					Invoke.snd_seq_port_subscribe_set_sender (subscriberInfo.Pointer, outPortAddr.Pointer);
					Invoke.snd_seq_port_subscribe_set_dest (subscriberInfo.Pointer, inPortAddr.Pointer);
					Invoke.snd_seq_port_subscribe_set_exclusive (subscriberInfo.Pointer, 0);
					Invoke.snd_seq_port_subscribe_set_time_update (subscriberInfo.Pointer, 0);
					Invoke.snd_seq_port_subscribe_set_time_real (subscriberInfo.Pointer, 0);
					int subs = disconnect
                        ? Invoke.snd_seq_unsubscribe_port (_alsaClient, subscriberInfo.Pointer)
                        : Invoke.snd_seq_subscribe_port (_alsaClient, subscriberInfo.Pointer);
					return subs == 0;
				}
			} catch {
				return false;
			}
		}
	}
}