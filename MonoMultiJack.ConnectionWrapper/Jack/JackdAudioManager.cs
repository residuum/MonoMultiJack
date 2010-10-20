// 
// JackdAudioConnection.cs
//  
// Author:
//       thomas <>
// 
// Copyright (c) 2010 thomas
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
	public class JackdAudioManager : IConnectionManager
	{
		
		public JackdAudioManager ()
		{
		}
				#region IConnectionManager implementation
		public event ConnectionEventHandler ConnectionHasChanged;

		public event ConnectionEventHandler BackendHasExited;
		
		public ConnectionType ConnectionType
		{
			get {return ConnectionType.JackdAudio;}
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
					var ports = new List<Port> ();
					var inPorts = LibJackWrapper.GetPorts (PortType.Input, ConnectionType);
					ports.AddRange (inPorts);
					var outPorts = LibJackWrapper.GetPorts (PortType.Output, ConnectionType);
					ports.AddRange(outPorts);
					return ports;
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
			if (outPort.ConnectionType != ConnectionType.JackdAudio && outPort.PortType != PortType.Output
				&& inPort.ConnectionType != ConnectionType.JackdAudio && outPort.PortType != PortType.Input)
			{
				return false;
			}
			else
			{
				throw new NotImplementedException ();
			}
		}

		public bool Disconnect (IConnection connection)
		{
			throw new NotImplementedException ();
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
				eventArgs.Message = "Connection to Jackd established";
				eventArgs.MessageType = MessageType.Info;
				ConnectionHasChanged(this, eventArgs);				
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
