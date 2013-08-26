// 
// AlsaPort.cs
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
namespace MonoMultiJack.ConnectionWrapper.Alsa.Types
{
	internal class AlsaPort : Port
	{
		public SndSeqAddr AlsaAddress { get; private set; }

		public string ClientName { get; private set; }
		
		public AlsaPort (SndSeqAddr alsaAdress, string portName, string clientName, FlowDirection flowDirection, uint portId)
		{				
			AlsaAddress = alsaAdress;
			FlowDirection = flowDirection;
			ConnectionType = ConnectionType.AlsaMidi;
			Name = portName;
			ClientName = clientName;
			Id = portId;
		}
				
		public override bool Equals (object obj)
		{
			AlsaPort otherPort = obj as AlsaPort;
			return Equals (otherPort);
		}

		public bool Equals (AlsaPort other)
		{
			if (other == null)
				return false;
			if (GetType () != other.GetType ())
				return false;
			
			return Id == other.Id 
				&& FlowDirection == other.FlowDirection 
				&& ConnectionType == other.ConnectionType
				&& AlsaAddress.Client == other.AlsaAddress.Client
				&& AlsaAddress.Port == other.AlsaAddress.Port;
		}

		public override int GetHashCode ()
		{
			return Id.GetHashCode ()
				^ ((int)FlowDirection).GetHashCode () 
				^ ((int)ConnectionType).GetHashCode ()
				^ AlsaAddress.Client.GetHashCode ()
				^ AlsaAddress.Port.GetHashCode ();
		}

		public static bool operator == (AlsaPort a, AlsaPort b)
		{
			if (object.ReferenceEquals (a, b)) {
				return true;
			}

			if (((object)a == null) || ((object)b == null)) {
				return false;
			}
			return (a.Equals (b));
		}

		public static bool operator != (AlsaPort a, AlsaPort b)
		{
			return !(a == b);
		}
	}	
}