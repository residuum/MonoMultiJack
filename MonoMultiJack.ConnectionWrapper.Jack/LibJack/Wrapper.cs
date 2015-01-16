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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using MonoMultiJack.ConnectionWrapper.Jack.Types;

namespace MonoMultiJack.ConnectionWrapper.Jack.LibJack
{
	/// <summary>
	/// Wrapper class for libjack. This file contains the main logic.
	/// </summary>
	internal static class Wrapper
	{		
		static IntPtr _jackClient = IntPtr.Zero;
		static List<JackPort> _portMapper = new List<JackPort> ();
		static List<IConnection> _connections = new List<IConnection> ();
		static readonly string ClientName = "MonoMultiJack"
			+ (DateTime.Now.Ticks / 10000000).ToString (CultureInfo.InvariantCulture).Substring (6);
		static readonly Definitions.JackPortConnectCallback _onPortConnect = OnPortConnect;
		static readonly Definitions.JackPortRegistrationCallback _onPortRegistration = OnPortRegistration;
		static readonly Definitions.JackShutdownCallback _onJackShutdown = OnJackShutdown;
		static readonly Definitions.JackXRunCallback _onJackXrun = OnJackRun;

		static void OnPortRegistration (uint port, int register, IntPtr args)
		{
			ConnectionEventArgs eventArgs = new ConnectionEventArgs {
				MessageType = MessageType.Change
			};
			ConnectionType connectionType = ConnectionType.Undefined;
			if (register > 0) {
				JackPort newPort = GetJackPortData (port);
				_portMapper.Add (newPort);
				eventArgs.Message = "New port registered.";
				connectionType = newPort.ConnectionType;
				eventArgs.ChangeType = ChangeType.New;
				List<IConnectable> clients = new List<IConnectable> ();
				Client newClient = new Client (newPort.ClientName, newPort.FlowDirection, newPort.ConnectionType);
				newClient.AddPort (newPort);
				clients.Add (newClient);
				eventArgs.Connectables = clients;
			} else {
				JackPort oldPort = _portMapper.FirstOrDefault (map => map.Id == port);
				if (oldPort != null) {
					List<Port> ports = new List<Port> ();
					ports.Add (oldPort);
					eventArgs.Connectables = ports;
					eventArgs.ChangeType = ChangeType.Deleted;						
					connectionType = oldPort.ConnectionType;
					_portMapper.Remove (oldPort);
				}
				eventArgs.Message = "Port unregistered.";
			}
			eventArgs.ConnectionType = connectionType;
			if (PortOrConnectionHasChanged != null) {
				PortOrConnectionHasChanged (null, eventArgs);
			}
		}

		static IConnection MapConnection (JackPort outPort, JackPort inPort)
		{
			IConnection newConn = null;
			switch (outPort.ConnectionType) {
			case ConnectionType.JackAudio:
				newConn = new JackAudioConnection ();
				break;
			case ConnectionType.JackMidi:
				newConn = new JackMidiConnection ();
				break;
			}
			Debug.Assert (newConn != null, "New connection is null");
			newConn.OutPort = outPort;
			newConn.InPort = inPort;
			return newConn;
		}

		static void OnPortConnect (uint a, uint b, int connect, IntPtr args)
		{
			ConnectionEventArgs eventArgs = new ConnectionEventArgs {
				MessageType = MessageType.Change
			};
			JackPort outPort = _portMapper.First (map => map.JackPortPointer == Invoke.jack_port_by_id (_jackClient, a));
			JackPort inPort = _portMapper.First (map => map.JackPortPointer == Invoke.jack_port_by_id (_jackClient, b));
			if (connect != 0) {
				List<IConnection> connections = new List<IConnection> ();
				IConnection newConn = MapConnection (outPort, inPort);
				_connections.Add (newConn);
				connections.Add (newConn);
				eventArgs.Connections = connections;					
				eventArgs.ConnectionType = newConn.ConnectionType;
				eventArgs.ChangeType = ChangeType.New;
				eventArgs.Message = "New connection established";
			} else {
				IEnumerable<IConnection> oldConn = _connections.Where (conn => conn.InPort == inPort
					&& conn.OutPort == outPort
				);
				eventArgs.Connections = oldConn.ToList ();
				eventArgs.ChangeType = ChangeType.Deleted;					
				eventArgs.ConnectionType = inPort.ConnectionType;
				_connections = _connections.Where (conn => conn.InPort != inPort || conn.OutPort != outPort)
			.ToList ();
				eventArgs.Message = "Connection deleted";
			}
			if (PortOrConnectionHasChanged != null) {
				PortOrConnectionHasChanged (null, eventArgs);
			}
		}

