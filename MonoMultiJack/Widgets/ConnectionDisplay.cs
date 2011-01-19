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
using Cairo;

namespace MonoMultiJack
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ConnectionDisplay : Bin
	{
		private IConnectionManager _connectionManager;
		
		private TreeStore _outputStore = new TreeStore(typeof(string));
		private TreeStore _inputStore = new TreeStore (typeof(string));
		
		private List<IConnection> _connections = new List<IConnection>();
		
		public ConnectionDisplay ()
		{
			this.Build ();
		}
		
		public ConnectionDisplay (IConnectionManager connectionManager) : this()
		{
			_connectionManager = connectionManager;
			_connectionManager.ConnectionHasChanged += Handle_connectionManagerConnectionHasChanged;
			_connectionManager.BackendHasExited += Handle_connectionManagerBackendHasExited;
			
			var inClientColumn = new TreeViewColumn ();
			var inClientCell = new CellRendererText ();
			inClientColumn.PackStart (inClientCell, true);
			inClientColumn.AddAttribute (inClientCell, "text", 0);
			_inputTreeview.AppendColumn (inClientColumn);
			_inputTreeview.Model = _inputStore;
			
			var outClientColumn = new TreeViewColumn ();
			var outClientCell = new CellRendererText ();
			outClientColumn.PackStart (outClientCell, true);
			outClientColumn.AddAttribute (outClientCell, "text", 0);
			_outputTreeview.AppendColumn (outClientColumn);
			_outputTreeview.Model = _outputStore;
			UpdatePorts (_connectionManager.Ports, ChangeType.New);
			UpdateConnections(_connectionManager.Connections, ChangeType.New);
		}

		private void Handle_connectionManagerBackendHasExited (object sender, ConnectionEventArgs e)
		{
			_outputStore.Clear ();
			_inputStore.Clear();
		}

		private void Handle_connectionManagerConnectionHasChanged (object sender, ConnectionEventArgs e)
		{
#if DEBUG
			Console.WriteLine (e.Message);
			Console.WriteLine (e.ConnectionType.ToString ());
#endif
			if (e.Ports != null && e.Ports.Any ())
			{
				UpdatePorts (e.Ports, e.ChangeType);
			}
			if (e.Connections != null && e.Connections.Any ())
			{
				UpdateConnections(e.Connections, e.ChangeType);
			}
			
		}
			
		private void RemoveTreeStoreValues (TreeStore store, IEnumerable<IGrouping<System.String, Port>> clients)
		{
			foreach (var client in clients)
			{
				TreeIter clientIter;
				string clientName = client.First ().ClientName;
				if (store.GetIterFirst (out clientIter))
				{
					while (store.GetValue (clientIter, 0).ToString () != clientName)
					{
						if (!store.IterNext (ref clientIter))
						{
							break;
						}
					}
					if (store.IterHasChild (clientIter))
					{
						foreach (var portName in client.Select (port => port.Name))
						{
							TreeIter portIter;
							if (store.IterChildren (out portIter, clientIter))
							{
								while (store.GetValue (portIter, 0).ToString () != portName)
								{
									if (!store.IterNext (ref portIter))
									{
										break;
									}
								}
								if (store.GetValue (portIter, 0).ToString () == portName)
								{
									store.Remove (ref portIter);
								}
							}
						}
					}
					if (!store.IterHasChild (clientIter))
					{
						store.Remove (ref clientIter);
					}
				}
			}
		}
		
		private void AddTreeStoreValues (TreeStore store, IEnumerable<IGrouping<System.String, Port>> clients)
		{
			foreach (var client in clients)
			{
				TreeIter clientIter;
				string clientName = client.First ().ClientName;
				if (store.GetIterFirst (out clientIter))
				{
					while (store.GetValue (clientIter, 0).ToString() != clientName)
					{
						if (!store.IterNext (ref clientIter))
						{
							clientIter = store.AppendValues (clientName);
							break;
						}
					}
				}
				else
				{
					clientIter = store.AppendValues(clientName);
				}
				foreach (var portName in client.Select (port => port.Name))
				{
					store.AppendValues (clientIter, portName);
				}
			}
		}
		
		private void UpdatePorts (IEnumerable<Port> updatedPorts, ChangeType changeType)
		{
			if (updatedPorts != null && updatedPorts.Any ())
			{
				switch (changeType)
				{
					case ChangeType.New:
						var newOutputClients = updatedPorts.Where (p => p.PortType == PortType.Output).GroupBy (port => port.ClientName);
						AddTreeStoreValues (_outputStore, newOutputClients);
						var newInputClients = updatedPorts.Where (p => p.PortType == PortType.Input).GroupBy (port => port.ClientName);
						AddTreeStoreValues (_inputStore, newInputClients);
						break;
			
					case ChangeType.Deleted:
						var oldOutputClients = updatedPorts.Where (p => p.PortType == PortType.Output).GroupBy (port => port.ClientName);
						RemoveTreeStoreValues (_outputStore, oldOutputClients);
						var oldInputClients = updatedPorts.Where (p => p.PortType == PortType.Input).GroupBy (port => port.ClientName);
						RemoveTreeStoreValues (_inputStore, oldInputClients);
						break;
				}
				UpdateConnectionLines ();
			}
		}
		
		private Port GetSelectedPort (TreeStore connectionStore, TreeIter selectedIter, PortType portType)
		{
			TreePath iterPath = connectionStore.GetPath (selectedIter);
			Port selectedPort = null;
			TreeIter outParentIter;
			if (iterPath.Up () && connectionStore.GetIter (out outParentIter, iterPath))
			{
				selectedPort = new Port (connectionStore.GetValue (selectedIter, 0).ToString (),
					connectionStore.GetValue (outParentIter, 0).ToString (),
					portType, _connectionManager.ConnectionType);
			}
			return selectedPort;
		}

		private void UpdateConnections (IEnumerable<IConnection> updatedConnections, ChangeType changeType)
		{
			if (updatedConnections != null && updatedConnections.Any ())
			{
				switch (changeType)
				{
					case ChangeType.New:
						
#if DEBUG
						foreach (IConnection conn in updatedConnections)
						{
							Console.WriteLine (conn.InPort.ClientName + ":" + conn.InPort.Name + " is connected to " + conn.OutPort.ClientName + ":" + conn.OutPort.Name);
						}
#endif
						_connections.AddRange(updatedConnections);
						break;
					case ChangeType.Deleted:
#if DEBUG
						foreach (IConnection conn in updatedConnections)
						{
							Console.WriteLine (conn.InPort.ClientName + ":" + conn.InPort.Name + " has been disconnected from " + conn.OutPort.ClientName + ":" + conn.OutPort.Name);
						}
#endif
						var oldConnectionHashes = new HashSet<IConnection>(updatedConnections);
						_connections.RemoveAll(c => oldConnectionHashes.Contains(c));					
						break;
				}
				UpdateConnectionLines ();
			}
		}
		protected virtual void ConnectButton_Click (object sender, System.EventArgs e)
		{
			TreeIter selectedOutIter;
			TreeIter selectedInIter;
			if (_outputTreeview.Selection.GetSelected (out selectedOutIter) && _inputTreeview.Selection.GetSelected (out selectedInIter))
			{
				Port outPort = GetSelectedPort (_outputStore, selectedOutIter, PortType.Output);
				Port inPort = GetSelectedPort(_inputStore, selectedInIter, PortType.Input);
				_connectionManager.Connect (outPort, inPort);
			}
			UpdateConnectionLines ();
		}
		
		protected virtual void DisconnectButton_Click (object sender, System.EventArgs e)
		{
			TreeIter selectedOutIter;
			TreeIter selectedInIter;
			if (_outputTreeview.Selection.GetSelected (out selectedOutIter) && _inputTreeview.Selection.GetSelected (out selectedInIter))
			{
				Port outPort = GetSelectedPort (_outputStore, selectedOutIter, PortType.Output);
				Port inPort = GetSelectedPort (_inputStore, selectedInIter, PortType.Input);
				_connectionManager.Disconnect (outPort, inPort);
			}
			UpdateConnectionLines ();
		}	
		
		protected virtual void OnTreeViewRowExpanded (object o, Gtk.RowExpandedArgs args)
		{
			UpdateConnectionLines();
		}
		
		void UpdateConnectionLines ()
		{
			_connectionArea.GdkWindow.Clear ();
			using (Context g = Gdk.CairoHelper.Create (_connectionArea.GdkWindow))
			{
				foreach (IConnection conn in _connections)
				{
					g.Save ();
					g.MoveTo (0, 0);
					g.LineTo(_connectionArea.WidthRequest, 0);
					g.Restore ();
				}
				g.Color = new Color (0, 0, 0);
				g.LineWidth = 2;
				g.Stroke();
			}

		}

		protected virtual void OnTreeViewRowCollapsed (object o, Gtk.RowCollapsedArgs args)
		{
			UpdateConnectionLines();
		}
		
		
		
	}
}