// 
// Port.cs
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
using System.Collections.Generic;

namespace MonoMultiJack.ConnectionWrapper
{
	public abstract class Port : IConnectable
	{
		public string Name { get; protected set; }

		public uint Id { get; protected set; }

		public FlowDirection FlowDirection { get; protected set; }

		public ConnectionType ConnectionType { get; protected set; }

        public Client Client { get; internal set; }

		public IEnumerable<Port> Ports {
			get {
				yield return this;
			}
		}

		public override bool Equals (object obj)
		{
			Port otherPort = obj as Port;
			return Equals (otherPort);
		}

		public bool Equals (Port other)
		{
			if (other == null)
				return false;
			if (GetType () != other.GetType ())
				return false;
			
			return Id == other.Id 
				&& FlowDirection == other.FlowDirection 
				&& ConnectionType == other.ConnectionType;
		}

		public override int GetHashCode ()
		{
			return Id.GetHashCode ()
				^ ((int)FlowDirection).GetHashCode () 
				^ ((int)ConnectionType).GetHashCode ();
		}

		public static bool operator == (Port a, Port b)
		{
			if (object.ReferenceEquals (a, b)) {
				return true;
			}

			if (((object)a == null) || ((object)b == null)) {
				return false;
			}
			return (a.Equals (b));
		}

		public static bool operator != (Port a, Port b)
		{
			return !(a == b);
		}
	}
}