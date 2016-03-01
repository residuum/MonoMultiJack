//
// ConnectableSerialization.cs
//
// Author:
//       Thomas Mayer <thomas@residuum.org>
//
// Copyright (c) 2014 Thomas Mayer
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
using System.Collections.Generic;

namespace Mmj.ConnectionWrapper
{
	public class ConnectableSerialization
	{
		public IConnectable GetConnectable ()
		{
			Client client = new Client ("", FlowDirection, ConnectionType);
			foreach (int portId in _portIds) {
				client.AddPort (new DummyPort (portId, ConnectionType, FlowDirection));
			}
			return client;
		}

		public ConnectionType ConnectionType{ get; private set; }

		public FlowDirection FlowDirection{ get; private set; }

		readonly IEnumerable<int> _portIds;

		public ConnectableSerialization (string connectableId)
		{
			string[] parameters = connectableId.Split (new char[] { '/' });
			if (parameters.Length != 3) {
				throw new ArgumentOutOfRangeException ();
			}
			ConnectionType = (ConnectionType)Enum.Parse (typeof(ConnectionType), parameters [0]);
			FlowDirection = (FlowDirection)Enum.Parse (typeof(FlowDirection), parameters [1]);
			string[] portIds = parameters [2].Split (new char[] { ',' });
			_portIds = portIds.Select (id => Convert.ToInt32(id)).ToList ();
		}

		public ConnectableSerialization (ConnectionType connectionType, FlowDirection flowDirection, IEnumerable<int> portIds)
		{
			ConnectionType = connectionType;
			FlowDirection = flowDirection;
			_portIds = portIds;
		}

		public override string ToString ()
		{			
			return string.Format ("{0}/{1}/{2}", ConnectionType, FlowDirection, string.Join (",", _portIds));
		}
	}
}

