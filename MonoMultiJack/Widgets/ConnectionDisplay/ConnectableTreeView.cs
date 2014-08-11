//
// ConnectableTreeView.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2009-2014 Thomas Mayer
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
using MonoMultiJack.ConnectionWrapper;
using MonoMultiJack.Controllers.EventArguments;
using Xwt;
using Xwt.Drawing;

namespace MonoMultiJack.Widgets
{
	public partial class ConnectionDisplay
	{
		class ConnectableTreeView : Widget
		{
			TreeView _treeView;
			TreeStore _treeStore;
			IDataField<IConnectable> _dataField;
			IDataField<string> _textField;

			public ConnectableTreeView ()
			{
				BuildWidget ();
				BindEvents ();
			}

			private void BindEvents ()
			{
				_treeView.MouseScrolled += UpdateParent;
				_treeView.RowExpanded += UpdateParent;
				_treeView.RowCollapsed += UpdateParent;
				_treeView.DragDrop += HandleDropped;
				_treeView.DragOver += HandleDragOver;
				_treeView.DragStarted += HandleDragStarted;
				_treeView.SetDragSource (TransferDataType.Text);
				_treeView.SetDragDropTarget (TransferDataType.Text);
			}

			private void BuildWidget ()
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
				Application.Invoke (() => {
					Client client = connectable as Client;
					Port port = connectable as Port;
					if (client != null) {
						TreeNavigator navigator = _treeStore.GetFirstNode ();
						navigator = AddClient (navigator, client);
						foreach (Port clientPort in client.Ports) {
							navigator = AddPort (navigator, clientPort);
						}
					}
					if (port != null) {
						TreeNavigator navigator = FindClientNavigator (port.Client);
						if (navigator != null) {
							AddPort (navigator, port);
						}
					}
					NotifyParent ();
				});
			}

			TreeNavigator AddClient (TreeNavigator navigator, Client client)
			{
				bool alreadyAdded = false;
				do {
					if (navigator.CurrentPosition != null) {
						if (client.Equals (navigator.GetValue (_dataField))) {
							alreadyAdded = true;
							break;
						}
					}
				} while (navigator.CurrentPosition != null && navigator.MoveNext());
				if (!alreadyAdded) {
					navigator = _treeStore.AddNode ().SetValue (_dataField, client).SetValue (_textField, client.Name);
				}
				return navigator;
			}

			TreeNavigator AddPort (TreeNavigator navigator, Port port)
			{
				navigator.MoveToChild ();
				bool alreadyAdded = false;
				do {
					if (navigator.CurrentPosition != null) {
						if (port.Equals (navigator.GetValue (_dataField))) {
							alreadyAdded = true;
							break;
						}
					}
				} while (navigator.CurrentPosition != null && navigator.MoveNext());
				navigator.MoveToParent ();
				if (!alreadyAdded) {
					navigator.AddChild ().SetValue (_dataField, port).SetValue (_textField, port.Name);
				}
				navigator.MoveToParent ();
				return navigator;
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
				TreeNavigator navigator = FindClientNavigator (client);
				navigator.RemoveChildren ();
				navigator.Remove ();
			}

			void RemovePort (Port port)
			{
				TreeNavigator navigator = FindClientNavigator (port.Client);
				navigator.MoveToChild ();
				do {
					if (port.Equals (navigator.GetValue (_dataField))) {
						navigator.Remove ();
						break;
					}
				} while (navigator.MoveNext());
				navigator.MoveToParent ();
				if (!navigator.MoveToChild ()) {
					navigator.Remove ();
				}
			}

			TreeNavigator FindClientNavigator (Client client)
			{
				TreeNavigator navigator = _treeStore.GetFirstNode ();
				do {
					if (client.Equals (navigator.GetValue (_dataField))) {
						return navigator;
					}
				} while (navigator.MoveNext());
				return null;
			}

			TreeNavigator FindPortNavigator (Port port)
			{
				TreeNavigator navigator = FindClientNavigator (port.Client);
				if (navigator == null) {
					return null;
				}
				if (navigator.MoveToChild ()) {
					do {
						if (port.Equals (navigator.GetValue (_dataField))) {
							return navigator;
						}
					} while (navigator.MoveNext());
				}
				return null;
			}

			public void UpdateConnectable (IConnectable connectable)
			{
				Application.Invoke (() =>
				{
					Client client = connectable as Client;
					Port port = connectable as Port;
					if (client != null) {
						TreeNavigator navigator = FindClientNavigator (client);
						UpdateTreeStoreValues (navigator, connectable);
					}
					if (port != null) {
						TreeNavigator navigator = FindPortNavigator (port);
						UpdateTreeStoreValues (navigator, connectable);
					}
				});
			}

			void UpdateTreeStoreValues (TreeNavigator navigator, IConnectable connectable)
			{
				if (navigator != null) {
					navigator.SetValue (_dataField, connectable);
					navigator.SetValue (_textField, connectable.Name);
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
				return GetConnectable (position);
			}

			public double GetYPositionOfConnectable (IConnectable connectable)
			{
				double startPos = _treeView.VerticalScrollControl.Value * -1;
				// TODO: Get real row height
				double rowHeight = 22;
				// Start in the middle of line
				startPos -= rowHeight / 2;
				TreeNavigator navigator = _treeStore.GetFirstNode ();
				double clientHeight = 0;
				do {
					startPos += clientHeight;
				} while (!IsInClient(navigator, connectable, rowHeight, out clientHeight));
				startPos += clientHeight;
				return startPos;
			}

			bool IsInClient (TreeNavigator navigator, IConnectable connectable, double rowHeight, out double clientHeight)
			{
				clientHeight = rowHeight;
				IConnectable value = navigator.GetValue (_dataField);
				if (connectable.Equals (value)) {
					return true;
				}
				bool isClientExpanded = _treeView.IsRowExpanded (navigator.CurrentPosition);
				if (!navigator.MoveToChild ()) {
					return false;
				}
				do {
					if (IsPort (navigator, connectable, rowHeight, isClientExpanded, ref clientHeight)) {
						return true;
					}
				} while (navigator.MoveNext());
				navigator.MoveToParent ();
				return !navigator.MoveNext ();
			}

			bool IsPort (TreeNavigator navigator, IConnectable connectable, double rowHeight, bool isClientExpanded, ref double clientHeight)
			{
				IConnectable value = navigator.GetValue (_dataField);
				if (isClientExpanded) {
					clientHeight += rowHeight;
				}
				return connectable.Equals (value);
			}

			public void Clear ()
			{
				Application.Invoke (_treeStore.Clear);
				NotifyParent ();
			}
		}
	}
}