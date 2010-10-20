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
using Gtk;
using MonoMultiJack.ConnectionWrapper;

namespace MonoMultiJack
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ConnectionDisplay : Bin
	{
		private IConnectionManager _connectionManager;
		
		public ConnectionDisplay ()
		{
			this.Build ();
		}
		
		public ConnectionDisplay (IConnectionManager connectionManager) : this()
		{
			_connectionManager = connectionManager;
			_connectionManager.ConnectionHasChanged += Handle_connectionManagerConnectionHasChanged;
			
			var inClientColumn = new TreeViewColumn ();
			var inClientCell = new CellRendererText ();
			inClientColumn.PackStart (inClientCell, true);
			inClientColumn.AddAttribute (inClientCell, "text", 0);
			_inputTreeview.AppendColumn (inClientColumn);
			
			var outClientColumn = new TreeViewColumn ();
			var outClientCell = new CellRendererText ();
			outClientColumn.PackStart (outClientCell, true);
			outClientColumn.AddAttribute (outClientCell, "text", 0);
			_outputTreeview.AppendColumn(outClientColumn);
			UpdatePorts(_connectionManager.Ports);
		}

		private void Handle_connectionManagerConnectionHasChanged (object sender, ConnectionEventArgs e)
		{
			UpdatePorts (e.Ports);
			//UpdateConnections (e.Connections);
			
		}
		
		private void FillTreeView (TreeView treeview, IEnumerable<IGrouping<System.String,Port>> clients)
		{
			var store = new TreeStore (typeof(string));
			foreach (var client in clients) {
				TreeIter clientIter = store.AppendValues (client.First ().ClientName);
				foreach (var portName in client.Select (port => port.Name)) {
					store.AppendValues (clientIter, portName);
				}
			}
			treeview.Model = store;
		}

		private void UpdatePorts (IEnumerable<Port> ports)
		{
			if (ports != null)
			{
				var outClients = ports.Where (p => p.PortType == PortType.Output).GroupBy (port => port.ClientName);
				FillTreeView (_outputTreeview, outClients);				
				var inClients = ports.Where (p => p.PortType == PortType.Input).GroupBy (port => port.ClientName);
				FillTreeView(_inputTreeview, inClients);
			}
		}
	}
}

