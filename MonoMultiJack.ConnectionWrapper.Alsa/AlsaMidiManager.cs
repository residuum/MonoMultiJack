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

		void ConnectPorts (Port outPort, Port inPort)
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
			foreach (Port outPort in outlet.Ports) {
				foreach (Port inPort in inlet.Ports) {
					DisconnectPort (outPort, inPort);
				}
			}
		}

		void DisconnectPort (Port outPort, Port inPort)
		{
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
			get {
				return "Alsa MIDI";
			}
		}
		#endregion

		void SendMessage (IEnumerable<IConnectable> connectables, IEnumerable<IConnection> connections, ChangeType changeType)
		{
			if (!connectables.Any () && !connections.Any ()) {
				return;
			}
			ConnectionEventArgs oldEventArgs = new ConnectionEventArgs ();
			oldEventArgs.ChangeType = changeType;
			oldEventArgs.Connectables = connectables.ToList ();
			oldEventArgs.Connections = connections.ToList ();
			if (ConnectionHasChanged != null) {
				ConnectionHasChanged (this, oldEventArgs);
			}
		}

		bool CheckForChanges ()
		{
			List<AlsaPort> newPorts;
			List<AlsaPort> obsoletePorts;
			_portMapper = UpdateList<AlsaPort> (
				LibAsoundWrapper.GetPorts (), 
				_portMapper, 
				out newPorts, 
				out obsoletePorts
			).ToList ();

			List<AlsaMidiConnection> newConnections;
			List<AlsaMidiConnection> obsoleteConnections;
			_connections = UpdateList<AlsaMidiConnection> (
				LibAsoundWrapper.GetConnections (_portMapper).ToList (), 
				_connections, 
				out newConnections, 
				out obsoleteConnections
			).ToList ();
			
			SendMessage (obsoletePorts, obsoleteConnections, ChangeType.Deleted);
			IEnumerable<Client> newClients = ClientsFromPorts (newPorts);
			SendMessage (newClients, newConnections, ChangeType.New);
			return true;
		}

		IEnumerable<T> UpdateList<T> (IEnumerable<T> all, List<T> mapped, out List<T> added, out List<T> obsolete) where T : class
		{
			added = new List<T> ();
			obsolete = new List<T> ();
			foreach (T current in all.Where(e => !mapped.Contains(e))) {
				added.Add (current);
				mapped.Add (current);
			}
			foreach (T old in mapped.Where(e => !all.Contains(e))) {
				obsolete.Add (old);
			}
			return all;
		}
	}
}