using System;
using System.IO;
using System.Text;

namespace Launchpad.LauncherUtil
{
	public sealed class ProfileHandler
	{
		public ProfileHandler ()
		{
		}

		public void SaveProfile(GameProfile Profile, bool bShouldOverwrite)
		{
			ConfigHandler Config = new ConfigHandler ();
			string profilePath = Config.GetProfilesDir () + Profile.ProfileName + ".lprofile";
			Directory.CreateDirectory (Path.GetDirectoryName (profilePath));

			if (!File.Exists(profilePath) || bShouldOverwrite)
			{
				FileStream newProfile = File.Create (profilePath);
				using (BinaryWriter bw = new BinaryWriter(newProfile, Encoding.UTF8))
				{
					//write profile file identifier
					bw.Write (Encoding.UTF8.GetBytes("LPAD"));
					//write profile chunk
					//all values are four bytes with the data size, followed by the data
					bw.Write (Encoding.UTF8.GetBytes("PROF"));
					bw.Write (Encoding.UTF8.GetByteCount (Profile.ProfileName));
					bw.Write (Encoding.UTF8.GetBytes(Profile.ProfileName));

					//write remote connection chunk
					bw.Write (Encoding.UTF8.GetBytes("REMT"));
					bw.Write (Encoding.UTF8.GetByteCount (Profile.FTPUrl));
					bw.Write (Encoding.UTF8.GetBytes(Profile.FTPUrl));
					bw.Write (Encoding.UTF8.GetByteCount(Profile.FTPUsername));
					bw.Write (Encoding.UTF8.GetBytes(Profile.FTPUsername));
					bw.Write (Encoding.UTF8.GetByteCount (Profile.FTPPassword));
					bw.Write (Encoding.UTF8.GetBytes(Profile.FTPPassword));

					//write game information chunk
					bw.Write (Encoding.UTF8.GetBytes("GAME"));
					bw.Write (Encoding.UTF8.GetByteCount (Profile.GameName));
					bw.Write (Encoding.UTF8.GetBytes(Profile.GameName));
					bw.Write (Encoding.UTF8.GetByteCount (Profile.LocalGamePath));
					bw.Write (Encoding.UTF8.GetBytes(Profile.LocalGamePath));
					bw.Write (Encoding.UTF8.GetByteCount (Profile.GameVersion.ToString()));
					bw.Write (Encoding.UTF8.GetBytes(Profile.GameVersion.ToString ()));
					bw.Write (Encoding.UTF8.GetByteCount (Profile.SystemTarget.ToString ()));
					bw.Write (Encoding.UTF8.GetBytes (Profile.SystemTarget.ToString ()));

					//finished
					bw.Close ();
				}				
			}
		}

		public GameProfile LoadProfile(string filePath)
		{
			GameProfile loadProfile = new GameProfile ();

			if (File.Exists(filePath))
			{
				try
				{
					FileStream profileFile = File.OpenRead (filePath);
					BinaryReader br = new BinaryReader(profileFile);
					
					string identifier = Encoding.UTF8.GetString(br.ReadBytes(4));
					if (identifier == "LPAD")
					{
						//read the profile chunk
						string profileHeader = Encoding.UTF8.GetString(br.ReadBytes(4));
						if (profileHeader == "PROF")
						{
							int profileNameSize = br.ReadInt32();
							string profileName = Encoding.UTF8.GetString(br.ReadBytes(profileNameSize));

							loadProfile.ProfileName = profileName;
						}
						else
						{
							throw new InvalidProfileException();
						}

						//read the remote chunk
						string remoteHeader = Encoding.UTF8.GetString(br.ReadBytes(4));
						if (remoteHeader == "REMT")
						{
							//read URL
							int urlSize = br.ReadInt32();
							string url = Encoding.UTF8.GetString(br.ReadBytes(urlSize));

							loadProfile.FTPUrl = url;

							//read username
							int usernameSize = br.ReadInt32();
							string username = Encoding.UTF8.GetString(br.ReadBytes(usernameSize));

							loadProfile.FTPUsername = username;

							//read password
							int passwordSize = br.ReadInt32();
							string password = Encoding.UTF8.GetString(br.ReadBytes(passwordSize));

							loadProfile.FTPPassword = password;
						}
						else
						{
							throw new InvalidProfileException();
						}

						//read the game chunk
						string gameHeader = Encoding.UTF8.GetString(br.ReadBytes(4));
						if (gameHeader == "GAME")
						{
							//read game name
							int gameNameSize = br.ReadInt32();
							string gameName = Encoding.UTF8.GetString(br.ReadBytes(gameNameSize));

							loadProfile.GameName = gameName;

							//read game path
							int gamePathSize = br.ReadInt32();
							string gamePath = Encoding.UTF8.GetString(br.ReadBytes(gamePathSize));

							loadProfile.LocalGamePath = gamePath;

							//read game version
							int gameVersionSize = br.ReadInt32();
							Version gameVersion = new Version (Encoding.UTF8.GetString(br.ReadBytes(gameVersionSize)));

							loadProfile.GameVersion = gameVersion;

							//read game system target
							int systemTargetSize = br.ReadInt32();
							ESystemTarget systemTarget = (ESystemTarget)Enum.Parse(typeof(ESystemTarget), 
							                                                       Encoding.UTF8.GetString(br.ReadBytes(systemTargetSize)), 
							                                                       false);

							loadProfile.SystemTarget = systemTarget;
						}
						else
						{
							throw new InvalidProfileException();
						}
					}
					else
					{
						throw new InvalidProfileException();
					}

					br.Close();
					profileFile.Close();

					return loadProfile;
				}
				catch (Exception ex)
				{
					Console.WriteLine ("LoadProfile(): " + ex.Message);
					return loadProfile;
				}
			}
			else
			{
				return null;
			}
		}

		public string GetProfileDir()
		{
			string path = String.Format ("{0}{1}Profiles{1}", 
			                     Directory.GetCurrentDirectory(),
			                     Path.DirectorySeparatorChar);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory (path);
			}

			return path;
		}
	}

	public sealed class GameProfile
	{
		public string ProfileName;

		public string FTPUrl;
		public string FTPUsername;
		public string FTPPassword;

		public string GameName;
		public string LocalGamePath;
		public Version GameVersion;
		public ESystemTarget SystemTarget;

		public GameProfile(string ProfileName, string FTPUrl, string FTPUsername, string FTPPassword, string GameName, string LocalGamePath, Version GameVersion, ESystemTarget SystemTarget)
		{
			this.ProfileName = ProfileName;

			this.FTPUrl = FTPUrl;
			this.FTPUsername = FTPUsername;
			this.FTPPassword = FTPPassword;

			this.GameName = GameName;
			this.LocalGamePath = LocalGamePath;
			this.GameVersion = GameVersion;
			this.SystemTarget = SystemTarget;
		}

		public GameProfile()
		{
			this.ProfileName = "";

			this.FTPUrl = "";
			this.FTPUsername = "";
			this.FTPPassword = "";

			this.GameName = "";
			this.LocalGamePath = "";
			this.GameVersion = Version.Parse("0.0.0");
			this.SystemTarget = ESystemTarget.Linux;
		}
	}

	public enum ESystemTarget
	{
		Linux,
		Mac, 
		Win32,
		Win64
	}

	public class InvalidProfileException : Exception
	{
		public InvalidProfileException()
		{

		}
	}
}

