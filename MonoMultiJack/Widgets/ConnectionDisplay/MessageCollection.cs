using System;
using System.Collections.Generic;

namespace MonoMultiJack.Widgets
{
	partial class ConnectionDisplay
	{
		private class MessageCollection
		{
			readonly Dictionary<DateTime, string> _messages = new Dictionary<DateTime, string> ();
			static readonly TimeSpan MessageTimeout = TimeSpan.FromSeconds (3);

			public void AddMessage(string message)
			{
				lock (_messages) {
					_messages.Add(DateTime.Now.Add (MessageTimeout), message);
				}
			}

			public string GetMessages()
			{
				List<string> outputMessages = new List<string> ();
				lock (_messages) {
					List<DateTime> oldTimes = new List<DateTime> ();
					foreach (var message in _messages) {
						if (message.Key > DateTime.Now) {
							string data;
							if (_messages.TryGetValue (message.Key, out data)) {
								outputMessages.Add (data);
							}
						} else {
							oldTimes.Add (message.Key);
						}
					}
					foreach (var dateTime in oldTimes) {
						_messages.Remove (dateTime);
					}
				}
				return string.Join (Environment.NewLine, outputMessages);
			}
		}
	}
}
