//
// Client.cs
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
using System.Diagnostics;
using System.Collections.Generic;

namespace MonoMultiJack.ConnectionWrapper
{
	public class Client : IConnectable
	{
		public string Name { get; private set; }

		public FlowDirection FlowDirection { get; private set; }

		public ConnectionType ConnectionType { get; private set; }

		readonly List<Port> _ports = new List<Port> ();

		public IEnumerable<Port> Ports {
			get {
				return _ports;
			}
		}

		public Client (string name, FlowDirection direction, ConnectionType connectionType)
		{
			Name = name;
			FlowDirection = direction;
			ConnectionType = connectionType;
		}

		public void AddPort (Port port)
		{
			Debug.Assert (FlowDirection == port.FlowDirection, "Flow directions do not match");
			Debug.Assert (ConnectionType == port.ConnectionType, "Connection types do not match");
			_ports.Add (port);
			port.Client = this;
		}

		public bool RemovePort (Port port)
		{
			return _ports.Remove (port);
		}

		public bool ReplacePort (Port oldPort, Port newPort)
		{
			bool status = _ports.Remove (oldPort);
			if (status) {
				_ports.Add (newPort);
			}
			return status;
		}

		public override bool Equals (object obj)
		{
			Client otherClient = obj as Client;
			return Equals (otherClient);
		}

		public bool Equals (Client other)
		{
			if (other == null)
				return false;
			if (GetType () != other.GetType ())
				return false;

			return Name == other.Name 
				&& FlowDirection == other.FlowDirection 
				&& ConnectionType == other.ConnectionType;
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode () 
				^ ((int)FlowDirection).GetHashCode ()
				^ ((int)ConnectionType).GetHashCode ();
		}

		public static bool operator == (Client a, Client b)
		{
			if (object.ReferenceEquals (a, b)) {
				return true;
			}

			if (((object)a == null) || ((object)b == null)) {
				return false;
			}
			return (a.Equals (b));
		}

		public static bool operator != (Client a, Client b)
		{
			return !(a == b);
		}
	}
}