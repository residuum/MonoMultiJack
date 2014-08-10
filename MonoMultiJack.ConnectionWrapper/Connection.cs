//
// Connection.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2013 Thomas Mayer
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
namespace MonoMultiJack.ConnectionWrapper
{
	public abstract class Connection : IConnection
	{
		public abstract Port OutPort{ get; set; }

		public abstract Port InPort{ get; set; }

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
			return OutPort.GetHashCode () 
				^ InPort.GetHashCode ()
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