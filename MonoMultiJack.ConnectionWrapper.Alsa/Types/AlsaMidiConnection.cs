// 
// AlsaMidiConnection.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2009-2012 Thomas Mayer
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
namespace MonoMultiJack.ConnectionWrapper.Alsa.Types
{
	public class AlsaMidiConnection : IConnection
	{
		private Port _outPort;
		private Port _inPort;
		
	#region IConnection implementation
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
		
		public ConnectionType ConnectionType {
			get { return ConnectionType.AlsaMidi; }
		}
	#endregion

		public override bool Equals (object obj)
		{
			var otherConn = obj as AlsaMidiConnection;
			if (otherConn == null) {
				return false;
			}
			return Equals (otherConn);
		}

		public bool Equals (AlsaMidiConnection other)
		{
			return OutPort.Equals (other.OutPort)
				&& InPort.Equals (other.InPort);
		}

		public override int GetHashCode ()
		{
			return InPort.GetHashCode () * OutPort.GetHashCode ();
		}

		public static bool operator == (AlsaMidiConnection a, AlsaMidiConnection b)
		{
			if (object.ReferenceEquals (a, b)) {
				return true;
			}

			if (((object)a == null) || ((object)b == null)) {
				return false;
			}
			return (a.Equals (b));
		}

		public static bool operator != (AlsaMidiConnection a, AlsaMidiConnection b)
		{
			return !(a == b);
		}
	}
}