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
		
		internal static bool Activate ()
		{
			int activation = snd_seq_open (out _alsaClient, "default", SND_SEQ_OPEN_DUPLEX, SND_SEQ_NONBLOCK);
#if DEBUG
			Console.WriteLine ("Alsa Activation: " + activation);
#endif
			if (activation == 0)
			{
				snd_seq_set_client_name(_alsaClient, "MonoMultiJack");
				return true;
			}
			return false;
		}
		
		
		internal static void DeActivate ()
		{
			if (_alsaClient != IntPtr.Zero)
			{
				snd_seq_close(_alsaClient);
			}
		}
		
		
		internal static IEnumerable<Port> GetPorts()
		{
			if (_alsaClient != IntPtr.Zero || Activate())
			{
				IntPtr clientInfo = IntPtr.Zero;
				IntPtr portInfo = IntPtr.Zero;
				var ports = new List<Port>();
				
				try
				{
					int clientInfoSize = snd_seq_client_info_sizeof().ToInt32();
					int portInfoSize = snd_seq_port_info_sizeof().ToInt32();
					clientInfo = Marshal.AllocHGlobal(clientInfoSize);
					portInfo = Marshal.AllocHGlobal(portInfoSize);
					snd_seq_client_info_set_client(out clientInfo, -1);
					snd_seq_set_client_info(_alsaClient, out clientInfo);
					
					while (snd_seq_query_next_client(_alsaClient, out clientInfo) == 0)
					{
 						int clientId = snd_seq_client_info_get_client(clientInfo);
						snd_seq_port_info_set_client(out portInfo, clientId);
						snd_seq_port_info_set_port(out portInfo, -1);
					
						while (snd_seq_query_next_port(_alsaClient, out portInfo) == 0)
						{
							ports.Add(new Port(snd_seq_port_info_get_name(portInfo).ToString(), snd_seq_client_info_get_name(clientInfo).ToString(),
									PortType.Output, ConnectionType.AlsaMidi));
						}				
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					return new Port[0];
				}
				finally
				{
					Marshal.FreeHGlobal(clientInfo);
					Marshal.FreeHGlobal(portInfo);
				}
				return ports;
			}
			return new Port[0];
		}
	}
}

