using Cairo;

namespace MonoMultiJack.Widgets.ConnectionColors
{
	static class Colors
	{
		internal static Color GetColor (int index, Color backgroundColor)
		{
			// TODO: Make colors dependant of backgroundColor, so that lines
			// are visible on dark and light themes.
			switch (index % 13) {
			case 1:
				return new Color (0, 0, 0);
			case 2:
				return new Color (0.3, 0, 0);
			case 3:
				return new Color (0, 0.3, 0);
			case 4:
				return new Color (0, 0, 0.3);
			case 5:
				return new Color (0.3, 0.3, 0);
			case 6:
				return new Color (0, 0.3, 0.3);
			case 7:
				return new Color (0.3, 0, 0.3);
			case 8:
				return new Color (0.6, 0.3, 0);
			case 9:
				return new Color (0, 0.6, 0.3);
			case 10:
				return new Color (0.3, 0, 0.6);
			case 11:
				return new Color (0.3, 0.6, 0);
			case 12:
				return new Color (0, 0.3, 0.6);
			case 0:
				return new Color (0.6, 0, 0.3);

			}
			return new Color (0, 0, 0);
		}
	}
}
