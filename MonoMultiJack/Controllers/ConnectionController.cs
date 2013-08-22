
//
// ConnectionDisplayController.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2012 Thomas Mayer
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
using MonoMultiJack.ConnectionWrapper;
using MonoMultiJack.Widgets;
using MonoMultiJack.Controllers.EventArguments;

namespace MonoMultiJack.Controllers
{
	public class ConnectionController : IController
	{

		IConnectionWidget _connectionWidget;
		IConnectionManager _connectionManager;

		public IConnectionWidget Widget {
			get {
				return _connectionWidget;
			}
		}

		public ConnectionController (IConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
			_connectionWidget = new ConnectionDisplay (connectionManager.Name);

			_connectionManager.BackendHasExited += ConnectionManager_BackendHasExited;
			_connectionManager.ConnectionHasChanged += ConnectionManager_ConnectionHasChanged;
			IEnumerable<IConnectable> clients = _connectionManager.Clients;
			if (clients != null) {
				foreach (IConnectable client in _connectionManager.Clients) {
					_connectionWidget.AddConnectable (client);
				}
			}
			
			_connectionWidget.Connect += Widget_Connect;
			_connectionWidget.Disconnect += Widget_Disconnect;
		}

		void Widget_Connect(object sender, ConnectEventArgs args){

		}

		void Widget_Disconnect(object sender, ConnectEventArgs args){

		}

		void ConnectionManager_BackendHasExited (object sender, ConnectionEventArgs args)
		{
			_connectionWidget.Clear ();
		}

		void ConnectionManager_ConnectionHasChanged (object sender, ConnectionEventArgs args)
		{
			if (args.ChangeType == ChangeType.New) {
				if (args.Connections != null) {
					foreach (IConnection connection in args.Connections) {
						_connectionWidget.AddConnection (connection);
					}
				}
				if (args.Connectables != null) {
					foreach (IConnectable connectable in args.Connectables) {
						_connectionWidget.AddConnectable (connectable);
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
			}
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
			_connectionWidget.Destroy ();
			_connectionWidget.Dispose ();
		}

		public event EventHandler AllWidgetsAreClosed;
	}
}

