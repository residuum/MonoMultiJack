//
// Connection.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2013-2014 Thomas Mayer
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
namespace Mmj.ConnectionWrapper
{
	public abstract class Connection : IConnection
	{
		Port _outPort;
		Port _inPort;

		public Port OutPort {
			get {
				if (_inPort != null) {
					return _outPort;
				} 
				return null;
			}
			set {
				if (value.ConnectionType == ConnectionType && value.FlowDirection == FlowDirection.Out) {
					_outPort = value;					
				}
			}
		}

		public Port InPort {
			get {
				if (_outPort != null) {
					return _inPort;
				}
				return null;
			}
			set {
				if (value.ConnectionType == ConnectionType && value.FlowDirection == FlowDirection.In) {
					_inPort = value;
				}
			}
		}

		public abstract ConnectionType ConnectionType{ get; }

		public override bool Equals (object obj)
		{
			Connection otherPort = obj as Connection;
			return Equals (otherPort);
		}

		public bool Equals (Connection other)
		{
			if (other == null)
				return false;
			if (GetType () != other.GetType ())
				return false;

			return OutPort == other.OutPort
				&& InPort == other.InPort
				&& ConnectionType == other.ConnectionType;
		}

		public override int GetHashCode ()
		{
			return (OutPort.GetHashCode () << 4)
				^ (InPort.GetHashCode () << 2)
				^ ((int)ConnectionType).GetHashCode ();
		}

		public static bool operator == (Connection a, Connection b)
		{
			if (object.ReferenceEquals (a, b)) {
				return true;
			}

			if (((object)a == null) || ((object)b == null)) {
				return false;
			}
			return (a.Equals (b));
		}

		public static bool operator != (Connection a, Connection b)
		{
			return !(a == b);
		}
	}
}
