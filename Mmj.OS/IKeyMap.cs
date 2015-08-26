using System;
using Xwt;

namespace Mmj.OS
{
	public interface IKeyMap
	{
		void ExecuteCommand (Key key, ModifierKeys modifiers);
		void SetCommand (Command command, Action action);
	}
}
