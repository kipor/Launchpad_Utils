using System;
using System.IO;
using Gtk;
using Gdk;

namespace Launchpad.LauncherUtil
{
	public partial class EditProfileDialog : Gtk.Dialog
	{
		private bool bHasLoadedProfile = false;
		private bool bHasNameChanged = false;
		private string OriginalName = "";

		public EditProfileDialog (GameProfile Profile)
		{
			this.Build ();
			OriginalName = Profile.ProfileName;
		}

		public void LoadProfile(GameProfile Profile)
		{
			bHasLoadedProfile = true;
			OriginalName = Profile.ProfileName;

			profileNameEntry.Text = Profile.ProfileName;
			FTPURLEntry.Text = Profile.FTPUrl;
			FTPUsernameEntry.Text = Profile.FTPUsername;
			FTPPasswordEntry.Text = Profile.FTPPassword;

			gameNameEntry.Text = Profile.GameName;
			gamePathChooser.SetCurrentFolder (Profile.LocalGamePath);
			gameVersionEntry.Text = Profile.GameVersion.ToString ();

			// Select "two"
			int row = (int)Profile.SystemTarget;
			Gtk.TreeIter iter;
			systemTargetSelector.Model.IterNthChild (out iter, row);
			systemTargetSelector.SetActiveIter (iter);
			
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{	
			try
			{
				bool bAreAllSettingsOK = true;

				GameProfile Profile = new GameProfile ();

				//PROF
				Profile.ProfileName = profileNameEntry.Text;

				//REMT
				if (!FTPURLEntry.Text.StartsWith("ftp://"))
				{
					FTPURLEntry.ModifyBase (Gtk.StateType.Normal, new Gdk.Color (255, 90, 90));
					FTPURLEntry.TooltipText = "The URL must begin with 'ftp://'. Verify your URL.";
					bAreAllSettingsOK = false;
				}
				else
				{
					FTPURLEntry.ModifyBase (Gtk.StateType.Normal);
					FTPURLEntry.TooltipText = "";
					Profile.FTPUrl = FTPURLEntry.Text;
				}

				Profile.FTPUsername = FTPUsernameEntry.Text;
				Profile.FTPPassword = FTPPasswordEntry.Text;

				//GAME
				Profile.GameName = gameNameEntry.Text;

				if (Directory.Exists(gamePathChooser.Filename) && !String.IsNullOrEmpty(gamePathChooser.Filename))
				{
					Profile.LocalGamePath = gamePathChooser.Filename;
				}
				else
				{
					MessageDialog dialog = new MessageDialog (
						null, DialogFlags.Modal, 
						MessageType.Warning, 
						ButtonsType.Ok, 
						"You must select a directory where your game files are.");

					dialog.Run();
					dialog.Destroy();

					gamePathChooser.TooltipText = "You must select a directory where your game files are.";
					bAreAllSettingsOK = false;
				}


				try
				{
					Version gameVersion = Version.Parse(gameVersionEntry.Text);
					Profile.GameVersion = gameVersion;
				}
				catch (Exception ex)
				{
					gameVersionEntry.ModifyBase(Gtk.StateType.Normal, new Gdk.Color (255, 90, 90));
					gameVersionEntry.TooltipText = ex.Message;
					bAreAllSettingsOK = false;
				}

				Profile.SystemTarget = (ESystemTarget)Enum.Parse (typeof(ESystemTarget), systemTargetSelector.ActiveText);

				if (bAreAllSettingsOK)
				{
					ProfileHandler Profiles = new ProfileHandler ();
					ConfigHandler Config = new ConfigHandler ();
					string profilePath = Config.GetProfilesDir () + Profile.ProfileName + ".lprofile";



					bHasNameChanged = !(String.Equals(Profile.ProfileName, OriginalName));

					if (File.Exists(profilePath) && bHasNameChanged)
					{
						MessageDialog dialog = new MessageDialog (
							null, DialogFlags.Modal, 
							MessageType.Warning, 
							ButtonsType.YesNo, 
							"This profile already exists on disk. Overwrite?");

						dialog.TransientFor = this;

						ResponseType response = (ResponseType)dialog.Run ();
						if (response == ResponseType.Yes)
						{
							Profiles.SaveProfile (Profile, true);
							dialog.Destroy ();
							this.Destroy ();
						}
						else
						{
							dialog.Destroy ();
						}
					}
					else
					{
						Profiles.SaveProfile (Profile, true);
						this.Destroy ();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine ("OKButton(): " + ex.Message);
			}

			//throw new NotImplementedException ();
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			this.Destroy ();
		}
	}
}

