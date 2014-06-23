// 
// ConnectionDisplay.cs
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
using System.Collections.Generic;
using MonoMultiJack.ConnectionWrapper;
using MonoMultiJack.Controllers.EventArguments;
using Xwt;

namespace MonoMultiJack.Widgets
{
	/// <summary>
	/// Widget for displaying and managing connections.
	/// </summary>
	public class ConnectionDisplay : Widget, IConnectionWidget
	{
		ConnectableTreeView _outTreeView;
		ConnectableTreeView _inTreeView;
		readonly List<IConnection> _connections = new List<IConnection> ();
		Button _connectButton;
		Button _disconnectButton;
		ConnectionArea _connectionArea;
		DateTime _lastLineUpdate = DateTime.Now;

		public new void Dispose ()
		{
			base.Dispose ();
		}

		public ConnectionDisplay (string connectionManagerName)
		{
			ConnectionManagerName = connectionManagerName;
			BuildWidget ();
			BindEvents ();
		}

		void BuildWidget ()
		{
			VBox vbox = new VBox {
				ExpandVertical = true,
				ExpandHorizontal = true
			};

			HBox hbox1 = new HBox ();
			_connectButton = new Button ("Connect");
			hbox1.PackStart (_connectButton);
			_disconnectButton = new Button ("Disconnect");
			hbox1.PackStart (_disconnectButton);
			vbox.PackStart (hbox1); 
            
            
			_inTreeView = new ConnectableTreeView ();
			_outTreeView = new ConnectableTreeView ();

			_connectionArea = new ConnectionArea (_outTreeView, _inTreeView) {
				MinWidth = 200,
				MinHeight = 200,
				ExpandVertical = true,
				ExpandHorizontal = true
			};
			HBox hbox2 = new HBox {
				ExpandVertical = true,
				ExpandHorizontal = true
			};
			hbox2.PackStart (_outTreeView, false, false);
			hbox2.PackStart (_connectionArea, true, true);
			hbox2.PackStart (_inTreeView, false, false);

			vbox.PackEnd (hbox2, true, true);
			Content = vbox;
			ExpandHorizontal = true;
			ExpandVertical = true;

		}

		void BindEvents ()
		{
			_connectButton.Clicked += ConnectButton_Click;
			_disconnectButton.Clicked += DisconnectButton_Click;
			_inTreeView.ViewChanged += OnTreeViewRowExpanded;
			_outTreeView.ViewChanged += OnTreeViewRowExpanded;
		}

		public void AddConnectable (IConnectable connectable)
		{
			if (connectable.FlowDirection == FlowDirection.In) {
				_inTreeView.AddConnectable (connectable);
			} else if (connectable.FlowDirection == FlowDirection.Out) {
				_outTreeView.AddConnectable (connectable);
			}
		}

		public void RemoveConnectable (IConnectable connectable)
		{
			if (connectable.FlowDirection == FlowDirection.In) {
				_inTreeView.RemoveConnectable (connectable);
			} else if (connectable.FlowDirection == FlowDirection.Out) {
				_outTreeView.RemoveConnectable (connectable);
			}
		}

		public void UpdateConnectable (IConnectable connectable)
		{
			if (connectable.FlowDirection == FlowDirection.In) {
				_inTreeView.UpdateConnectable (connectable);
			} else if (connectable.FlowDirection == FlowDirection.Out) {
				_outTreeView.UpdateConnectable (connectable);
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
		protected virtual void ConnectButton_Click (object sender, EventArgs e)
		{
			if (Connect != null) {
				Connect (this, new ConnectEventArgs {
					Outlet = _outTreeView.GetSelected (),
					Inlet = _inTreeView.GetSelected ()
				});
			}
		}

		protected virtual void DisconnectButton_Click (object sender, EventArgs e)
		{
			if (Disconnect != null) {
				Disconnect (this, new ConnectEventArgs {
					Outlet = _outTreeView.GetSelected (),
					Inlet = _inTreeView.GetSelected ()
				});
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
			try {
				_connectionArea.Clear ();
				_connectionArea.SetConnections (_connections);
				_connectionArea.QueueDraw (_connectionArea.Bounds);
				_lastLineUpdate = now;
			} catch (Exception ex) {
#if DEBUG
				Console.WriteLine (ex.Message);
#endif
			}
		}

		protected virtual void OnTreeViewRowExpanded (object o, EventArgs args)
		{
			Application.Invoke (UpdateConnectionLines);
		}

		protected virtual void OnTreeViewRowCollapsed (object o, EventArgs args)
		{
			Application.Invoke (UpdateConnectionLines);
		}
		#region IConnectionWidget implementation
		public event ConnectEventHandler Connect;
		public event ConnectEventHandler Disconnect;

		public void Clear ()
		{
			_inTreeView.Clear ();
			_outTreeView.Clear ();
			_connections.Clear ();
			Application.Invoke (UpdateConnectionLines);
		}

		public void AddConnection (IConnection connection)
		{			
			_connections.Add (connection);
            
			Application.Invoke (UpdateConnectionLines);
		}

		public void RemoveConnection (IConnection connection)
		{
			_connections.Remove (connection);

			Application.Invoke (UpdateConnectionLines);
		}

		public string ConnectionManagerName { get; private set; }
		#endregion
	}
}
