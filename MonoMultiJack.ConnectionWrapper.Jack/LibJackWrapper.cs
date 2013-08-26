// 
// LibJackWrapper.cs
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
using MonoMultiJack.ConnectionWrapper.Jack.Types;

namespace MonoMultiJack.ConnectionWrapper.Jack
{
	/// <summary>
	/// Wrapper class for libjack. This file contains the main logic.
	/// </summary>
	internal static partial class LibJackWrapper
	{		
		static IntPtr _jackClient = IntPtr.Zero;
		static readonly List<JackPort> _portMapper = new List<JackPort> ();
		static List<IConnection> _connections = new List<IConnection> ();
		static readonly string _clientName = "MonoMultiJack" 
				+ (DateTime.Now.Ticks / 10000000).ToString ().Substring (6);
		
		static void OnPortRegistration (uint port, int register, IntPtr args)
		{
			ConnectionEventArgs eventArgs = new ConnectionEventArgs ();
			ConnectionType connectionType = ConnectionType.Undefined;
			if (register > 0) {
				JackPort newPort = GetJackPortData (port);
				_portMapper.Add (newPort);
				eventArgs.Message = "New port registered.";
				connectionType = newPort.ConnectionType;
				eventArgs.ChangeType = ChangeType.New;
				var clients = new List<IConnectable> ();
				Client newClient = new Client (newPort.ClientName, newPort.FlowDirection, newPort.ConnectionType);
				newClient.AddPort (newPort);
				clients.Add (newClient);
				eventArgs.Connectables = clients;
			} else {
				var oldPort = _portMapper.FirstOrDefault (map => map.Id == port);
				if (oldPort != null) {
					var ports = new List<Port> ();
					ports.Add (oldPort);
					eventArgs.Connectables = ports;
					eventArgs.ChangeType = ChangeType.Deleted;						
					connectionType = oldPort.ConnectionType;
					_portMapper.Remove (oldPort);
				}
				eventArgs.Message = "Port unregistered.";
			}
			eventArgs.ConnectionType = connectionType;
			PortOrConnectionHasChanged (null, eventArgs);			
		}
		
		static void OnPortConnect (uint a, uint b, int connect, IntPtr args)
		{
			try {
				var eventArgs = new ConnectionEventArgs ();
				var outPort = _portMapper.First (map => map.JackPortPointer == jack_port_by_id (_jackClient, a));
				var inPort = _portMapper.First (map => map.JackPortPointer == jack_port_by_id (_jackClient, b));
				if (connect != 0) {
					var connections = new List<IConnection> ();
					IConnection newConn = null;
					switch (outPort.ConnectionType) {
					case ConnectionType.JackAudio:
						newConn = new JackAudioConnection ();
						break;
					case ConnectionType.JackMidi:
						newConn = new JackMidiConnection ();
						break;
					}
					newConn.OutPort = outPort;
					newConn.InPort = inPort;
					_connections.Add (newConn);
					connections.Add (newConn);
					eventArgs.Connections = connections;					
					eventArgs.ConnectionType = _portMapper.First (map => map.JackPortPointer == jack_port_by_id (_jackClient, a)).ConnectionType;
					eventArgs.ChangeType = ChangeType.New;
					eventArgs.Message = "New Connection established";
				} else {
					var oldConn = _connections.Where (conn => conn.InPort == inPort
						&& conn.OutPort == outPort
					);
					eventArgs.Connections = oldConn.ToList ();
					eventArgs.ChangeType = ChangeType.Deleted;					
					eventArgs.ConnectionType = _portMapper.First (map => map.JackPortPointer == jack_port_by_id (_jackClient, a)).ConnectionType;
					_connections = _connections.Where (conn => conn.InPort != inPort || conn.OutPort != outPort)
			.ToList ();
					eventArgs.Message = "Connection deleted";
				}
				eventArgs.MessageType = MessageType.Info;
				PortOrConnectionHasChanged (null, eventArgs);
			} catch (Exception e) {
#if DEBUG
				Console.WriteLine (e.Message);
#endif
			}
		}
		
		static void OnJackShutdown (IntPtr args)
		{
			_jackClient = IntPtr.Zero;
			_portMapper.Clear ();
			JackHasShutdown (null, new ConnectionEventArgs ());
		}
		
		internal static event ConnectionEventHandler PortOrConnectionHasChanged;
		internal static event ConnectionEventHandler JackHasShutdown;
		
		internal static bool ConnectToServer ()
		{
			if (_jackClient == IntPtr.Zero) {
				_jackClient = jack_client_open (_clientName, 1, IntPtr.Zero);
			}
			if (_jackClient != IntPtr.Zero) {
				return Activate ();
			}
			return false;
		}

