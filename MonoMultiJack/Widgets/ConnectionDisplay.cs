// 
// ConnectionDisplay.cs
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
using System;
using System.Collections.Generic;
using Gtk;
using MonoMultiJack.ConnectionWrapper;
using Cairo;
using MonoMultiJack.Controllers.EventArguments;

namespace MonoMultiJack.Widgets
{
	/// <summary>
	/// Widget for displaying and managing connections.
	/// </summary>
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ConnectionDisplay : Bin, IConnectionWidget
	{
		TreeStore _outputStore = new TreeStore (typeof(IConnectable));
		TreeStore _inputStore = new TreeStore (typeof(IConnectable));
		List<IConnection> _connections = new List<IConnection> ();
		DateTime _lastLineUpdate = DateTime.Now;

		public override void Dispose ()
		{
			base.Dispose ();
		}

		void RenderClientName (TreeViewColumn treeColumn, CellRenderer cell, TreeModel treeModel, TreeIter iter)
		{
			IConnectable connectable = (IConnectable)treeModel.GetValue (iter, 0);
			((CellRendererText)cell).Text = connectable.Name;
		}

		public ConnectionDisplay (string connectionManagerName)
		{
			this.Build ();
			TreeViewColumn inClientColumn = new TreeViewColumn ();
			CellRendererText inClientCell = new CellRendererText ();
			inClientColumn.PackStart (inClientCell, true);
			inClientColumn.SetCellDataFunc (inClientCell, new TreeCellDataFunc (RenderClientName));
			_inputTreeview.AppendColumn (inClientColumn);
			_inputTreeview.Model = _inputStore;
			
			TreeViewColumn outClientColumn = new TreeViewColumn ();
			CellRendererText outClientCell = new CellRendererText ();
			outClientColumn.PackStart (outClientCell, true);
			outClientColumn.SetCellDataFunc (outClientCell, new TreeCellDataFunc (RenderClientName));
			_outputTreeview.AppendColumn (outClientColumn);
			_outputTreeview.Model = _outputStore;
			ConnectionManagerName = connectionManagerName;
		}

		void AddTreeStoreValues (IConnectable connectable, TreeStore store)
		{
			Client client = connectable as Client;
			if (client != null) {
				TreeIter clientIter;
				if (TryGetClientIter (store, client, true, out clientIter)) {
					foreach (Port port in client.Ports) {
						TreeIter portIter;
						if (!TryGetPortIter (store, clientIter, port, out portIter)) {							
							store.AppendValues (clientIter, port);
						}
					}
				}
			} else {				
				Port port = connectable as Port;
				if (port != null) {
					Console.WriteLine (port.Name);
					//throw new NotSupportedException("Only clients can be appended to tree store.");
				}
			}
		}

		bool TryGetClientIter (TreeStore store, Client client, bool createIfNotExists, out TreeIter clientIter)
		{
			clientIter = TreeIter.Zero;
			if (store.GetIterFirst (out clientIter)) {
				while (client != (Client) store.GetValue(clientIter, 0)) {
					if (!store.IterNext (ref clientIter)) {
						if (createIfNotExists) {
							clientIter = store.AppendValues (client);
							return true;
						} else {
							return false;
						}
					}
				}
			} else if (createIfNotExists) {
				clientIter = store.AppendValues (client);
				return true;
			}
			return true;
		}

		bool TryGetPortIter (TreeStore store, TreeIter clientIter, Port port, out TreeIter portIter)
		{
			portIter = TreeIter.Zero;
			if (store.IterHasChild (clientIter)) {
				if (store.IterChildren (out portIter, clientIter)) {
					while ((Port)store.GetValue(portIter, 0) != port) { 
						if (!store.IterNext (ref portIter)) {
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		public void AddConnectable (IConnectable connectable)
		{
			Application.Invoke (delegate {
				if (connectable.FlowDirection == FlowDirection.In) {					
					AddTreeStoreValues (connectable, _inputStore);
				} else if (connectable.FlowDirection == FlowDirection.Out) {			
					AddTreeStoreValues (connectable, _outputStore);
				}
			}
			);
		}

		void RemoveTreeStoreValues (IConnectable connectable, TreeStore store)
		{
			Client client = connectable as Client;
			if (client != null) {
				TreeIter clientIter;
				if (TryGetClientIter (store, client, false, out clientIter)) {
					store.Remove (ref clientIter);
				}
			} else {				
				Port port = connectable as Port;
				if (port != null) {
					TreeIter clientIter;
					if (TryGetClientIter (store, port.Client, false, out clientIter)) {
						TreeIter portIter;
						if (TryGetPortIter (store, clientIter, port, out portIter)) {
							store.Remove (ref portIter);
						}
						if (!store.IterHasChild (clientIter)) {
							store.Remove (ref clientIter);
						}
					}
				}
			}
		}

		public void RemoveConnectable (IConnectable connectable)
		{
			Application.Invoke (delegate {
				if (connectable.FlowDirection == FlowDirection.In) {
					RemoveTreeStoreValues (connectable, _inputStore);
				} else if (connectable.FlowDirection == FlowDirection.Out) {
					RemoveTreeStoreValues (connectable, _outputStore);
				}
			}
			);
		}

		IConnectable GetSelectedConnectable (TreeStore connectionStore, TreeIter selectedIter)
		{
			return connectionStore.GetValue (selectedIter, 0) as IConnectable; 
		}

		int GetYPositionForPort (TreeView tree, TreeStore store, Port selectedPort)
		{
			int cellHeight = 24;
			//We start in the middle of the first Treeview item
			int position = cellHeight / 2;

			ScrolledWindow treeParent = tree.Parent as ScrolledWindow;
			if (treeParent != null) {
				position -= Convert.ToInt32 (treeParent.Vadjustment.Value);
			}
			TreeIter clientIter;
			TreeIter portIter;
			if (store.GetIterFirst (out clientIter)) {
				do {
					if (store.IterHasChild (clientIter) && tree.GetRowExpanded (store.GetPath (clientIter))) {
						if (store.IterChildren (out portIter, clientIter)) {
							do {
								position += cellHeight;
							} while (((Port)store.GetValue(portIter, 0) != selectedPort || (Client)store.GetValue(clientIter, 0) != selectedPort.Client) && store.IterNext(ref portIter));
						}
					}
					//Necessary because the first Treeview item only counts as 1/2 cell height.
					if (((Client)store.GetValue (clientIter, 0)) == selectedPort.Client) {
						break;
					}
					position += cellHeight;
				} while (store.IterNext(ref clientIter));
			}
			return position;
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
		protected virtual void ConnectButton_Click (object sender, EventArgs e)
		{
			TreeIter selectedOutIter;
			TreeIter selectedInIter;
			if (_outputTreeview.Selection.GetSelected (out selectedOutIter) && _inputTreeview.Selection.GetSelected (out selectedInIter)) {
				IConnectable outlet = GetSelectedConnectable (
					_outputStore,
					selectedOutIter
				);
				IConnectable inlet = GetSelectedConnectable (
					_inputStore,
					selectedInIter
				);
				if (Connect != null) {
					Connect (this, new ConnectEventArgs{Outlet = outlet, Inlet = inlet});
				}
			}
		}

		protected virtual void DisconnectButton_Click (object sender, System.EventArgs e)
		{
			TreeIter selectedOutIter;
			TreeIter selectedInIter;
			if (_outputTreeview.Selection.GetSelected (out selectedOutIter) && _inputTreeview.Selection.GetSelected (out selectedInIter)) {
				IConnectable outlet = GetSelectedConnectable (
					_outputStore,
					selectedOutIter
				);
				IConnectable inlet = GetSelectedConnectable (
					_inputStore,
					selectedInIter
				);
				if (Disconnect != null) {
					Disconnect (this, new ConnectEventArgs{Outlet = outlet, Inlet = inlet});
				}
			}
		}

		/// <summary>
		/// Updates the connection lines
		/// </summary>
		void UpdateConnectionLines ()
		{
			DateTime now = DateTime.Now;
			if (now - _lastLineUpdate < TimeSpan.FromSeconds (0.01)) {
				return;
			}
			_lastLineUpdate = now;
			try {
				if (_connectionArea.GdkWindow == null) {
					return;
				}
				_connectionArea.GdkWindow.Clear ();
				using (Context g = Gdk.CairoHelper.Create (_connectionArea.GdkWindow)) {
					List<IConnection> connections = _connections;
					foreach (IConnection conn in connections) {
						int outY = GetYPositionForPort (_outputTreeview, _outputStore, conn.OutPort);
						int inY = GetYPositionForPort (_inputTreeview, _inputStore, conn.InPort);
						int areaWidth = _connectionArea.Allocation.Width;
						if (outY != -1 && inY != -1) {
							g.Save ();
							g.MoveTo (0, outY);
							//g.LineTo (areaWidth, inY);
							g.CurveTo (
								new PointD (areaWidth / 4, outY),
								new PointD (3 * areaWidth / 4, inY),
								new PointD (areaWidth, inY)
							);
							g.Restore ();
						}
					}
					g.Color = new Color (0, 0, 0);
					g.LineWidth = 1;
					g.Stroke ();
					g.Target.Dispose ();
				}
			} catch (Exception ex) {
				#if DEBUG
		Console.WriteLine (ex.Message);
				#endif				
			}	
		}

		protected virtual void OnTreeViewRowExpanded (object o, Gtk.RowExpandedArgs args)
		{
			UpdateConnectionLines ();
		}

		protected virtual void OnTreeViewRowCollapsed (object o, Gtk.RowCollapsedArgs args)
		{
			UpdateConnectionLines ();
		}

		protected virtual void Handle_ExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
			UpdateConnectionLines ();
		}
		#region IConnectionWidget implementation
		public event ConnectEventHandler Connect;
		public event ConnectEventHandler Disconnect;

		public void Clear ()
		{
			_inputStore.Clear ();
			_outputStore.Clear ();
			_connections.Clear ();
			Application.Invoke (delegate {
				UpdateConnectionLines ();
			}
			);
		}

		public void AddConnection (IConnection connection)
		{
			#if DEBUG
			Console.WriteLine (connection.OutPort.Id + ":" + connection.OutPort.Name + " is connected to " + connection.InPort.Id + ":" + connection.InPort.Name);
			#endif
			
			_connections.Add (connection);
			
			Application.Invoke (delegate {
				UpdateConnectionLines ();
			}
			);
		}

		public void RemoveConnection (IConnection connection)
		{
			#if DEBUG
			Console.WriteLine (connection.OutPort.Id + ":" + connection.OutPort.Name + " has been disconnected from " + connection.InPort.Id + ":" + connection.InPort.Name);
			#endif
			_connections.Remove (connection);
			
			Application.Invoke (delegate {
				UpdateConnectionLines ();
			}
			);
		}

		public string ConnectionManagerName { get; private set; }
		#endregion

	}
}