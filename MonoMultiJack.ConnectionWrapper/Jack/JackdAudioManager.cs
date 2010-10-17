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
using GLib;
using System;
using System.Collections.Generic;

namespace MonoMultiJack.ConnectionWrapper.Jack
{
	public class JackdAudioManager : IConnectionManager
	{
		private readonly IConnectionType _connectionType = new JackdAudioConnectionType();
		
		public JackdAudioManager ()
		{
		}
				
		
		#region IConnectionManager implementation
		public event ConnectionEventHandler ConnectionHasChanged;

		public event ConnectionEventHandler BackendHasExited;
		
		public IConnectionType ConnectionType
		{
			get {return _connectionType;}
		}

		public bool IsActive 
		{
			get { return LibJackWrapper.IsActive; }
		}
		
		public IEnumerable<IPort> Ports 
		{
			get 
			{
				if (IsActive)
				{
					var ports = new List<IPort> ();
					var portStrings = LibJackWrapper.GetPorts (PortType.Input, true);
					foreach (var portString in portStrings) 
					{
						string[] splittedString = portString.Split (new[] { ':' });
						ports.Add (new JackdAudioPort (splittedString[1], splittedString[0], PortType.Input));
					}
				
					portStrings = LibJackWrapper.GetPorts (PortType.Output, true);
					foreach (var portString in portStrings) 
					{
						string[] splittedString = portString.Split (new[] { ':' });
						ports.Add (new JackdAudioPort (splittedString[1], splittedString[0], PortType.Output));
					}
					return ports;
				}
				else
				{
					GLib.Timeout.Add(2000, new GLib.TimeoutHandler(ConnectToServer));
					LibJackWrapper.ConnectToServer();
					return null;
				}
			}
		}
		
		#endregion
		
		private bool ConnectToServer ()
		{
			if (LibJackWrapper.ConnectToServer ())
			{
				var eventArgs = new ConnectionEventArgs ();
				eventArgs.Ports = Ports;
				eventArgs.Message = "Connection to Jackd established";
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
