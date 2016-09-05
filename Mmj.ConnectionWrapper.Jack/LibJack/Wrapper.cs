// 
// Wrapper.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2016 Thomas Mayer
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
using Mmj.ConnectionWrapper.Jack.Types;
using JackSharp;
using JackSharp.Events;
using JackSharp.Ports;

namespace Mmj.ConnectionWrapper.Jack.LibJack
{
	/// <summary>
	/// Wrapper class for libjack. This file contains the main logic.
	/// </summary>
	internal static class Wrapper
	{
		static Controller _jackClient;
		static readonly List<JackPort> PortMapper = new List<JackPort> ();
		static List<IConnection> _connections = new List<IConnection> ();
		static readonly string ClientName = "MonoMultiJack"
		                                    + (DateTime.Now.Ticks / 10000000).ToString (CultureInfo.InvariantCulture).Substring (6);

		static void OnPortRegistration (object sender, PortRegistrationEventArgs args)
		{
			ConnectionEventArgs eventArgs = null;
			if (args.ChangeType == JackSharp.Events.ChangeType.New) {
				eventArgs = GetNewPortArgs (args);
			} else if (args.ChangeType == JackSharp.Events.ChangeType.Deleted) {
				if (!TryGetOldPortArgs (args, out eventArgs)) {
					return;
				}
			} else if (args.ChangeType == JackSharp.Events.ChangeType.Renamed) {
				JackPort oldPort = PortMapper.FirstOrDefault (map => map.PortReference == args.Port);
				if (oldPort == null) {
					// Cannot find it, treat is as new
					eventArgs = GetNewPortArgs (args);
				} else {
					eventArgs = GetRenamedPortArgs (args, oldPort);
				}
			}
			if (PortOrConnectionHasChanged != null && eventArgs != null) {
				PortOrConnectionHasChanged (null, eventArgs);
			}
		}

		static ConnectionEventArgs GetRenamedPortArgs (PortRegistrationEventArgs args, JackPort oldPort)
		{
			ConnectionEventArgs eventArgs = new ConnectionEventArgs {
				MessageType = MessageType.Change
			};
			PortMapper.Remove (oldPort);
			JackPort port = AddNewJackPort (args.Port);
			eventArgs.Connectables = new List<Port> { port };
			eventArgs.ChangeType = ChangeType.Content;
			eventArgs.ConnectionType = port.ConnectionType;
			eventArgs.Message = "Port renamed.";
			return eventArgs;
		}

		static bool TryGetOldPortArgs (PortRegistrationEventArgs args, out ConnectionEventArgs eventArgs)
		{
			eventArgs = new ConnectionEventArgs {
				MessageType = MessageType.Change
			};
			JackPort oldPort = PortMapper.FirstOrDefault (map => map.PortReference == args.Port);
			if (oldPort == null) {
				return false;
			}
			List<Port> ports = new List<Port> ();
			ports.Add (oldPort);
			eventArgs.Connectables = ports;
			eventArgs.ChangeType = ChangeType.Deleted;
			eventArgs.ConnectionType = oldPort.ConnectionType;
			PortMapper.Remove (oldPort);
			eventArgs.Message = "Port unregistered.";
			return true;
		}

		static ConnectionEventArgs GetNewPortArgs (PortRegistrationEventArgs args)
		{
			ConnectionEventArgs eventArgs = new ConnectionEventArgs {
				MessageType = MessageType.Change
			};
			var newPort = AddNewJackPort (args.Port);
			eventArgs.Message = "New port registered.";
			eventArgs.ChangeType = ChangeType.New;
			List<IConnectable> clients = new List<IConnectable> ();
			Client newClient = new Client (newPort.ClientName, newPort.FlowDirection, newPort.ConnectionType);
			eventArgs.ConnectionType = newClient.ConnectionType;
			newClient.AddPort (newPort);
			clients.Add (newClient);
			eventArgs.Connectables = clients;
			return eventArgs;
		}

		static JackPort AddNewJackPort (PortReference port)
		{
			JackPort newPort = new JackPort (port);
			PortMapper.Add (newPort);
			return newPort;
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
			Debug.Assert (newConn != null, "New connection is null.");
			newConn.OutPort = outPort;
			newConn.InPort = inPort;
			return newConn;
		}