		static void OnJackShutdown (IntPtr args)
		{
			_jackClient = IntPtr.Zero;
			_portMapper.Clear ();
			if (BackendHasChanged != null) {
				BackendHasChanged (null, new ConnectionEventArgs {
					Message = string.Format ("Backend has exited"),
					ChangeType = ChangeType.BackendExited,
					MessageType = MessageType.Change
				});
			}
		}

		static void OnJackRun (IntPtr args)
		{
			float xrunDelay = Invoke.jack_get_xrun_delayed_usecs (_jackClient);
			if (xrunDelay > 0) {
				if (BackendHasChanged != null) {
					BackendHasChanged (null, new ConnectionEventArgs {
						Message = string.Format ("Xrun occurred: {0:0.###} ms", xrunDelay),
						ChangeType = ChangeType.Information,
						MessageType = MessageType.Info
					});
				}
			}
		}

		internal static event ConnectionEventHandler PortOrConnectionHasChanged;
		internal static event ConnectionEventHandler BackendHasChanged;

		internal static bool ConnectToServer ()
		{
			if (_jackClient == IntPtr.Zero) {
				_jackClient = Invoke.jack_client_open (ClientName, 1, IntPtr.Zero);
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
				Invoke.jack_client_close (_jackClient);
			}
		}

		/// <summary>
		/// Activates jack client
		/// </summary>
		static bool Activate ()
		{
			Invoke.jack_set_xrun_callback (_jackClient, _onJackXrun, IntPtr.Zero);
			Invoke.jack_set_port_connect_callback (_jackClient, _onPortConnect, IntPtr.Zero);
			Invoke.jack_set_port_registration_callback (_jackClient, _onPortRegistration, IntPtr.Zero);
			Invoke.jack_on_shutdown (_jackClient, _onJackShutdown, IntPtr.Zero);
			int jackActivateStatus = Invoke.jack_activate (_jackClient);
			return jackActivateStatus == 0;
		}

		public static bool IsActive {
			get {
				return _jackClient != IntPtr.Zero;
			}
		}

		static JackPort GetJackPortData (uint portId)
		{
			IntPtr portPointer = Invoke.jack_port_by_id (_jackClient, portId);			
			return MapPort (portPointer, portId);
		}

		static FlowDirection GetFlowDirection (IntPtr portPointer)
		{
			FlowDirection portType = FlowDirection.Undefined;
			JackPortFlags portFlags = (JackPortFlags)Invoke.jack_port_flags (portPointer);
			if ((portFlags & JackPortFlags.JackPortIsInput) == JackPortFlags.JackPortIsInput) {
				portType = FlowDirection.In;
			} else if ((portFlags & JackPortFlags.JackPortIsOutput) == JackPortFlags.JackPortIsOutput) {
				portType = FlowDirection.Out;
			}
			return portType;
		}

		static ConnectionType GetConnectionType (IntPtr portPointer)
		{
			ConnectionType connectionType = ConnectionType.Undefined;
			string connectionTypeName = Invoke.jack_port_type (portPointer).PtrToString ();
			switch (connectionTypeName) {
			case Definitions.JACK_DEFAULT_AUDIO_TYPE:
				connectionType = ConnectionType.JackAudio;
				break;
			case Definitions.JACK_DEFAULT_MIDI_TYPE:
				connectionType = ConnectionType.JackMidi;
				break;
			}
			return connectionType;
		}

		static JackPort MapPort (IntPtr portPointer, uint portId)
		{
			if (portPointer == IntPtr.Zero) {
				return null;
			}
			string portName = Invoke.jack_port_name (portPointer).PtrToString ();
			if (!string.IsNullOrEmpty (portName)) {
				FlowDirection portType = GetFlowDirection (portPointer);
				ConnectionType connectionType = GetConnectionType (portPointer);
				JackPort newPort = new JackPort (portName, portPointer, portType, connectionType, portId);
				return newPort;
			}
			return null;
		}

