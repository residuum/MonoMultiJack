// 
// ConnectionArea.cs
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
using System.Collections.Generic;
using System.Linq;
using MonoMultiJack.ConnectionWrapper;
using Xwt;
using Xwt.Drawing;

namespace MonoMultiJack.Widgets
{
	public partial class ConnectionDisplay
	{
		class ConnectionArea : Canvas
		{
			readonly ConnectableTreeView _outTreeView;
			readonly ConnectableTreeView _inTreeView;
			IEnumerable<IConnection> _connections;

			public ConnectionArea (ConnectableTreeView outTreeView, ConnectableTreeView inTreeView)
			{
				_outTreeView = outTreeView;
				_inTreeView = inTreeView;
			}

			public void SetConnections (IEnumerable<IConnection> connections)
			{
				_connections = connections;
			}

			protected override void OnDraw (Context ctx, Rectangle dirtyRect)
			{
				List<IConnection> connections = new List<IConnection> (_connections ?? Enumerable.Empty<IConnection> ());
				for (int i = 0; i < connections.Count; i++) {
					IConnection conn = connections [i];
					double outY = _outTreeView.GetYPositionOfConnectable (conn.OutPort);
					double inY = _inTreeView.GetYPositionOfConnectable (conn.InPort);
					double areaWidth = this.Bounds.Width;
					ctx.Save ();
					ctx.MoveTo (0, outY);
					ctx.CurveTo (new Point (areaWidth / 4, outY),
					            new Point (3 * areaWidth / 4, inY),
					            new Point (areaWidth, inY));
					ctx.Restore ();
					ctx.SetColor (i.GetColor (BackgroundColor));
					ctx.SetLineWidth (1);
					ctx.Stroke ();
				}
				base.OnDraw (ctx, dirtyRect);
			}
		}
	}
}