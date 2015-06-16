//
// ConnectableTreeView.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2009-2015 Thomas Mayer
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
using Mmj.ConnectionWrapper;
using Xwt;
using Xwt.Drawing;
using Mmj.Controllers.EventArguments;

namespace Mmj.Views.Widgets
{
	public partial class ConnectionDisplay
	{
		class ConnectableTreeView : Widget
		{
			TreeView _treeView;
			TreeStore _treeStore;
			IDataField<IConnectable> _dataField;
			IDataField<string> _textField;
			bool _eventsBound;

			public ConnectableTreeView ()
			{
				BuildWidget ();
			}

			void BindEvents ()
			{
				_treeView.RowExpanded += UpdateParent;
				_treeView.RowCollapsed += UpdateParent;
				_treeView.VerticalScrollControl.ValueChanged += UpdateParent;
				_treeView.DragDrop += HandleDropped;
				_treeView.DragOver += HandleDragOver;
				_treeView.DragStarted += HandleDragStarted;
				_treeView.SetDragSource (TransferDataType.Text);
				_treeView.SetDragDropTarget (TransferDataType.Text);
				_eventsBound = true;
			}

			void BuildWidget ()
			{
				_dataField = new DataField<IConnectable> ();
				_textField = new DataField<string> ();
				_treeStore = new TreeStore (new IDataField[] {
					_dataField,
					_textField
				});
				_treeView = new TreeView (_treeStore);
				_treeView.Columns.Add ("", _textField);
				_treeView.MinHeight = 200;
				_treeView.MinWidth = 200;
				_treeView.ExpandVertical = true;
				_treeView.ExpandHorizontal = false;
				_treeView.HeadersVisible = false;
				Content = _treeView;
			}

			void HandleDragOver (object sender, DragOverEventArgs e)
			{
				ConnectableSerialization id = new ConnectableSerialization ((string)e.Data.GetValue (TransferDataType.Text));
				TreeNavigator firstItem = _treeStore.GetFirstNode ();
				if (firstItem == null) {
					e.AllowedAction = DragDropAction.None;
					return;
				}
				IConnectable firstClient = firstItem.GetValue (_dataField);
				if (firstClient.ConnectionType != id.ConnectionType || firstClient.FlowDirection == id.FlowDirection) {					
					e.AllowedAction = DragDropAction.None;
					return;
				}
				if (e.Action == DragDropAction.All) {
					e.AllowedAction = DragDropAction.Move;
				} else {
					e.AllowedAction = e.Action;
				}
			}

			void HandleDragStarted (object sender, DragStartedEventArgs e)
			{
				Image icon = Icons.Connect;
				e.DragOperation.SetDragImage (icon, (int)icon.Width, (int)icon.Height);
				e.DragOperation.AllowedActions = DragDropAction.All;
				e.DragOperation.Data.AddValue (GetSelected ().Serialization.ToString ());
			}

			public event EventHandler ViewChanged;
			public event ConnectEventHandler Connect;

			void NotifyParent ()
			{
				if (ViewChanged != null) {
					ViewChanged (this, new EventArgs ());
				}
			}

			void HandleDropped (object sender, DragEventArgs e)
			{
				ConnectableSerialization id = new ConnectableSerialization ((string)e.Data.GetValue (TransferDataType.Text));
				RowDropPosition pos;
				TreePosition nodePosition;
				_treeView.GetDropTargetRow (e.Position.X, e.Position.Y, out pos, out nodePosition);
				IConnectable droppedOn = GetConnectable (nodePosition);
				IConnectable connectable = id.GetConnectable ();

				if (Connect != null && droppedOn != null) {
					Connect (this, new ConnectEventArgs {
						Outlet = droppedOn,
						Inlet = connectable
					});
				}
			}

			void UpdateParent (object sender, EventArgs e)
			{
				NotifyParent ();
			}

