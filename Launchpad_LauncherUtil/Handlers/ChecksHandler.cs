using System;

namespace Launchpad.LauncherUtil
{
	public sealed class ChecksHandler
	{
		public ChecksHandler ()
		{
		}

		/// <summary>
		/// Determines whether this instance is running on unix.
		/// </summary>
		/// <returns><c>true</c> if this instance is running on unix; otherwise, <c>false</c>.</returns>
		public bool IsRunningOnUnix()
		{
			int p = (int)Environment.OSVersion.Platform;
			if ((p == 4) || (p == 6) || (p == 128))
			{
				Console.WriteLine("Running on Unix");
				return true;
			}
			else
			{
				Console.WriteLine("Not running on Unix");
				return false;
			}
		}
	}
}

