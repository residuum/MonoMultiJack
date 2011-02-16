// 
// JackConnectionManager.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2010 Thomas Mayer
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
using GLib;

namespace MonoMultiJack.ConnectionWrapper.Jack
{
	public abstract class JackConnectionManager : IConnectionManager
	{
		protected JackConnectionManager ()
		{
			LibJackWrapper.PortOrConnectionHasChanged += LibJackWrapperHasChanged;
			LibJackWrapper.JackHasShutdown += OnJackShutdown;
		}
		
		#region IConnectionManager implementation
		public event ConnectionEventHandler ConnectionHasChanged;

		public event ConnectionEventHandler BackendHasExited;
		
		public virtual ConnectionType ConnectionType
		{
			get {return ConnectionType.Undefined;}
		}

		public bool IsActive 
		{
			get { return LibJackWrapper.IsActive; }
		}
		
		public IEnumerable<Port> Ports 
		{
			get 
			{
				if (IsActive)
				{
					return LibJackWrapper.GetPorts (ConnectionType);
				}
				else
				{
					GLib.Timeout.Add (2000, new GLib.TimeoutHandler (ConnectToServer));
					LibJackWrapper.ConnectToServer ();
					return null;
				}
			}
		}
		public bool Connect (Port outPort, Port inPort)
		{
			if (outPort.ConnectionType != ConnectionType && outPort.PortType != PortType.Output
				&& inPort.ConnectionType != ConnectionType && outPort.PortType != PortType.Input)
			{
				return false;
			}
			else
			{
				return LibJackWrapper.Connect(outPort, inPort);
			}
		}

		public bool Disconnect (Port outPort, Port inPort)
		{
			return LibJackWrapper.Disconnect(outPort, inPort);
		}

		public IEnumerable<IConnection> Connections 
		{
			get 
			{
				return LibJackWrapper.GetConnections (ConnectionType);
			}
		}
		#endregion

		private bool ConnectToServer ()
		{
			if (LibJackWrapper.ConnectToServer ())
			{
				var eventArgs = new ConnectionEventArgs ();
				eventArgs.Ports = Ports;
				eventArgs.Connections = Connections;
				eventArgs.ChangeType = ChangeType.New;
				eventArgs.Message = "Connection to Jackd established";
				eventArgs.MessageType = MessageType.Info;
				ConnectionHasChanged (this, eventArgs);
				return false;
			}
			else
			{
				return true;
			}
		}
		
		private void LibJackWrapperHasChanged (object sender, ConnectionEventArgs args)
		{
#if DEBUG
			Console.WriteLine (args.Message);
#endif
			if (args.ConnectionType == ConnectionType)
			{
				ConnectionHasChanged (this, args);	
			}
		}
		
		private void OnJackShutdown(object sender, ConnectionEventArgs args)
		{
			BackendHasExited(this, args);
			GLib.Timeout.Add (2000, new GLib.TimeoutHandler (ConnectToServer));
		}
	}
}
