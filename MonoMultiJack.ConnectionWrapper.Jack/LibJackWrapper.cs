// 
// LibJackWrapper.cs
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
using System.Runtime.InteropServices;
using MonoMultiJack.ConnectionWrapper.Jack.Types;

namespace MonoMultiJack.ConnectionWrapper.Jack
{
	/// <summary>
	/// Wrapper class for libjack. This file contains the main logic.
	/// </summary>
	internal static partial class LibJackWrapper
	{		
		private static IntPtr _jackClient = IntPtr.Zero;
		private static List<JackPort> _portMapper = new List<JackPort>();
		private static List<IConnection> _connections = new List<IConnection>();
		
		private static void OnPortRegistration(uint port, int register, IntPtr args)
		{
			ConnectionEventArgs eventArgs = new ConnectionEventArgs();
			ConnectionType connectionType = ConnectionType.Undefined;
			if (register > 0) {
				JackPort newPort = GetJackPortData(port);
				_portMapper.Add(newPort);
				eventArgs.Message = "New port registered.";
				connectionType = newPort.ConnectionType;
				eventArgs.ChangeType = ChangeType.New;
				var ports = new List<Port>();
				ports.Add(newPort);
				eventArgs.Ports = ports;
			} else {
				var oldPort = _portMapper.FirstOrDefault(map => map.JackPortId == port);
				if (oldPort != null) {
					var ports = new List<Port>();
					ports.Add(oldPort);
					eventArgs.Ports = ports;
					eventArgs.ChangeType = ChangeType.Deleted;						
					connectionType = oldPort.ConnectionType;
					_portMapper.Remove(oldPort);
				}
				eventArgs.Message = "Port unregistered.";
			}
			eventArgs.ConnectionType = connectionType;
			PortOrConnectionHasChanged(null, eventArgs);			
		}
		
		private static void OnPortConnect(int a, int b, int connect, IntPtr args)
		{
			try {
				var eventArgs = new ConnectionEventArgs();
				var outPort = _portMapper.First(map => map.JackPortId == a);
				var inPort = _portMapper.First(map => map.JackPortId == b);
				if (connect != 0) {
					var connections = new List<IConnection>();
					IConnection newConn = null;
					switch (outPort.ConnectionType) {
						case ConnectionType.JackAudio:
							newConn = new JackAudioConnection();
							break;
						case ConnectionType.JackMidi:
							newConn = new JackMidiConnection();
							break;
					}
					newConn.OutPort = outPort;
					newConn.InPort = inPort;
					_connections.Add(newConn);
					connections.Add(newConn);
					eventArgs.Connections = connections;					
					eventArgs.ConnectionType = _portMapper.First(map => map.JackPortId == a).ConnectionType;
					eventArgs.ChangeType = ChangeType.New;
					eventArgs.Message = "New Connection established";
				} else {
					var oldConn = _connections.Where(conn => conn.InPort.ClientName == inPort.ClientName && conn.InPort.Name == inPort.Name
						&& conn.OutPort.ClientName == outPort.ClientName && conn.OutPort.Name == outPort.Name
					);
					eventArgs.Connections = oldConn.ToList();
					eventArgs.ChangeType = ChangeType.Deleted;					
					eventArgs.ConnectionType = _portMapper.First(map => map.JackPortId == a).ConnectionType;
					_connections = _connections.Where(conn => conn.InPort.ClientName != inPort.ClientName || conn.InPort.Name != inPort.Name
						|| conn.OutPort.ClientName != outPort.ClientName || conn.OutPort.Name != outPort.Name
					)
			.ToList();
					eventArgs.Message = "Connection deleted";
				}
				eventArgs.MessageType = MessageType.Info;
				PortOrConnectionHasChanged(null, eventArgs);
			} catch (Exception e) {
#if DEBUG
				Console.WriteLine (e.Message);
#endif
			}
		}
		
		private static void OnJackShutdown(IntPtr args)
		{
			_jackClient = IntPtr.Zero;
			_portMapper.Clear();
			JackHasShutdown(null, new ConnectionEventArgs());
		}
		
		internal static event ConnectionEventHandler PortOrConnectionHasChanged;
		internal static event ConnectionEventHandler JackHasShutdown;
		
		internal static bool ConnectToServer()
		{
			if (_jackClient == IntPtr.Zero) {
				_jackClient = jack_client_new("MonoMultiJack");
			}
			if (_jackClient != IntPtr.Zero) {
				return Activate();
			}
			return false;
		}

		/// <summary>
		/// Closes jack client
		/// </summary>
		internal static void Close()
		{
			if (_jackClient != IntPtr.Zero) {
				jack_client_close(_jackClient);
			}
		}
		
