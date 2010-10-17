// 
// ConnectionDisplay.cs
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
using System.Linq;
using MonoMultiJack.ConnectionWrapper;

namespace MonoMultiJack
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ConnectionDisplay : Gtk.Bin
	{
		private IConnectionManager _connectionManager;
		
		public ConnectionDisplay ()
		{
			this.Build ();
		}
		
		public ConnectionDisplay (IConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
			_connectionManager.ConnectionHasChanged += Handle_connectionManagerConnectionHasChanged;
			UpdatePorts(_connectionManager.Ports);
		}

		private void Handle_connectionManagerConnectionHasChanged (object sender, ConnectionEventArgs e)
		{
			UpdatePorts (e.Ports);
		}
		
		private void UpdatePorts (IEnumerable<IPort> ports)
		{
			if (ports != null)
			{
				var outPorts = ports.Where (p => p.PortType == PortType.Output);
				var inPorts = ports.Where (p => p.PortType == PortType.Input);
			}
		}
	}
}

