// 
// ConnectionDisplay.cs
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
using System.Collections.Generic;
using Mmj.ConnectionWrapper;
using Xwt;
using Mmj.Controllers.EventArguments;
using Mmj.Utilities;
using Mmj.OS;
using Command = Mmj.OS.Command;
using System.Linq;

namespace Mmj.Views.Widgets
{
	/// <summary>
	/// Widget for displaying and managing connections.
	/// </summary>
	public partial class ConnectionDisplay : Widget, IConnectionWidget
	{
		ConnectableTreeView _outTreeView;
		ConnectableTreeView _inTreeView;
		readonly List<IConnection> _connections = new List<IConnection> ();
		Button _connectButton;
		Button _disconnectButton;
		ConnectionArea _connectionArea;
		DateTime _lastLineUpdate = DateTime.Now;
		RichTextView _messageDisplay;
		ScrollView _messageContainer;
		readonly MessageCollection _messages = new MessageCollection ();
		IDisposable _timeout;
		readonly IKeyMap _keyMap = DependencyResolver.GetImplementation<IKeyMap> ("IKeyMap");

		public new void Dispose ()
		{
			Dispose(true);
		}

		~ConnectionDisplay()
		{
			Dispose(false);
		}

		protected new void Dispose(bool isDisposing)
		{

			_connectButton.Clicked -= ConnectButton_Click;
			_disconnectButton.Clicked -= DisconnectButton_Click;
			_inTreeView.ViewChanged -= OnTreeViewRowExpanded;
			_outTreeView.ViewChanged -= OnTreeViewRowExpanded;
			_inTreeView.Connect -= OnInTreeConnect;
			_outTreeView.Connect -= OnOutTreeConnect;
			KeyPressed -= OnKeyEvent;
			base.Dispose(isDisposing);
		}

		public ConnectionDisplay (string connectionManagerName)
		{
			ConnectionManagerName = connectionManagerName;
			BuildWidget ();
			BindEvents ();
			_keyMap.SetCommand (Command.Connect, CallConnect);
			_keyMap.SetCommand (Command.Disconnect, CallDisconnect);
		}

		void BuildWidget ()
		{
			VBox vbox = new VBox {
				ExpandVertical = true,
					       ExpandHorizontal = true
			};

			HBox buttonBox = new HBox ();
			_connectButton = new Button (Icons.Connect, I18N._ ("Connect"));
			buttonBox.PackStart (_connectButton);
			_disconnectButton = new Button (Icons.Disconnect, I18N._ ("Disconnect"));
			buttonBox.PackStart (_disconnectButton);
			vbox.PackStart (buttonBox);            

			_inTreeView = new ConnectableTreeView ();
			_outTreeView = new ConnectableTreeView ();

			_connectionArea = new ConnectionArea (_outTreeView, _inTreeView) {
				MinWidth = 200,
					 MinHeight = 200,
					 ExpandVertical = true,
					 ExpandHorizontal = true
			};
			HBox connectionBox = new HBox {
				ExpandVertical = true,
					       ExpandHorizontal = true
			};
			connectionBox.PackStart (_outTreeView, false, false);
			connectionBox.PackStart (_connectionArea, true, true);
			connectionBox.PackStart (_inTreeView, false, false);
			vbox.PackStart (connectionBox, true, true);

			_messageDisplay = new RichTextView ();
			_messageContainer = new ScrollView (_messageDisplay) {
				HorizontalScrollPolicy = ScrollPolicy.Never,
						       VerticalScrollPolicy = ScrollPolicy.Automatic,
						       HeightRequest = 40,
						       BorderVisible = true,
						       Margin = 2
			};
			_messageContainer.Hide ();
			vbox.PackEnd (_messageContainer);

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
			_inTreeView.Connect += OnInTreeConnect;
			_outTreeView.Connect += OnOutTreeConnect;
			KeyPressed += OnKeyEvent;
		}

		void OnKeyEvent (object sender, KeyEventArgs e)
		{
			_keyMap.ExecuteCommand (e.Key, e.Modifiers);
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

		public void AddMessage (string message, object[] parameters = null)
		{
			Application.Invoke (() => {
					_messages.AddMessage (message, parameters);
					_messageDisplay.LoadText (_messages.GetMessages (), Xwt.Formats.TextFormat.Markdown);
					_messageContainer.Show ();
					_messageContainer.VerticalScrollControl.Value = 0;
					});
			if (_timeout != null) {
				_timeout.Dispose ();
			}
			_timeout = Application.TimeoutInvoke (100, () => {
					string output = _messages.GetMessages ();
					if (string.IsNullOrEmpty (output)) {
					_messageContainer.Hide ();
					return false;
					}
					_messageDisplay.LoadText (output, Xwt.Formats.TextFormat.Markdown);
					return true;
					});
		}

		protected virtual void ConnectButton_Click (object sender, EventArgs e)
		{
			CallConnect ();
		}

		void CallConnect ()
		{
			IEnumerable<IConnectable> outlets = _outTreeView.GetSelected ();
			IEnumerable<IConnectable> inlets = _inTreeView.GetSelected ();
			if (outlets == null || inlets == null) {
				AddMessage ("Select ports or clients on both sides");
				return;
			}
			if (Connect != null) {
				Connect (this, new ConnectEventArgs {
						Outlets = outlets,
						Inlets = inlets
						});
			}
		}

		void ConnectFromDragAndDrop (ConnectEventArgs e)
		{
			if (Connect != null) {
				Connect (this, new ConnectEventArgs {
						Outlets = e.Outlets,
						Inlets = e.Inlets
						});
			}
		}

		void OnInTreeConnect (object sender, ConnectEventArgs e)
		{
			IEnumerable<IConnectable> realOutlet = e.Inlets;
			e.Inlets = e.Outlets;
			e.Outlets = realOutlet;
			ConnectFromDragAndDrop (e);
		}

		void OnOutTreeConnect (object sender, ConnectEventArgs e)
		{
			ConnectFromDragAndDrop (e);
		}

		protected virtual void DisconnectButton_Click (object sender, EventArgs e)
		{
			CallDisconnect ();
		}

		void CallDisconnect ()
		{
			if (Disconnect != null) {
				int notSelected = 0;
				IEnumerable<IConnectable> outlets = _outTreeView.GetSelected ();
				if (!outlets.Any ()) {
					outlets = _outTreeView.GetAll ();
					notSelected += 1;
				}
				IEnumerable<IConnectable> inlets = _inTreeView.GetSelected ();
				if (!inlets.Any ()) {
					inlets = _inTreeView.GetAll ();
					notSelected += 1;
				}
				if (notSelected == 2) {
					AddMessage ("No port or client selected");
					return;
				}
				Disconnect (this, new ConnectEventArgs {
						Outlets = outlets,
						Inlets = inlets
						});
			}
		}

		void UpdateConnectionLines ()
		{
			DateTime now = DateTime.Now;
			if (now - _lastLineUpdate < TimeSpan.FromSeconds (0.01)) {
				return;
			}
			try {
				_connectionArea.Clear ();
				_connectionArea.SetConnections (_connections);
				_connectionArea.QueueDraw ();
				_lastLineUpdate = now;
			} catch (Exception ex) {
				Logging.LogException (ex);
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