		/// <summary>
		/// Activates jack client
		/// </summary>
		private static bool Activate()
		{
			jack_set_port_connect_callback(_jackClient, OnPortConnect, IntPtr.Zero);
			jack_set_port_registration_callback(
		_jackClient,
		OnPortRegistration,
		IntPtr.Zero
			);
			jack_on_shutdown(_jackClient, OnJackShutdown, IntPtr.Zero);
			int jackActivateStatus = jack_activate(_jackClient);
			return jackActivateStatus == 0;
		}
		
		public static bool IsActive {
			get {
				return _jackClient != IntPtr.Zero;
			}
		}
				
		private static JackPort GetJackPortData(uint portId)
		{
			try {
				IntPtr portPointer = jack_port_by_id(_jackClient, portId);
				if (portPointer != IntPtr.Zero) {
					string portName = Marshal.PtrToStringAnsi(jack_port_name(portPointer));
					PortType portType = PortType.Undefined;
					JackPortFlags portFlags = (JackPortFlags)jack_port_flags(portPointer);
					if ((portFlags & JackPortFlags.JackPortIsInput) == JackPortFlags.JackPortIsInput) {
						portType = PortType.Input;
					} else if ((portFlags & JackPortFlags.JackPortIsOutput) == JackPortFlags.JackPortIsOutput) {
						portType = PortType.Output;
					}
					ConnectionType connectionType = ConnectionType.Undefined;
					string connectionTypeName = Marshal.PtrToStringAnsi(jack_port_type(portPointer));
					if (connectionTypeName == JACK_DEFAULT_AUDIO_TYPE) {
						connectionType = ConnectionType.JackAudio;
					} else if (connectionTypeName == JACK_DEFAULT_MIDI_TYPE) {
						connectionType = ConnectionType.JackMidi;
					}
					JackPort newPort = new JackPort(
			portName,
			portId,
			portPointer,
			portType,
			connectionType
					);
					return newPort;
				}
				return null;
			} catch (Exception e) {
#if DEBUG
				Console.WriteLine (e.Message);
#endif
				return null;
			}
		}
		
		internal static bool Disconnect(Port outputPort, Port inputPort)
		{
			if (outputPort.PortType != PortType.Output || inputPort.PortType != PortType.Input || outputPort.ConnectionType != inputPort.ConnectionType) {
				return false;
			}
			string outPortName = _portMapper.Where(map => map.ClientName == outputPort.ClientName 
				&& map.Name == outputPort.Name 
				&& map.ConnectionType == outputPort.ConnectionType 
				&& map.PortType == outputPort.PortType
			).Select(map => map.JackPortName).First();
			string inPortName = _portMapper.Where(map => map.ClientName == inputPort.ClientName 
				&& map.Name == inputPort.Name 
				&& map.ConnectionType == inputPort.ConnectionType 
				&& map.PortType == inputPort.PortType
			).Select(map => map.JackPortName).First();
			return jack_disconnect(_jackClient, outPortName, inPortName) == 0;			
		}
		
		internal static bool Connect(Port outputPort, Port inputPort)
		{
			if (outputPort.PortType != PortType.Output || inputPort.PortType != PortType.Input || outputPort.ConnectionType != inputPort.ConnectionType) {
				return false;
			}
			string outPortName = _portMapper.Where(map => map.ClientName == outputPort.ClientName 
				&& map.Name == outputPort.Name 
				&& map.ConnectionType == outputPort.ConnectionType 
				&& map.PortType == outputPort.PortType
			).Select(map => map.JackPortName).First();
			string inPortName = _portMapper.Where(map => map.ClientName == inputPort.ClientName 
				&& map.Name == inputPort.Name 
				&& map.ConnectionType == inputPort.ConnectionType 
				&& map.PortType == inputPort.PortType
			).Select(map => map.JackPortName).First();
			return jack_connect(_jackClient, outPortName, inPortName) == 0;
		}
		
		internal static IEnumerable<Port> GetPorts(ConnectionType connectionType)
		{		
			var mappedPorts = _portMapper.Where(portMap => portMap.ConnectionType == connectionType)
		.Select(portMap => portMap as Port);
			if (mappedPorts.Any()) {
				return mappedPorts;
			}
			var ports = new List<Port>();
			for (uint i = 0; true; i++) {
				var newPort = GetJackPortData(i);
				if (newPort == null) {
					break;
				}
				_portMapper.Add(newPort);
				if (newPort.ConnectionType == connectionType) {
					ports.Add(newPort);
				}
			}
			return ports;
		}
		
		internal static IEnumerable<IConnection> GetConnections(ConnectionType connectionType)
		{
			return _connections;
		}
	}
}