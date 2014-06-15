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
using System.Configuration;
using MonoMultiJack.ConnectionWrapper;
using MonoMultiJack.Controllers.EventArguments;
using Xwt;
using Xwt.Drawing;
using Colors = MonoMultiJack.Widgets.ConnectionColors.Colors;

namespace MonoMultiJack.Widgets
{
	/// <summary>
	/// Widget for displaying and managing connections.
	/// </summary>
	public class ConnectionDisplay : Widget, IConnectionWidget
	{
		private ConnectableTreeView _outTreeView;
		private ConnectableTreeView _inTreeView;
		readonly List<IConnection> _connections = new List<IConnection> ();
		private Button _connectButton;
		private Button _disconnectButton;
		private Canvas _connectionArea;
		DateTime _lastLineUpdate = DateTime.Now;
		int _cellHeight = 0;

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
			this._connectButton = new Button ("Connect");
			hbox1.PackStart (this._connectButton);
			this._disconnectButton = new Button ("Disconnect");
			hbox1.PackStart (this._disconnectButton);
			vbox.PackStart (hbox1); 
            
            
			_inTreeView = new ConnectableTreeView ();
			_outTreeView = new ConnectableTreeView ();

			_connectionArea = new Canvas {
				MinWidth = 200,
				MinHeight = 200,
				ExpandVertical = true,
				ExpandHorizontal = true
			};
			HBox hbox2 = new HBox { ExpandVertical = true, ExpandHorizontal = true };
			hbox2.PackStart (_outTreeView);
			hbox2.PackStart (_connectionArea);
			hbox2.PackStart (_inTreeView);

			vbox.PackEnd (hbox2);
			Content = vbox;
			this.ExpandHorizontal = true;
			this.ExpandVertical = true;

		}

		void BindEvents ()
		{
			_connectButton.Clicked += ConnectButton_Click;
			_disconnectButton.Clicked += DisconnectButton_Click;
			_inTreeView.ViewChanged += OnTreeViewRowExpanded;
			_outTreeView.ViewChanged += OnTreeViewRowExpanded;
            
			//this.GotFocus += OnExpose;

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
				//if (_connectionArea.GdkWindow == null)
				//{
				//    return;
				//}
				//_connectionArea.GdkWindow.Clear();
				//using (Context g = Gdk.CairoHelper.Create(_connectionArea.GdkWindow))
				//{
				//    g.Antialias = Antialias.Subpixel;
				//    List<IConnection> connections = new List<IConnection>(_connections);
				//    for (int i = 0; i < connections.Count; i++)
				//    {
				//        IConnection conn = connections[i];
				//        int outY = GetYPositionForPort(_outputTreeview, _outputStore, conn.OutPort);
				//        int inY = GetYPositionForPort(_inputTreeview, _inputStore, conn.InPort);
				//        int areaWidth = _connectionArea.Allocation.Width;
				//        if (outY != -1 && inY != -1)
				//        {
				//            g.Save();
				//            g.MoveTo(0, outY);
				//            g.CurveTo(
				//                new PointD(areaWidth / 4, outY),
				//                new PointD(3 * areaWidth / 4, inY),
				//                new PointD(areaWidth, inY)
				//            );
				//            g.Restore();
				//        }
				//        // TODO: Find a way to get the background color
				//        g.Color = Colors.GetColor(i, new Color());
				//        g.LineWidth = 1;
				//        g.Stroke();
				//    }
				//    g.Target.Dispose();
				//    _lastLineUpdate = now;
				//}
			} catch (Exception ex) {
#if DEBUG
				Console.WriteLine (ex.Message);
#endif
			}
		}

		protected virtual void OnTreeViewRowExpanded (object o, EventArgs args)
		{
			Application.Invoke (delegate {
				UpdateConnectionLines ();
			}
			);
		}

		protected virtual void OnTreeViewRowCollapsed (object o, EventArgs args)
		{
			Application.Invoke (delegate {
				UpdateConnectionLines ();
			}
			);
		}
		#region IConnectionWidget implementation
		public event ConnectEventHandler Connect;
		public event ConnectEventHandler Disconnect;

		public void Clear ()
		{
			//_inputStore.Clear ();
			//_outputStore.Clear ();
			//_connections.Clear ();
			//Application.Invoke (delegate {
			//    UpdateConnectionLines ();
			//}
			//);
		}

		public void AddConnection (IConnection connection)
		{			
			//_connections.Add (connection);
			
			//Application.Invoke (delegate {
			//    UpdateConnectionLines ();
			//}
			//);
		}

		public void RemoveConnection (IConnection connection)
		{
			//_connections.Remove (connection);
			
			//Application.Invoke (delegate {
			//    UpdateConnectionLines ();
			//}
			//);
		}

		public string ConnectionManagerName { get; private set; }
		#endregion
	}
}
