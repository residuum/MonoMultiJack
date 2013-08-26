// 
// JackPort.cs
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
namespace MonoMultiJack.ConnectionWrapper.Jack.Types
{
	internal class JackPort : Port
	{			
		public IntPtr JackPortPointer { get; private set; }
			
		public string JackPortName { get; private set; }

		public string ClientName { get; private set; }
		
		public JackPort (string jackPortName, IntPtr jackPortPtr, FlowDirection portType, ConnectionType connectionType, uint portId)
		{				
			JackPortName = jackPortName;
			JackPortPointer = jackPortPtr;
			FlowDirection = portType;
			ConnectionType = connectionType;
			string[] splittedName = jackPortName.Split (new[] { ':' });
			ClientName = splittedName [0];
			Name = splittedName [1];				
			Id = portId;
		}
	}
}

