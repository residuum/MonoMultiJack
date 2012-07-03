// 
// AlsaPort.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2012 Thomas Mayer
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
    internal class AlsaPort : Port
    {
	public SndSeqAddr AlsaAddress { get; private set; }
		
	public AlsaPort (SndSeqAddr alsaAdress, string portName, string clientName, PortType portType)
	{				
	    AlsaAddress = alsaAdress;
	    PortType = portType;
	    ConnectionType = ConnectionType.AlsaMidi;
	    ClientName = clientName;
	    Name = portName;				
	}

	public override bool Equals (object obj)
	{
	    var otherPort = obj as AlsaPort;
	    if (otherPort == null) {
		return false;
	    }
	    return Equals (otherPort);
	}

	public bool Equals (AlsaPort other)
	{
	    return AlsaAddress.Client.Equals (other.AlsaAddress.Client) 
		&& AlsaAddress.Port.Equals (other.AlsaAddress.Port)
		&& PortType.Equals (other.PortType);
	}

	public override int GetHashCode ()
	{
	    return AlsaAddress.Port * AlsaAddress.Client * (int)PortType;
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