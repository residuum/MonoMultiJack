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
	/// <summary>
	/// Widget for displaying and managing connections.
	/// </summary>
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ConnectionDisplay : Bin
	{
		/// <summary>
		/// Connection manager for this instance.
		/// </summary>
		private IConnectionManager _connectionManager;
		
		/// <summary>
		/// Treestore for outputs.
		/// </summary>
		private TreeStore _outputStore = new TreeStore(typeof(string));
		
		/// <summary>
		/// Treestore for inputs.
		/// </summary>
		private TreeStore _inputStore = new TreeStore (typeof(string));
		
		/// <summary>
		/// All active connections.
		/// </summary>
		private List<IConnection> _connections = new List<IConnection>();
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ConnectionDisplay ()
		{
			this.Build ();
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connectionManager">
		/// A <see cref="IConnectionManager"/> whose ports and connection are displayed.
		/// </param>
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
			UpdateConnections (_connectionManager.Connections, ChangeType.New);
		}
		
		/// <summary>
		/// Handles the BackendHasExited event of the connection manager.
		/// </summary>
		/// <param name="sender">
		/// The source of the event.
		/// </param>
		/// <param name="e">
		/// The <see cref="ConnectionEventArgs"/> of the event.
		/// </param>
		private void Handle_connectionManagerBackendHasExited (object sender, ConnectionEventArgs e)
		{
			_outputStore.Clear ();
			_inputStore.Clear ();
			_connections.Clear ();
			UpdateConnectionLines ();
		}

		/// <summary>
		/// Handles the ConnectionHasChanged event of the connection manager.
		/// </summary>
		/// <param name="sender">
		/// The source of the event.
		/// </param>
		/// <param name="e">
		/// The <see cref="ConnectionEventArgs"/> of the event.
		/// </param>
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
			
		/// <summary>
		/// Removes values from a treestore.
		/// </summary>
		/// <param name="store">
		/// A <see cref="TreeStore"/> to remove values from.
		/// </param>
		/// <param name="clients">
		/// A <see cref="IEnumerable<IGrouping<System.String, Port>>"/> determining the values to remove. 
		/// </param>
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
		
		/// <summary>
		/// Adds values to a treestore
		/// </summary>
		/// <param name="store">
		/// The <see cref="TreeStore"/> to add values to.
		/// </param>
		/// <param name="clients">
		/// A <see cref="IEnumerable<IGrouping<System.String, Port>>"/> determining the values to add to the treestore.
		/// </param>
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
		
		/// <summary>
		/// Updates ports.
		/// </summary>
		/// <param name="updatedPorts">
		/// A <see cref="IEnumerable<Port>"/> determining the ports to update.
		/// </param>
		/// <param name="changeType">
		/// A <see cref="ChangeType"/>
		/// </param>
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
		
		/// <summary>
		/// Gets the selected from from a treestore by the selected treeiter.
		/// </summary>
		/// <param name="connectionStore">
		/// A <see cref="TreeStore"/>
		/// </param>
		/// <param name="selectedIter">
		/// A <see cref="TreeIter"/>
		/// </param>
		/// <param name="portType">
		/// A <see cref="PortType"/>
		/// </param>
		/// <returns>
		/// A <see cref="Port"/>
		/// </returns>
		private List<Port> GetSelectedPorts (TreeStore connectionStore, TreeIter selectedIter, PortType portType)
		{
			TreePath iterPath = connectionStore.GetPath (selectedIter);
			List<Port> selectedPorts = new List<Port> ();
			TreeIter otherIter;
			if (iterPath.Depth == 2)
			{
				if (iterPath.Up () && connectionStore.GetIter (out otherIter, iterPath))
				{
					selectedPorts.Add (new Port (connectionStore.GetValue (selectedIter, 0).ToString (),
						connectionStore.GetValue (otherIter, 0).ToString (),
						portType, _connectionManager.ConnectionType));
				}
			}
			else if (iterPath.Depth == 1)			
			{
				iterPath.Down ();
				while (connectionStore.GetIter (out otherIter, iterPath))
				{
					selectedPorts.Add (new Port (connectionStore.GetValue (otherIter, 0).ToString (),
						connectionStore.GetValue (selectedIter, 0).ToString (),
						portType, _connectionManager.ConnectionType));
					iterPath.Next();
				}
			}
			return selectedPorts;
		}
		
		/// <summary>
		/// Gets the vertical position for the selected post in the treeview.
		/// </summary>
		/// <param name="tree">
		/// A <see cref="TreeView"/>
		/// </param>
		/// <param name="store">
		/// A <see cref="TreeStore"/>
		/// </param>
		/// <param name="selectedPort">
		/// A <see cref="Port"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		private int GetYPositionForPort (TreeView tree, TreeStore store, Port selectedPort)
		{
			int cellHeight = 24;
			//We start in the middle of the first Treeview item
			int position = cellHeight / 2;
			
			ScrolledWindow treeParent = tree.Parent as ScrolledWindow;
			if (treeParent != null)
			{
				position -= Convert.ToInt32(treeParent.Vadjustment.Value);
			}
			TreeIter clientIter;
			TreeIter portIter;
			if (store.GetIterFirst (out clientIter))
			{
				do
				{
					if (store.IterHasChild (clientIter) && tree.GetRowExpanded (store.GetPath (clientIter)))
					{
						if (store.IterChildren (out portIter, clientIter))
						{
							do
							{
								position += cellHeight;
							}
							while ((store.GetValue (portIter, 0).ToString () != selectedPort.Name || store.GetValue (clientIter, 0).ToString () != selectedPort.ClientName)
								&& store.IterNext(ref portIter));
						}
					}
					//Necessary because the first Treeview item only counts as 1/2 cell height.
					if (store.GetValue (clientIter, 0).ToString () == selectedPort.ClientName)
					{
						break;
					}
					position += cellHeight;
				} 
				while (store.IterNext (ref clientIter) );
			}
			return position;
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
							Console.WriteLine (conn.OutPort.ClientName + ":" + conn.OutPort.Name + " is connected to " + conn.InPort.ClientName + ":" + conn.InPort.Name);
						}
#endif
						_connections.AddRange(updatedConnections);
						break;
					case ChangeType.Deleted:
#if DEBUG
						foreach (IConnection conn in updatedConnections)
						{
							Console.WriteLine (conn.OutPort.ClientName + ":" + conn.OutPort.Name + " has been disconnected from " + conn.InPort.ClientName + ":" + conn.InPort.Name);
						}
#endif
						var oldConnectionHashes = new HashSet<IConnection>(updatedConnections);
						_connections.RemoveAll(c => oldConnectionHashes.Contains(c));					
						break;
				}
				UpdateConnectionLines ();
			}
		}
		
		/// <summary>
		/// Handles the click event on the ConnectButton
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="e">
		/// A <see cref="System.EventArgs"/>
		/// </param>
		protected virtual void ConnectButton_Click (object sender, System.EventArgs e)
		{
			TreeIter selectedOutIter;
			TreeIter selectedInIter;
			if (_outputTreeview.Selection.GetSelected (out selectedOutIter) && _inputTreeview.Selection.GetSelected (out selectedInIter))
			{
				List<Port> outPorts = GetSelectedPorts (_outputStore, selectedOutIter, PortType.Output);
				List<Port> inPorts = GetSelectedPorts (_inputStore, selectedInIter, PortType.Input);
				int minCount = Math.Min (inPorts.Count (), outPorts.Count ());
				for (var i = 0; i<minCount; i++)
				{
					_connectionManager.Connect (outPorts[i], inPorts[i]);
				}
			}
		}
		
		protected virtual void DisconnectButton_Click (object sender, System.EventArgs e)
		{
			TreeIter selectedOutIter;
			TreeIter selectedInIter;
			if (_outputTreeview.Selection.GetSelected (out selectedOutIter) && _inputTreeview.Selection.GetSelected (out selectedInIter))
			{
				IEnumerable<Port> outPorts = GetSelectedPorts (_outputStore, selectedOutIter, PortType.Output);
				IEnumerable<Port> inPorts = GetSelectedPorts (_inputStore, selectedInIter, PortType.Input);
				foreach (Port outPort in outPorts)
				{
					foreach (Port inPort in inPorts)					
					{
						_connectionManager.Disconnect (outPort, inPort);
					}
				}
			}
		}	
		
		private void UpdateConnectionLines ()
		{
			_connectionArea.GdkWindow.Clear ();
			using (Context g = Gdk.CairoHelper.Create (_connectionArea.GdkWindow))
			{
				foreach (IConnection conn in _connections)
				{
					int outY = GetYPositionForPort (_outputTreeview, _outputStore, conn.OutPort);
					int inY = GetYPositionForPort (_inputTreeview, _inputStore, conn.InPort);
					if (outY != -1 && inY != -1)
					{
						g.Save ();
						g.MoveTo (0, outY);
						g.LineTo (_connectionArea.Allocation.Width, inY);
						g.Restore ();
					}
				}
				g.Color = new Color (0, 0, 0);
				g.LineWidth = 1;
				g.Stroke();
			}

		}

		protected virtual void OnTreeViewRowExpanded (object o, Gtk.RowExpandedArgs args)
		{
			UpdateConnectionLines ();
		}

		protected virtual void OnTreeViewRowCollapsed (object o, Gtk.RowCollapsedArgs args)
		{
			UpdateConnectionLines();
		}

		protected virtual void Handle_ExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
			UpdateConnectionLines();
		}		
	}
}