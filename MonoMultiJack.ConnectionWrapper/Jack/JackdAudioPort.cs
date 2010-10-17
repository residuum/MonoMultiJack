// 
// JackdAudioPorts.cs
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
	public class JackdAudioPort : IPort
	{
		private readonly IConnectionType _connectionType = new JackdAudioConnectionType();
		
		#region IPort implementation
		public void Connect (IPort port)
		{
			if (PortType == port.PortType)
			{
				throw new ArgumentOutOfRangeException("Cannot Connect two " + PortType.ToString() + " ports.");
			}
			throw new NotImplementedException ();
		}

		public void Disconnect (IPort port)
		{
			throw new NotImplementedException ();
		}

		public string Name 
		{
			get; private set;
		}

		public string ClientName 
		{
			get; private set;
		}

		public PortType PortType 
		{
			get; private set;
		}
		
		public IConnectionType ConnectionType
		{
			get {return _connectionType;}
		}

		public List<IPort> Connections 
		{
			get; private set;
		}
		#endregion

		public JackdAudioPort (string name, string clientName, PortType portType)
		{
			Name = name;
			ClientName = clientName;
			PortType = portType;
		}
	}
}

