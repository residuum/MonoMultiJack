// 
// JackConnectionManager.cs
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

namespace MonoMultiJack.ConnectionWrapper.Jack
{
	public abstract class JackConnectionManager : IConnectionManager
	{
		protected JackConnectionManager ()
		{
			LibJackWrapper.PortOrConnectionHasChanged += OnLibJackWrapperHasChanged;
			LibJackWrapper.JackHasShutdown += OnJackShutdown;
		}

		~JackConnectionManager ()
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
			if (isDisposing) {
				LibJackWrapper.PortOrConnectionHasChanged -= OnLibJackWrapperHasChanged;
				LibJackWrapper.JackHasShutdown -= OnJackShutdown;
			}

			LibJackWrapper.Close ();
		}
		
		#region IConnectionManager implementation
		public event ConnectionEventHandler ConnectionHasChanged;
		public event ConnectionEventHandler BackendHasExited;
		
		public virtual ConnectionType ConnectionType {
			get { return ConnectionType.Undefined;}
		}

		public bool IsActive {
			get { return LibJackWrapper.IsActive; }
		}
		
		public IEnumerable<IConnectable> Clients {
			get {
				if (IsActive) {
					var portGroups = LibJackWrapper.GetPorts(ConnectionType)
						.GroupBy(p => new {p.ClientName, p.ConnectionType, p.FlowDirection});
					List<Client> clients = new List<Client>();
					foreach(var portGroup in portGroups){
						IEnumerable<Port> ports = portGroup.ToList();
						Client newClient = new Client(portGroup.Key.ClientName,
						                              portGroup.Key.FlowDirection, 
						                              portGroup.Key.ConnectionType);
						foreach(Port port in ports){
							newClient.AddPort(port);
						}
						clients.Add(newClient);
					}
					return clients;
				} else {
					GLib.Timeout.Add (2000, new GLib.TimeoutHandler (ConnectToServer));
					LibJackWrapper.ConnectToServer ();
					return null;
				}
			}
		}

		public bool Connect (IConnectable outlet, IConnectable inlet)
		{
			bool connected = true;
			foreach(KeyValuePair<Port, Port> portPair in EnumerableHelper.PairPorts(outlet, inlet)){
				if (!LibJackWrapper.Connect(portPair.Key, portPair.Value)) {
					connected = false;
				}
			}
			return connected;
		}

		public bool Disconnect (IConnectable outlet, IConnectable inlet)
		{
			bool disconnected = true;
			foreach(KeyValuePair<Port, Port> portPair in EnumerableHelper.PairPorts(outlet, inlet)){
				if (!LibJackWrapper.Disconnect(portPair.Key, portPair.Value)) {
					disconnected = false;
				}
			}
			return disconnected;
		}

		public IEnumerable<IConnection> Connections {
			get {
				return LibJackWrapper.GetConnections (ConnectionType);
			}
		}

		public virtual string Name {
			get {
				return "Jack";
			}
		}
		#endregion

		bool ConnectToServer ()
		{
			if (LibJackWrapper.ConnectToServer ()) {
				var eventArgs = new ConnectionEventArgs ();
				eventArgs.Connectables = Clients;
				eventArgs.Connections = Connections;
				eventArgs.ChangeType = ChangeType.New;
				eventArgs.Message = "Connection to Jackd established";
				eventArgs.MessageType = MessageType.Info;
				if (ConnectionHasChanged != null){
					ConnectionHasChanged (this, eventArgs);
				}
				return false;
			} 
			return true;
		}
		
		void OnLibJackWrapperHasChanged (object sender, ConnectionEventArgs args)
		{
#if DEBUG
			Console.WriteLine (args.Message);
#endif
			if (args.ConnectionType == ConnectionType && ConnectionHasChanged != null) {
				ConnectionHasChanged (this, args);	
			}
		}
		
		void OnJackShutdown (object sender, ConnectionEventArgs args)
		{
			if (BackendHasExited != null) {
				BackendHasExited (this, args);
			}
			GLib.Timeout.Add (2000, new GLib.TimeoutHandler (ConnectToServer));
		}
	}
}