			public void AddConnectable (IConnectable connectable)
			{
				if (!_eventsBound) {
					BindEvents ();
				}
				Application.Invoke (() => {
					Client client = connectable as Client;
					Port port = connectable as Port;
					if (client != null) {
						AddClient (client);
						foreach (Port clientPort in client.Ports) {
							AddPort (clientPort);
						}
					}
					if (port != null) {
						TreeNavigator navigator = FindNavigator (port.Client);
						if (navigator != null) {
							AddPort (port);
						}
					}
					NotifyParent ();
				});
			}

			void AddClient (Client client)
			{
				TreeNavigator navigator = FindNavigator (client);
				if (navigator != null) {
					return;
				}
				_treeStore.AddNode ().SetValues (0, _dataField, client, _textField, client.Name);
			}

			void AddPort (Port port)
			{
				TreeNavigator navigator = FindNavigator (port);
				if (navigator != null) {
					return;
				}
				navigator = FindNavigator (port.Client);
				navigator.AddChild ().SetValues (0, _dataField, port, _textField, port.Name);
				navigator.MoveToParent ();
				_treeView.ExpandRow (navigator.CurrentPosition, false);
			}

			public void RemoveConnectable (IConnectable connectable)
			{
				Application.Invoke (() =>
				{
					Client client = connectable as Client;
					Port port = connectable as Port;
					if (client != null) {
						RemoveClient (client);
					}
					if (port != null) {
						RemovePort (port);
					}
					NotifyParent ();
				});
			}

			void RemoveClient (Client client)
			{
				TreeNavigator navigator = FindNavigator (client);
				navigator.RemoveChildren ();
				navigator.Remove ();
			}

			void RemovePort (Port port)
			{
				TreeNavigator clientNavigator = FindNavigator (port.Client);
				TreeNavigator navigator = FindNavigator (port);
				navigator.Remove ();
				if (!clientNavigator.MoveToChild ()) {
					clientNavigator.Remove ();
				}
			}

			TreeNavigator FindNavigator (IConnectable connectable)
			{
				try {
					return _treeStore.FindNavigators (connectable, _dataField).FirstOrDefault ();
				} catch (NullReferenceException) {
					return null;
				}
			}

			public void UpdateConnectable (IConnectable connectable)
			{
				Application.Invoke (() =>
				{
					Client client = connectable as Client;
					Port port = connectable as Port;
					if (client != null) {
						TreeNavigator navigator = FindNavigator (client);
						UpdateTreeStoreValues (navigator, connectable);
					}
					if (port != null) {
						TreeNavigator navigator = FindNavigator (port);
						UpdateTreeStoreValues (navigator, connectable);
					}
				});
			}

			void UpdateTreeStoreValues (TreeNavigator navigator, IConnectable connectable)
			{
				if (navigator != null) {
					navigator.SetValues (0, _dataField, connectable, _textField, connectable.Name);
				}
			}

			IConnectable GetConnectable (TreePosition position)
			{
				TreeNavigator navigator = _treeStore.GetNavigatorAt (position);
				return navigator.GetValue (_dataField);
			}

			public IConnectable GetSelected ()
			{
				TreePosition position = _treeView.SelectedRow;
				if (position == null) {
					return null;
				}
				return GetConnectable (position);
			}

			public IConnectable GetAll ()
			{
				TreeNavigator navigator = _treeStore.GetFirstNode ();
				Client firstClient = (Client)navigator.GetValue (_dataField);
				Client dummy = new Client ("", firstClient.FlowDirection, firstClient.ConnectionType);
				do {
					navigator.MoveToChild ();
					do {
						dummy.AddPort ((Port)navigator.GetValue (_dataField));
					} while (navigator.MoveNext());
					navigator.MoveToParent ();
				} while(navigator.MoveNext ());
				return dummy;
			}

			public double GetYPositionOfConnectable (IConnectable connectable)
			{
				TreeNavigator navigator = FindNavigator (connectable);
				TreePosition position = navigator.CurrentPosition;
				return _treeView.GetRowBounds (position, true).Center.Y;
			}

			public void Clear ()
			{
				Application.Invoke (_treeStore.Clear);
				NotifyParent ();
			}
		}
	}
}
