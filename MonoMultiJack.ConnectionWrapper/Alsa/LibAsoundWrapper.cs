// 
// LibAsoundWrapper.cs
//  
// Author:
//       Thomas Mayer <thomas@residuum.org>
// 
// Copyright (c) 2010 Thomas Mayer
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
using Mono.Unix;
using System.Runtime.InteropServices;

namespace MonoMultiJack.ConnectionWrapper.Alsa
{
	internal static partial class LibAsoundWrapper
	{		
		private static IntPtr _alsaClient = IntPtr.Zero;
		private static List<AlsaPort> _portMapper = new List<AlsaPort>();
		private static List<IConnection> _connections = new List<IConnection>();
		
		internal static void Activate ()
		{
			int activation = snd_seq_open (out _alsaClient, "default", SND_SEQ_OPEN_DUPLEX, SND_SEQ_NONBLOCK);
			Console.WriteLine ("Alsa Activation: " + activation);
			if (activation == 0)
			{
				snd_seq_set_client_name(_alsaClient, "MonoMultiJack");
			}
		}
		
		
		internal static void DeActivate ()
		{
			if (_alsaClient != IntPtr.Zero)
			{
				snd_seq_close(_alsaClient);
			}
		}
	}
}

