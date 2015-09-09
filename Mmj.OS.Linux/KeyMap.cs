using System;
using System.Collections.Generic;
using Xwt;

namespace Mmj.OS
{
	public class KeyMap : IKeyMap
	{
		readonly Dictionary<Command, Action> _commandMap = new Dictionary<Command, Action> ();

		public void ExecuteCommand (Key key, ModifierKeys modifiers)
		{
			Command command = GetCommand (key, modifiers);
			Action action;
			if (_commandMap.TryGetValue (command, out action)) {
				action.Invoke ();
			}
		}

		public void SetCommand (Command command, Action action)
		{
			Action oldAction;
			if (_commandMap.TryGetValue (command, out oldAction)) {
				_commandMap [command] = action;
			} else {
				_commandMap.Add (command, action);
			}
		}

		static Command GetCommand (Key key, ModifierKeys modifiers)
		{
			modifiers = modifiers & ~ModifierKeys.Command;

			if (key == Key.F1 && modifiers == ModifierKeys.None) {
				return Command.Help;
			}
			if ((key == Key.F4 && modifiers == ModifierKeys.Alt)
			    || (key == Key.q && modifiers == ModifierKeys.Control)) {
				return Command.Quit;
			}
			if (key == Key.f && modifiers == ModifierKeys.None) {
				return Command.Fullscreen;
			}
			if ((key == Key.c && modifiers == ModifierKeys.Control)) {
				return Command.Connect;
			}
			if ((key == Key.d && modifiers == ModifierKeys.Control)) {
				return Command.Disconnect;
			}
			return Command.Undefined;
		}
	}
}