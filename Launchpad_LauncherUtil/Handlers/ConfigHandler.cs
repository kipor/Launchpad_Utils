using System;
using System.IO;

namespace Launchpad.LauncherUtil
{
	public sealed class ConfigHandler
	{
		public ConfigHandler ()
		{
		}

		public string GetProfilesDir()
		{
			return String.Format ("{0}{1}Profiles{1}", 
			                     Directory.GetCurrentDirectory (),
			                     Path.DirectorySeparatorChar);
		}
	}
}

