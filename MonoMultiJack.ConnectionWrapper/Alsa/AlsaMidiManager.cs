// 
// AlsaMidiManager.cs
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
namespace MonoMultiJack.ConnectionWrapper.Alsa
{
	public class AlsaMidiManager : IConnectionManager
	{
		public AlsaMidiManager ()
		{
			LibAsoundWrapper.Activate ();
		}
		
		~AlsaMidiManager () 
		{
			LibAsoundWrapper.DeActivate();
		}
		
		#region IConnectionManager implementation
		public event ConnectionEventHandler ConnectionHasChanged;

		public event ConnectionEventHandler BackendHasExited;

		public bool Connect (Port outPort, Port inPort)
		{
			throw new NotImplementedException ();
		}

		public bool Disconnect (Port outPort, Port inPort)
		{
			throw new NotImplementedException ();
		}

		public ConnectionType ConnectionType
		{
			get 
			{
				return ConnectionType.AlsaMidi;
			}
		}

		public bool IsActive
		{
			get 
			{
				//throw new NotImplementedException ();
				return true;
			}
		}

		public IEnumerable<Port> Ports
		{
			get 
			{
				return LibAsoundWrapper.GetPorts();
			}
		}

		public IEnumerable<IConnection> Connections
		{
			get 
			{
				return null;
				//throw new NotImplementedException ();
			}
		}
		#endregion
}
}

