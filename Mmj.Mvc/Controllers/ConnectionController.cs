//
// ConnectionController.cs
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
using Mmj.Utilities;
using Mmj.ConnectionWrapper;
using Mmj.Controllers.EventArguments;
using Mmj.Views.Widgets;
using Mmj.OS;
using System.Linq;

namespace Mmj.Controllers
{
	public class ConnectionController : IController
	{
		readonly IConnectionWidget _connectionWidget;
		readonly IConnectionManager _connectionManager;

		public IConnectionWidget Widget {
			get {
				return _connectionWidget;
			}
		}

		public ConnectionController (IConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
			_connectionWidget = new ConnectionDisplay (I18N._ (connectionManager.Name));

			_connectionManager.BackendHasChanged += ConnectionManager_BackendHasChanged;
			_connectionManager.ConnectionHasChanged += ConnectionManager_ConnectionHasChanged;
			IEnumerable<IConnectable> clients = _connectionManager.Clients;
			if (clients != null) {
				foreach (IConnectable client in clients) {
					_connectionWidget.AddConnectable (client);
				}
			}
			
			_connectionWidget.Connect += Widget_Connect;
			_connectionWidget.Disconnect += Widget_Disconnect;
		}

		public IEnumerable<IConnection> Connections {
			get {
				return this._connectionManager.Connections;
			}
		}

		void Widget_Connect (object sender, ConnectEventArgs args)
		{
			Logging.LogMessage ("Connecting", LogLevel.Debug);
			_connectionManager.Connect (args.Outlets, args.Inlets);
		}

		void Widget_Disconnect (object sender, ConnectEventArgs args)
		{
			Logging.LogMessage ("Disconnecting", LogLevel.Debug);
			_connectionManager.Disconnect (args.Outlets, args.Inlets);
		}

		void ConnectionManager_BackendHasChanged (object sender, ConnectionEventArgs args)
		{
			Logging.LogMessage (args);
			switch (args.ChangeType) {
			case ChangeType.BackendExited:
				_connectionWidget.Clear ();
				break;
			}
			_connectionWidget.AddMessage (args.Message, args.Params);
		}

		void ConnectionManager_ConnectionHasChanged (object sender, ConnectionEventArgs args)
		{
			Logging.LogMessage (args);
			if (args.ChangeType == ChangeType.New) {
				if (args.Connectables != null) {
					foreach (IConnectable connectable in args.Connectables) {
						_connectionWidget.AddConnectable (connectable);
					}
				}
				if (args.Connections != null) {
					foreach (IConnection connection in args.Connections) {
						_connectionWidget.AddConnection (connection);
					}
				}
			} else if (args.ChangeType == ChangeType.Deleted) {
				if (args.Connections != null) {				
					foreach (IConnection connection in args.Connections) {
						_connectionWidget.RemoveConnection (connection);
					}
				}
				if (args.Connectables != null) {				
					foreach (IConnectable connectable in args.Connectables) {
						_connectionWidget.RemoveConnectable (connectable);
					}
				}
			} else if (args.ChangeType == ChangeType.Content) {
				if (args.Connectables != null) {
					foreach (IConnectable connectable in args.Connectables) {
						_connectionWidget.UpdateConnectable (connectable);
					}
				}
			}
			_connectionWidget.AddMessage (args.Message, args.Params);
		}

		~ConnectionController ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool isDisposing)
		{
			_connectionWidget.Dispose ();
		}
	}
}