		internal static bool Disconnect (Port outputPort, Port inputPort)
		{
			if (outputPort.FlowDirection != FlowDirection.Out || inputPort.FlowDirection != FlowDirection.In || outputPort.ConnectionType != inputPort.ConnectionType) {
				return false;
			}
			_portMapper = UpdatePortList (_portMapper).ToList ();
			string outPortName = _portMapper.Where (map => map == outputPort)
				.Select (map => map.JackPortName).First ();
			string inPortName = _portMapper.Where (map => map == inputPort)
				.Select (map => map.JackPortName).First ();
			if (_connections.Any (c => c.InPort == inputPort && c.OutPort == outputPort)) {
				return Invoke.jack_disconnect (_jackClient, outPortName, inPortName) == 0;			
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
			_portMapper = UpdatePortList (_portMapper).ToList ();
			string outPortName = _portMapper.First (map => map == outputPort).JackPortName;
			string inPortName = _portMapper.First (map => map == inputPort).JackPortName;
			return Invoke.jack_connect (_jackClient, outPortName, inPortName) == 0;
		}

		internal static IEnumerable<JackPort> GetPorts (ConnectionType connectionType)
		{		
			if (!_portMapper.Any ()) {
				_portMapper = GetInitialPorts ().ToList ();
				_connections = GetInitialConnections (_portMapper).ToList ();
			} else {
				_portMapper = UpdatePortList (_portMapper).ToList ();
			}
			return _portMapper.Where (portMap => portMap.ConnectionType == connectionType);
		}

		static IEnumerable<JackPort> UpdatePortList (IEnumerable<JackPort> existingPorts)
		{
			foreach (JackPort existing in existingPorts) {
				JackPort current = GetJackPortData (existing.Id);
				if (current.Name != existing.Name) { 					
					existing.Client.ReplacePort (existing, current);
					if (PortOrConnectionHasChanged != null) {
						ConnectionEventArgs args = new ConnectionEventArgs {
							ConnectionType = current.ConnectionType,
							Connectables = new List<IConnectable> { current },
							ChangeType = ChangeType.Content,
							MessageType = MessageType.Change
						};
						PortOrConnectionHasChanged (null, args);
					}
					yield return current;
				} else {
					yield return existing;
				}
			}
		}

		static int GetPortCount ()
		{
			IntPtr portNamesPtr = Invoke.jack_get_ports (_jackClient, "", "", 0);
			if (portNamesPtr == IntPtr.Zero) {
				return 0;
			}

			string[] portNames = portNamesPtr.PtrToStringArray ();
			Invoke.jack_free (portNamesPtr);
			return portNames.Length;
		}

		static IEnumerable<JackPort> GetInitialPorts ()
		{
			int portCount = GetPortCount ();
			List<JackPort> newPorts = new List<JackPort> ();
			for (uint i = 0; i < 256 && newPorts.Count < portCount; i++) {
				JackPort newPort = GetJackPortData (i);
				if (newPort != null) {
					newPorts.Add (newPort);
				}
			}
			return newPorts;
		}

		static IEnumerable<IConnection> GetInitialConnections (List<JackPort> portMapper)
		{
			IEnumerable<JackPort> outPorts = portMapper.Where (p => p.FlowDirection == FlowDirection.Out);
			IEnumerable<JackPort> inPorts = portMapper.Where (p => p.FlowDirection == FlowDirection.In).ToList ();

			foreach (var outPort in outPorts) {
				IntPtr inPortNamePtr = Invoke.jack_port_get_all_connections (_jackClient, outPort.JackPortPointer);
				if (inPortNamePtr == IntPtr.Zero) {
					continue;
				}
				string[] inPortNames = inPortNamePtr.PtrToStringArray ();
				foreach (var name in inPortNames) {
					IntPtr inPortPtr = Invoke.jack_port_by_name (_jackClient, name);
					JackPort inPort = inPorts.FirstOrDefault (p => p.JackPortPointer == inPortPtr);
					if (inPort == null)
						continue;

					switch (inPort.ConnectionType) {
					case ConnectionType.JackAudio:
						yield return new JackAudioConnection {
							InPort = inPort,
							OutPort = outPort
						};
						break;
					case ConnectionType.JackMidi:
						yield return new JackMidiConnection {
							InPort = inPort,
							OutPort = outPort
						};
						break;
					}
				}
				Invoke.jack_free (inPortNamePtr);
			}
		}

		internal static IEnumerable<IConnection> GetConnections (ConnectionType connectionType)
		{
			return _connections.Where (c => c.ConnectionType == connectionType);
		}
	}
}