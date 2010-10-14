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

namespace MonoMultiJack.ConnectionWrapper.Jack
{
	public class JackdAudioManager : IConnectionManager<JackdAudioConnectionType>
	{
		public JackdAudioManager ()
		{
		}
				
		
		#region IConnectionManager implementation
		public event ConnectionEventHandler<JackdAudioConnectionType> ConnectionHasChanged;

		public event ConnectionEventHandler<JackdAudioConnectionType> BackendHasExited;

		public string TypeName {
			get { return "Jack Audio"; }
		}

		public bool IsActive {
			get { return LibJackWrapper.IsActive; }
		}
		public IEnumerable<IPort<JackdAudioConnectionType>> Ports {
			get 
			{
				var ports = new List<IPort<JackdAudioConnectionType>> ();
				var portStrings = LibJackWrapper.GetPorts (PortType.Input, true);
				foreach (var portString in portStrings) 
				{
					string[] splittedString = portString.Split (new[] { ':' });
					ports.Add (new JackdAudioPort (splittedString[1], splittedString[0], PortType.Input));
				}
				
				portStrings = LibJackWrapper.GetPorts (PortType.Input, true);
				foreach (var portString in portStrings) 
				{
					string[] splittedString = portString.Split (new[] { ':' });
					ports.Add (new JackdAudioPort (splittedString[1], splittedString[0], PortType.Output));
				}
				return ports;
			}
		}
		
		#endregion
	}
}