		/// <summary>
		/// Closes jack client
		/// </summary>
		internal static void Close ()
		{
			if (_jackClient != IntPtr.Zero) {
				jack_client_close (_jackClient);
			}
		}
		
		/// <summary>
		/// Activates jack client
		/// </summary>
		static bool Activate ()
		{
			jack_set_port_connect_callback (_jackClient, OnPortConnect, IntPtr.Zero);
			jack_set_port_registration_callback (_jackClient, OnPortRegistration, IntPtr.Zero);
			jack_on_shutdown (_jackClient, OnJackShutdown, IntPtr.Zero);
			int jackActivateStatus = jack_activate (_jackClient);
			//TODO: for Jackdmp: Call jack_get_ports() to populate initial clients or use some other methods;
//			IntPtr portList = jack_get_ports(_jackClient, null, null, 0);
//			string[] portNames = MarshallingHelper.PtrToStringArray(portList);
//			foreach(string portName in portNames) {
//				_portMapper.Add(GetJackPortData(portName));
//			}
//			if (portList != IntPtr.Zero){
//				Marshal.FreeHGlobal(portList);
//			}
			return jackActivateStatus == 0;
		}
		
		public static bool IsActive {
			get {
				return _jackClient != IntPtr.Zero;
			}
		}

		static JackPort GetJackPortData (uint portId)
		{
			IntPtr portPointer = jack_port_by_id (_jackClient, portId);			
			return MapPort (portPointer, portId);
		}

		static JackPort MapPort (IntPtr portPointer, uint portId)
		{
			if (portPointer == IntPtr.Zero) {
				return null;
			}
			try {
				string portName = jack_port_name (portPointer).PtrToString ();
				if (!string.IsNullOrEmpty (portName)) {
					FlowDirection portType = FlowDirection.Undefined;
					JackPortFlags portFlags = (JackPortFlags)jack_port_flags (portPointer);
					if ((portFlags & JackPortFlags.JackPortIsInput) == JackPortFlags.JackPortIsInput) {
						portType = FlowDirection.In;
					} else
						if ((portFlags & JackPortFlags.JackPortIsOutput) == JackPortFlags.JackPortIsOutput) {
						portType = FlowDirection.Out;
					}
					ConnectionType connectionType = ConnectionType.Undefined;
					string connectionTypeName = jack_port_type (portPointer).PtrToString ();
					if (connectionTypeName == JACK_DEFAULT_AUDIO_TYPE) {
						connectionType = ConnectionType.JackAudio;
					} else
						if (connectionTypeName == JACK_DEFAULT_MIDI_TYPE) {
						connectionType = ConnectionType.JackMidi;
					}
					JackPort newPort = new JackPort (portName, portPointer, portType, connectionType, portId);
					return newPort;
				}
				return null;
			} catch (Exception e) {
				#if DEBUG
				Console.WriteLine(e.Message);
				#endif
				return null;
			}
		}
		
		internal static bool Disconnect (Port outputPort, Port inputPort)
		{
			if (outputPort.FlowDirection != FlowDirection.Out || inputPort.FlowDirection != FlowDirection.In || outputPort.ConnectionType != inputPort.ConnectionType) {
				return false;
			}
			string outPortName = _portMapper.Where (map => map == outputPort)
				.Select (map => map.JackPortName).First ();
			string inPortName = _portMapper.Where (map => map == inputPort)
				.Select (map => map.JackPortName).First ();
			if (_connections.Any (c => c.InPort == inputPort && c.OutPort == outputPort)) {
				return jack_disconnect (_jackClient, outPortName, inPortName) == 0;			
			}
			return true;
		}
		
		internal static bool Connect (Port outputPort, Port inputPort)
		{
			if (outputPort.FlowDirection != FlowDirection.Out 
				|| inputPort.FlowDirection != FlowDirection.In 
				|| outputPort.ConnectionType != inputPort.ConnectionType) {
				return false;
			}
			string outPortName = _portMapper.First (map => map == outputPort).JackPortName;
			string inPortName = _portMapper.First (map => map == inputPort).JackPortName;
			return jack_connect (_jackClient, outPortName, inPortName) == 0;
		}
		
		internal static IEnumerable<JackPort> GetPorts (ConnectionType connectionType)
		{		
			var mappedPorts = _portMapper.Where (portMap => portMap.ConnectionType == connectionType);
			if (mappedPorts.Any ()) {
				return mappedPorts;
			}
			var ports = new List<JackPort> ();
			for (uint i = 0; true; i++) {
				JackPort newPort = GetJackPortData (i);
				if (newPort == null) {
					break;
				}
				_portMapper.Add (newPort);
				if (newPort.ConnectionType == connectionType) {
					ports.Add (newPort);
				}
			}
			return ports;
		}
		
		internal static IEnumerable<IConnection> GetConnections (ConnectionType connectionType)
		{
			return _connections;
		}
	}
}
