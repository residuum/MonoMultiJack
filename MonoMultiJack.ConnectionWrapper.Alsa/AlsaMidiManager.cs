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
using System.Linq;
using System.Collections.Generic;
using GLib;

namespace MonoMultiJack.ConnectionWrapper.Alsa
{
    public class AlsaMidiManager : IConnectionManager
    {
	private List<AlsaPort> _portMapper = new List<AlsaPort> ();
	private List<IConnection> _connections = new List<IConnection> ();
	
	public AlsaMidiManager ()
	{
	    LibAsoundWrapper.Activate ();
			
	    GLib.Timeout.Add (2000, new GLib.TimeoutHandler (CheckForChanges));
	}
		
	~AlsaMidiManager ()
	{
	    LibAsoundWrapper.DeActivate ();
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

	public ConnectionType ConnectionType {
	    get {
		return ConnectionType.AlsaMidi;
	    }
	}

	public bool IsActive {
	    get {
		//throw new NotImplementedException ();
		return true;
	    }
	}

	public IEnumerable<Port> Ports {
	    get {
		_portMapper = LibAsoundWrapper.GetPorts ().ToList ();
		return _portMapper.Cast<Port> ();
	    }
	}

	public IEnumerable<IConnection> Connections {
	    get {
		return LibAsoundWrapper.GetConnections ();
	    }
	}
		#endregion

	bool CheckForChanges ()
	{
	    IEnumerable<AlsaPort> allPorts = LibAsoundWrapper.GetPorts ();
	    var mappedPorts = new List<AlsaPort> ();
	    var newPorts = new List<Port> ();
	    var obsoletePorts = new List<Port> ();

	    foreach (AlsaPort port in allPorts) {
		AlsaPort foundPort = _portMapper.FirstOrDefault (p => p.Equals (port));
				
		mappedPorts.Add (port);
		if (foundPort == null) {
		    newPorts.Add (port);
		    _portMapper.Add (port);
		}
	    }
	    
	    foreach (AlsaPort oldPort in _portMapper) {
		AlsaPort mappedPort = mappedPorts.FirstOrDefault (p => p.Equals(oldPort));
		if (mappedPort == null) {
		    obsoletePorts.Add (oldPort);
		}
	    }
	    // Remove obsolete ports
	    foreach (AlsaPort obsoletePort in obsoletePorts) {
		_portMapper.Remove (obsoletePort);
	    }

	    //TODO: Connections
	    if (newPorts.Any ()) {
		var newEventArgs = new ConnectionEventArgs ();
		newEventArgs.ChangeType = ChangeType.New;
		newEventArgs.Ports = newPorts;
		ConnectionHasChanged (this, newEventArgs);
	    }
	    if (obsoletePorts.Any ()) {
		var oldEventArgs = new ConnectionEventArgs ();
		oldEventArgs.ChangeType = ChangeType.Deleted;
		oldEventArgs.Ports = obsoletePorts;
		ConnectionHasChanged (this, oldEventArgs);
	    }

	    return true;
	}
    }
}