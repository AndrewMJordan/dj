using System;

namespace Andtech.DJ
{

	public static class Log
	{
		public static Verbosity Verbosity { get; set; } = Verbosity.Default;

		public static void WriteLine(object message, Verbosity verbosity = Verbosity.Default)
		{
			if ((int)verbosity <= (int)Verbosity)
			{
				Console.WriteLine(message);
			}
		}

		public static void WriteLine(object message, ConsoleColor foregroundColor, Verbosity verbosity = Verbosity.Default)
		{
			if ((int)verbosity <= (int)Verbosity)
			{
				var temp = Console.ForegroundColor;
				Console.ForegroundColor = foregroundColor;
				Console.WriteLine(message);
				Console.ForegroundColor = temp;
			}
		}
	}
}
