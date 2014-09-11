// 
// MessageCollection.cs
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

namespace MonoMultiJack.Widgets
{
	partial class ConnectionDisplay
	{
		private class MessageCollection
		{
			readonly List<Message> _messages = new List<Message>();
			static readonly TimeSpan MessageTimeout = TimeSpan.FromSeconds (10);

			public void AddMessage (string message)
			{
				lock (_messages) {
					_messages.Add (new Message {
						Created = DateTime.Now, 
						Content = message
					});
				}
			}

			public string GetMessages ()
			{
				List<string> outputMessages = new List<string> ();
				lock (_messages) {
					for (int i = _messages.Count - 1; i >= 0; i--) {
						Message message = _messages[i];
						if (message.Created.Add (MessageTimeout) < DateTime.Now) {
							_messages.RemoveAt (i);
						} else {
							outputMessages.Add (string.Format ("**{0}**: {1}", message.Created.ToLongTimeString (), message.Content));
						}
					}
				}
				return " " + string.Join ("  \n", outputMessages);
			}

			private class Message
			{
				public DateTime Created { get; set; }
				public string Content { get; set; }
			}
		}
	}
}
