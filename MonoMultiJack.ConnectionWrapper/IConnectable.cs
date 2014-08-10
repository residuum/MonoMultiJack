//
// IConnectable.cs
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
using System.Collections.Generic;

namespace MonoMultiJack.ConnectionWrapper
{
	public interface IConnectable
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		string Name { get; }

		/// <summary>
		/// Gets the serialization.
		/// </summary>
		/// <value>The serialization.</value>
		ConnectableSerialization Serialization  { get; }

		/// <summary>
		/// Gets the ports.
		/// </summary>
		/// <value>
		/// The ports.
		/// </value>
		IEnumerable<Port> Ports { get; }

		/// <summary>
		/// Gets the type of the connection.
		/// </summary>
		/// <value>
		/// The type of the connection.
		/// </value>
		ConnectionType ConnectionType { get; }

		/// <summary>
		/// Gets the flow direction.
		/// </summary>
		/// <value>
		/// The flow direction.
		/// </value>
		FlowDirection FlowDirection { get; }
	}
}