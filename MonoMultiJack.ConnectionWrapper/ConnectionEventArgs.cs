// 
// ConnectionEventHandler.cs
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

namespace MonoMultiJack.ConnectionWrapper
{
	/// <summary>
	/// Event arguments for connection management.
	/// </summary>
	public class ConnectionEventArgs : EventArgs
	{
		/// <summary>
		/// The message for the event.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the connectables.
		/// </summary>
		/// <value>
		/// The connectables.
		/// </value>
		public IEnumerable<IConnectable> Connectables { get; set; }

		/// <summary>
		/// The affected ConnectionType
		/// </summary>
		public ConnectionType ConnectionType { get; set; }

		/// <summary>
		/// The Type of message
		/// </summary>
		public MessageType MessageType { get; set; }

		/// <summary>
		/// The connections affected by the event.
		/// </summary>
		public IEnumerable<IConnection> Connections { get; set; }

		/// <summary>
		/// The type of change of the ports or connections.
		/// </summary>
		public ChangeType ChangeType { get; set; }
	}
}