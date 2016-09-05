// 
// JackConnectionManager.cs
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
using System.Linq;
using Xwt;
using Mmj.ConnectionWrapper.Jack.LibJack;
using Mmj.ConnectionWrapper.Jack.Types;

namespace Mmj.ConnectionWrapper.Jack
{
	public abstract class JackConnectionManager : IConnectionManager
	{
		static IDisposable _timeout;

		protected JackConnectionManager ()
		{
			Wrapper.PortOrConnectionHasChanged += OnLibJackWrapperHasChanged;
			Wrapper.BackendHasChanged += OnJackChanged;
		}

		~JackConnectionManager ()
		{
			Dispose (false);
			GC.SuppressFinalize (this);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool isDisposing)
		{
			if (isDisposing) {
				Wrapper.PortOrConnectionHasChanged -= OnLibJackWrapperHasChanged;
				Wrapper.BackendHasChanged -= OnJackChanged;
			}
			if (_timeout != null) {
				_timeout.Dispose ();
			}

			Wrapper.Close ();
		}

		#region IConnectionManager implementation

		public event ConnectionEventHandler ConnectionHasChanged;
		public event ConnectionEventHandler BackendHasChanged;

		public abstract ConnectionType ConnectionType { get; }

		public bool IsActive {
			get { return Wrapper.IsActive; }
		}

		public IEnumerable<Client> Clients {
			get {
				if (IsActive) {
					IEnumerable<IGrouping<Client, JackPort>> portGroups = Wrapper.GetPorts (ConnectionType)
						.GroupBy (p => new Client (p.ClientName, p.FlowDirection, p.ConnectionType));
					List<Client> clients = new List<Client> ();
					foreach (IGrouping<Client, JackPort> portGroup in portGroups) {
						Client newClient = portGroup.Key;
						foreach (Port port in portGroup) {
							newClient.AddPort (port);
						}
						clients.Add (newClient);
					}
					return clients;
				} else {
					if (_timeout == null) {
						_timeout = Application.TimeoutInvoke (2000, ConnectToServer);
					}
					return null;
				}
			}
		}

		public void Connect (IEnumerable<IConnectable> outlets, IEnumerable<IConnectable> inlets)
		{
			foreach (KeyValuePair<Port, Port> portPair in EnumerableHelper.PairPorts(outlets, inlets)) {
				Wrapper.Connect (portPair.Key, portPair.Value);
			}
		}

		public void Disconnect (IEnumerable<IConnectable> outlets, IEnumerable<IConnectable> inlets)
		{
			foreach (KeyValuePair<Port, Port> portPair in EnumerableHelper.PairAll(outlets, inlets)) {
				Wrapper.Disconnect (portPair.Key, portPair.Value);
			}
		}

		public IEnumerable<IConnection> Connections {
			get {
				return Wrapper.GetConnections (ConnectionType);
			}
		}

		public abstract string Name { get; }

		#endregion

		bool ConnectToServer ()
		{
			if (Wrapper.ConnectToServer ()) {
				ConnectionEventArgs eventArgs = new ConnectionEventArgs {
					Connectables = Clients,
					Connections = Connections,
					ChangeType = ChangeType.New,
					Message = "Connection to Jackd established.",
					MessageType = MessageType.Info
				};
				if (ConnectionHasChanged != null) {
					ConnectionHasChanged (this, eventArgs);
				}
				_timeout.Dispose ();
				_timeout = null;
				return false;
			} 
			return true;
		}

		void OnLibJackWrapperHasChanged (object sender, ConnectionEventArgs args)
		{
			if (args.ConnectionType == ConnectionType && ConnectionHasChanged != null) {
				ConnectionHasChanged (this, args);	
			}
		}

		void OnJackChanged (object sender, ConnectionEventArgs args)
		{
			if (BackendHasChanged != null) {
				BackendHasChanged (this, args);
			}
			if (args.ChangeType == ChangeType.BackendExited) {
				if (_timeout == null) {
					_timeout = Application.TimeoutInvoke (2000, ConnectToServer);
				}
			}
		}
	}
}
