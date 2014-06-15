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
// THE SOFTWARE.using System;
using System;
using System.Management.Instrumentation;
using MonoMultiJack.ConnectionWrapper;
using Xwt;

namespace MonoMultiJack.Widgets
{
	class ConnectableTreeView : Widget
	{
		private ScrollView scrollView;
		private TreeView treeView;
		private readonly TreeStore treeStore;
		private readonly IDataField<IConnectable> dataField;
		private readonly IDataField<string> textField;

		public ConnectableTreeView ()
		{
			dataField = new DataField<IConnectable> ();
			textField = new DataField<string> ();
			treeStore = new TreeStore (new IDataField[] { dataField, textField });
			treeView = new TreeView (treeStore);
			treeView.Columns.Add ("", textField);
			scrollView = new ScrollView (treeView) {
				MinHeight = 200,
				MinWidth = 300,
				ExpandVertical = true
			};
			this.Content = scrollView;
			scrollView.VisibleRectChanged += UpdateParent;
			treeView.RowExpanded += UpdateParent;
		}

		public event EventHandler ViewChanged;

		private void UpdateParent (object sender, EventArgs e)
		{
			if (ViewChanged != null) {
				ViewChanged (this, e);
			}
		}

		public void AddConnectable (IConnectable connectable)
		{
			Client client = connectable as Client;
			if (client != null) {
				TreeNavigator navigator = treeStore.GetFirstNode ();
				navigator = AddClient (navigator, client);
				foreach (Port port in client.Ports) {
					navigator = AddPort (navigator, port);
				}
			} else {
				throw new ArgumentOutOfRangeException ("connectable", connectable, "Only clients can be added to TreeView.");
			}
		}

		private TreeNavigator AddClient (TreeNavigator navigator, Client client)
		{
			bool alreadyAdded = false;
			do {
				if (navigator.CurrentPosition != null) {
					if (client.Equals (navigator.GetValue (dataField))) {
						alreadyAdded = true;
						break;
					}
				}
			} while (navigator.CurrentPosition != null && navigator.MoveNext());
			if (!alreadyAdded) {
				navigator = treeStore.AddNode ().SetValue (dataField, client).SetValue (textField, client.Name);
			}
			return navigator;
		}

		private TreeNavigator AddPort (TreeNavigator navigator, Port port)
		{
			navigator.MoveToChild ();
			bool alreadyAdded = false;
			do {
				if (navigator.CurrentPosition != null) {
					if (port.Equals (navigator.GetValue (dataField))) {
						alreadyAdded = true;
						break;
					}
				}
			} while (navigator.CurrentPosition != null && navigator.MoveNext());
			navigator.MoveToParent ();
			if (!alreadyAdded) {
				navigator.AddChild ().SetValue (dataField, port).SetValue (textField, port.Name);
			}
			navigator.MoveToParent ();
			return navigator;
		}

		public void RemoveConnectable (IConnectable connectable)
		{
			Client client = connectable as Client;
			Port port = connectable as Port;
			if (client != null) {
				RemoveClient (client);
			}
			if (port != null) {
				RemovePort (port);
			}
		}

		private void RemoveClient (Client client)
		{
			TreeNavigator navigator = FindClientNavigator (client);
			navigator.RemoveChildren ();
			navigator.Remove ();
		}

		private void RemovePort (Port port)
		{
			TreeNavigator navigator = FindClientNavigator (port.Client);
			navigator.MoveToChild ();
			do {
				if (port.Equals (navigator.GetValue (dataField))) {
					navigator.Remove ();
					break;
				}
			} while (navigator.MoveNext());
			navigator.MoveToParent ();
			if (!navigator.MoveToChild ()) {
				navigator.Remove ();
			}
		}

		private TreeNavigator FindClientNavigator (Client client)
		{
			TreeNavigator navigator = treeStore.GetFirstNode ();
			do {
				if (client.Equals (navigator.GetValue (dataField)))
					return navigator;
			} while (navigator.MoveNext());
			return null;
		}

		public void UpdateConnectable (IConnectable connectable)
		{
			throw new NotImplementedException ();
		}

		public IConnectable GetSelected ()
		{
			throw new NotImplementedException ();
		}

		public int GetYPositionOfConnectable (IConnectable connectable)
		{
			return 12;
		}
	}
}
