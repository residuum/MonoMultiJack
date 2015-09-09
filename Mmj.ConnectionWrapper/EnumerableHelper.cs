//
// EnumerableHelper.cs
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
using System.Linq;

namespace Mmj.ConnectionWrapper
{
	/// <summary>
	/// Enumerable helper.
	/// </summary>
	public static class EnumerableHelper
	{
		/// <summary>
		/// Pairs the ports.
		/// </summary>
		/// <returns>
		/// The ports.
		/// </returns>
		/// <param name='outlet'>
		/// Outlet.
		/// </param>
		/// <param name='inlet'>
		/// Inlet.
		/// </param>
		public static IEnumerable<KeyValuePair<Port, Port>> PairPorts (IEnumerable<IConnectable> outlet, IEnumerable<IConnectable> inlet)
		{
			List<Port> outPorts = outlet.SelectMany(c => c.Ports).ToList ();
			List<Port> inPorts = inlet.SelectMany(c => c.Ports).ToList ();
			int portCount = Math.Min (outPorts.Count, inPorts.Count);
			for (int i = 0; i <portCount; i++) {
				yield return new KeyValuePair<Port, Port> (outPorts [i], inPorts [i]);
			}
		}

		public static IEnumerable<KeyValuePair<Port, Port>> PairAll (IEnumerable<IConnectable> outlets, IEnumerable<IConnectable> inlets)
		{		
			List<Port> outPorts = outlets.SelectMany(c => c.Ports).ToList ();
			List<Port> inPorts = inlets.SelectMany(c => c.Ports).ToList ();
			foreach (Port outPort in outPorts) {
				foreach (Port inPort in inPorts) {
					yield return new KeyValuePair<Port, Port> (outPort, inPort);
				}
			}
		}
	}
}