		static void OnPortConnect (object sender, ConnectionChangeEventArgs args)
		{
			ConnectionEventArgs eventArgs = new ConnectionEventArgs {
				MessageType = MessageType.Change
			};
			JackPort outPort = PortMapper.FirstOrDefault (map => map.PortReference == args.Outlet);
			JackPort inPort = PortMapper.FirstOrDefault (map => map.PortReference == args.Inlet);
			if (outPort == null || inPort == null) {
				return;
			}
			if (args.ChangeType == JackSharp.Events.ChangeType.New) {
				List<IConnection> connections = new List<IConnection> ();
				IConnection newConn = MapConnection (outPort, inPort);
				_connections.Add (newConn);
				connections.Add (newConn);
				eventArgs.Connections = connections;					
				eventArgs.ConnectionType = newConn.ConnectionType;
				eventArgs.ChangeType = ChangeType.New;
				eventArgs.Message = "New connection established.";
			} else {
				IEnumerable<IConnection> oldConn = _connections.Where (conn => conn.InPort == inPort
				                                   && conn.OutPort == outPort);
				eventArgs.Connections = oldConn.ToList ();
				eventArgs.ChangeType = ChangeType.Deleted;					
				eventArgs.ConnectionType = inPort.ConnectionType;
				_connections = _connections.Where (conn => conn.InPort != inPort || conn.OutPort != outPort)
			.ToList ();
				eventArgs.Message = "Connection deleted.";
			}
			if (PortOrConnectionHasChanged != null) {
				PortOrConnectionHasChanged (null, eventArgs);
			}
		}

		static void OnJackShutdown (object sender, EventArgs args)
		{
			PortMapper.Clear ();
			if (BackendHasChanged != null) {
				BackendHasChanged (null, new ConnectionEventArgs {
					Message = "Backend has exited",
					ChangeType = ChangeType.BackendExited,
					MessageType = MessageType.Change
				});
			}
		}

		static void OnJackXRun (object sender, XrunEventArgs args)
		{
			if (args.XrunDelay > 0 && BackendHasChanged != null) {
				BackendHasChanged (null, new ConnectionEventArgs {
					Message = "Xrun occurred: {0:0.###} ms",
					Params = new object[] { args.XrunDelay },
					ChangeType = ChangeType.Information,
					MessageType = MessageType.Info
				});
			}
		}

		internal static event ConnectionEventHandler PortOrConnectionHasChanged;
		internal static event ConnectionEventHandler BackendHasChanged;

		internal static bool ConnectToServer ()
		{
			if (_jackClient == null) {
				_jackClient = CreateClient ();	
			}
			if (_jackClient.IsConnectedToJack) {
				return true;
			}
			if (!_jackClient.Start ()) {
				return false;
			}
			return true;
		}

		static Controller CreateClient ()
		{
			Controller controller = new Controller (ClientName);
			controller.ConnectionChanged += OnPortConnect;
			controller.PortChanged += OnPortRegistration;
			controller.Shutdown += OnJackShutdown;
			controller.Xrun += OnJackXRun;
			return controller;
		}

		/// <summary>
		/// Closes jack client
		/// </summary>
		internal static void Close ()
		{
			if (_jackClient.Stop ()) {
				_jackClient.ConnectionChanged -= OnPortConnect;
				_jackClient.PortChanged -= OnPortRegistration;
				_jackClient.Shutdown -= OnJackShutdown;
				_jackClient.Xrun -= OnJackXRun;
				_jackClient.Dispose ();
			}
		}

		public static bool IsActive { get { return _jackClient != null && _jackClient.IsConnectedToJack; } }

		internal static bool Disconnect (Port outputPort, Port inputPort)
		{
			if (outputPort.FlowDirection != FlowDirection.Out || inputPort.FlowDirection != FlowDirection.In || outputPort.ConnectionType != inputPort.ConnectionType) {
				return false;
			}
			JackPort outPort = PortMapper.First (map => map == outputPort);
			JackPort inPort = PortMapper.First (map => map == inputPort);
			if (_connections.Any (c => c.InPort == inputPort && c.OutPort == outputPort)) {
				return _jackClient.Disconnect (outPort.PortReference, inPort.PortReference);
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
			JackPort outPort = PortMapper.First (map => map == outputPort);
			JackPort inPort = PortMapper.First (map => map == inputPort);
			return _jackClient.Connect (outPort.PortReference, inPort.PortReference);
		}

		internal static IEnumerable<JackPort> GetPorts (ConnectionType connectionType)
		{	
			return PortMapper.Where (portMap => portMap.ConnectionType == connectionType);
		}

		internal static IEnumerable<IConnection> GetConnections (ConnectionType connectionType)
		{
			return _connections.Where (c => c.ConnectionType == connectionType);
		}
	}
}
