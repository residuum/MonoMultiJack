// 
// AlsaMidiManager.cs
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
using System.Linq;
using System.Collections.Generic;
using GLib;
using MonoMultiJack.ConnectionWrapper.Alsa.Types;

namespace MonoMultiJack.ConnectionWrapper.Alsa
{
	public class AlsaMidiManager : IConnectionManager
	{
		List<AlsaPort> _portMapper = new List<AlsaPort> ();
		List<AlsaMidiConnection> _connections = new List<AlsaMidiConnection> ();
	
		public AlsaMidiManager ()
		{
			LibAsoundWrapper.Activate ();
			
			Timeout.Add (2000, new TimeoutHandler (CheckForChanges));
		}
		
		~AlsaMidiManager ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
	
		protected virtual void Dispose (bool isDisposing)
		{
			LibAsoundWrapper.DeActivate ();
		}
		
	#region IConnectionManager implementation
		public event ConnectionEventHandler ConnectionHasChanged;
		public event ConnectionEventHandler BackendHasExited;

		public void Connect (IConnectable outlet, IConnectable inlet)
		{
			foreach (KeyValuePair<Port, Port> portPair in EnumerableHelper.PairPorts(outlet, inlet)) {
				ConnectPorts (portPair.Key, portPair.Value);
			}
		}

		void ConnectPorts(Port outPort, Port inPort)
		{
			AlsaPort alsaOutPort = _portMapper.FirstOrDefault (p => p == outPort);
			AlsaPort alsaInPort = _portMapper.FirstOrDefault (p => p == inPort);
			if (alsaOutPort == null || alsaInPort == null 
			    || outPort.FlowDirection != FlowDirection.Out || inPort.FlowDirection != FlowDirection.In) {
				return;
			}
			LibAsoundWrapper.Connect (alsaOutPort, alsaInPort);
		}

		public void Disconnect (IConnectable outlet, IConnectable inlet)
		{
			foreach(Port outPort in outlet.Ports){
				foreach(Port inPort in inlet.Ports){
					DisconnectPort(outPort, inPort);
				}
			}
		}

		void DisconnectPort(Port outPort, Port inPort){
			AlsaPort alsaOutPort = _portMapper.FirstOrDefault (p => p == outPort);
			AlsaPort alsaInPort = _portMapper.FirstOrDefault (p => p == inPort);
			if (alsaOutPort == null || alsaInPort == null 
			    || outPort.FlowDirection != FlowDirection.Out || outPort.ConnectionType != ConnectionType
				|| inPort.FlowDirection != FlowDirection.In || inPort.ConnectionType != ConnectionType) {
				return;
			}
			LibAsoundWrapper.Disconnect (alsaOutPort, alsaInPort);
		}

		public ConnectionType ConnectionType {
			get {
				return ConnectionType.AlsaMidi;
			}
		}

		public bool IsActive {
			get {
				return true;
			}
		}

		IEnumerable<Client> ClientsFromPorts (IEnumerable<AlsaPort> ports)
		{
			IEnumerable<IGrouping<Client, AlsaPort>> clientGroups = 
				ports.GroupBy (p => new Client (p.ClientName, p.FlowDirection, p.ConnectionType));
			List<Client> clients = new List<Client> ();
			foreach (IGrouping<Client, AlsaPort> clientGroup in clientGroups) {
				Client newClient = clientGroup.Key;
				foreach (AlsaPort port in clientGroup) {
					newClient.AddPort (port);
				}
				clients.Add (newClient);
			}
			return clients;
		}

		public IEnumerable<Client> Clients {
			get {
				_portMapper = LibAsoundWrapper.GetPorts ().ToList ();
				return ClientsFromPorts (_portMapper);
			}
		}

		public IEnumerable<IConnection> Connections {
			get {
				_connections = LibAsoundWrapper.GetConnections (_portMapper).ToList ();
				return _connections.Cast<IConnection> ();
			}
		}

		public string Name {
			get{
				return "Alsa MIDI";
			}
		}
		#endregion

		bool CheckForChanges ()
		{
			List<AlsaPort> newPorts;
			List<Port> obsoletePorts;
			UpdatePortInformation (
		LibAsoundWrapper.GetPorts (),
		ref _portMapper,
		out newPorts,
		out obsoletePorts
			);
			List<IConnection> newConnections;
			List<IConnection> obsoleteConnections;
			
			UpdateConnectionInformation (
		LibAsoundWrapper.GetConnections (_portMapper),
		ref _connections,
		out newConnections,
		out obsoleteConnections
			);

			if (newPorts.Any () || newConnections.Any ()) {
				var newEventArgs = new ConnectionEventArgs ();
				newEventArgs.ChangeType = ChangeType.New;
				newEventArgs.Connectables = ClientsFromPorts(newPorts);
				newEventArgs.Connections = newConnections;
			}
			if (obsoletePorts.Any ()) {
				var oldEventArgs = new ConnectionEventArgs ();
				oldEventArgs.ChangeType = ChangeType.Deleted;
				oldEventArgs.Connectables = obsoletePorts;
				oldEventArgs.Connections = obsoleteConnections;
				if (ConnectionHasChanged != null){
					ConnectionHasChanged (this, oldEventArgs);
				}
			}
			return true;
		}

		void UpdatePortInformation (IEnumerable<AlsaPort> allPorts, ref List<AlsaPort> mappedPorts, out List<AlsaPort> newPorts, out List<Port> obsoletePorts)
		{
			newPorts = new List<AlsaPort> ();
			obsoletePorts = new List<Port> ();

			foreach (AlsaPort port in allPorts) {
				AlsaPort foundPort = mappedPorts.FirstOrDefault (p => p == port);

				if (foundPort == null) {
					newPorts.Add (port);
					mappedPorts.Add (port);
				}
			}
	    
			foreach (AlsaPort oldPort in mappedPorts) {
				AlsaPort mappedPort = allPorts.FirstOrDefault (p => p == oldPort);
				if (mappedPort == null) {
					obsoletePorts.Add (oldPort);
				}
			}
			// Remove obsolete ports
			foreach (AlsaPort obsoletePort in obsoletePorts) {
				mappedPorts.Remove (obsoletePort);
			}
		}

		void UpdateConnectionInformation (IEnumerable<AlsaMidiConnection> allConnections, ref List<AlsaMidiConnection> mappedConnections, out List<IConnection> newConnections, out List<IConnection> obsoleteConnections)
		{
			newConnections = new List<IConnection> ();
			obsoleteConnections = new List<IConnection> ();

			foreach (AlsaMidiConnection conn in allConnections) {
				AlsaMidiConnection foundConn = mappedConnections.FirstOrDefault (p => p == conn);

				if (foundConn == null) {
					newConnections.Add (conn);
					mappedConnections.Add (conn);
				}
			}
	    
			foreach (AlsaMidiConnection oldConn in mappedConnections) {
				AlsaMidiConnection mappedPort = allConnections.FirstOrDefault (p => p == oldConn);
				if (mappedPort == null) {
					obsoleteConnections.Add (oldConn);
				}
			}
			// Remove obsolete connections
			foreach (AlsaMidiConnection obsoletePort in obsoleteConnections) {
				mappedConnections.Remove (obsoletePort);
			}
		}
	}
}