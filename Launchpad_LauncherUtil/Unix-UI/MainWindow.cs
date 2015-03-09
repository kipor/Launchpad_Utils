using System;
using System.IO;
using Gtk;

namespace Launchpad.LauncherUtil
{
	public partial class MainWindow : Gtk.Window
	{
		public MainWindow () : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected void OnButtonGameUploadWizardClicked (object sender, EventArgs e)
		{
			GameUploadWizard uploadWizard = new GameUploadWizard ();
			uploadWizard.Run ();
		}

		protected void OnAddProfileButtonClicked (object sender, EventArgs e)
		{

		}

		protected void OnEditGameProfileButtonClicked (object sender, EventArgs e)
		{
			ProfileListDialog profileList = new ProfileListDialog ();
			profileList.Modal = true;
			profileList.TransientFor = this;
			profileList.Run ();
		}


		protected void OnImportProfileActionActivated (object sender, EventArgs e)
		{
			//First, select and copy
			FileChooserDialog dialog = new FileChooserDialog ("Choose a profile", 
			                                                  this, 
			                                                  FileChooserAction.Open,
			                                                  "Cancel", ResponseType.Cancel,
			                                                  "Import", ResponseType.Accept);

			FileFilter filter = new FileFilter ();
			filter.AddPattern ("*.lprofile");

			dialog.Filter = filter;
			dialog.SetCurrentFolder (Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

			if ((ResponseType)dialog.Run() == ResponseType.Accept)
			{
				try
				{
					ConfigHandler Config = new ConfigHandler();

					string copyTarget = Config.GetProfilesDir() +
						System.IO.Path.GetFileName(dialog.Filename);

					if(File.Exists(copyTarget))
					{
						MessageDialog completedDialog = new MessageDialog (
							null, DialogFlags.Modal, 
							MessageType.Info, 
							ButtonsType.YesNo, 
							"The selected profile already exists (or one with the same name). Overwrite?");

						completedDialog.Modal = true;
						completedDialog.TransientFor = this;

						if((ResponseType)completedDialog.Run () == ResponseType.Yes)
						{
							File.Copy(dialog.Filename, copyTarget, true);
							completedDialog.Destroy ();
						}
						else
						{
							completedDialog.Destroy();
						}
					}
					else
					{
						File.Copy(dialog.Filename, copyTarget);
					}
					dialog.Destroy();

					//then show a list
					ProfileListDialog profileList = new ProfileListDialog ();
					profileList.Modal = true;
					profileList.TransientFor = this;
					profileList.Run ();
				}
				catch (Exception ex)
				{
					dialog.Destroy ();
					Console.WriteLine ("ImportProfile(): " + ex.Message);
				}
			}
			else
			{
				dialog.Destroy ();
			}
		}

		protected void OnExportProfileActionActivated (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnCreateNewProfileActionActivated (object sender, EventArgs e)
		{
			EditProfileDialog editProfile = new EditProfileDialog (new GameProfile());
			editProfile.Modal = true;
			editProfile.TransientFor = this;
			editProfile.Run ();
		}

		protected void OnEditProfileActionActivated (object sender, EventArgs e)
		{
			ConfigHandler Config = new ConfigHandler ();
			FileChooserDialog dialog = new FileChooserDialog ("Choose a profile", 
			                                                 this, 
			                                                 FileChooserAction.Open,
			                                                 "Cancel", ResponseType.Cancel,
			                                                 "Open", ResponseType.Accept);

			FileFilter filter = new FileFilter ();
			filter.AddPattern ("*.lprofile");

			dialog.Filter = filter;
			dialog.SetCurrentFolder (Config.GetProfilesDir ());
			dialog.Modal = true;
			dialog.TransientFor = this;


			if ((ResponseType)dialog.Run() == ResponseType.Accept)
			{
				string filePath = dialog.Filename;
				dialog.Destroy ();

				ProfileHandler Profiles = new ProfileHandler ();
				EditProfileDialog editProfile = new EditProfileDialog (new GameProfile());
				editProfile.LoadProfile (Profiles.LoadProfile (filePath));

				editProfile.Run ();
			}
			else
			{
				dialog.Destroy ();
			}
		}

		protected void OnViewProfilesAction1Changed (object o, ChangedArgs args)
		{
			ProfileListDialog profileList = new ProfileListDialog ();
			profileList.Modal = true;
			profileList.TransientFor = this;
			profileList.Run ();
		}

		protected void OnGenerateManifestActionActivated (object sender, EventArgs e)
		{
			FileChooserDialog dialog = new FileChooserDialog ("Select a folder", 
			                                                  this, 
			                                                  FileChooserAction.SelectFolder,
			                                                  "Cancel", ResponseType.Cancel,
			                                                  "Select", ResponseType.Accept);

			dialog.Modal = true;
			dialog.TransientFor = this;
			if ((ResponseType)dialog.Run () == ResponseType.Accept)
			{
				ManifestHandler Manifest = new ManifestHandler ();
				string folderPath = dialog.Filename;
				dialog.Destroy ();

				MessageDialog infoDialog = new MessageDialog (
					null, DialogFlags.Modal, 
					MessageType.Info, 
					ButtonsType.None, 
					"Generating...");

				infoDialog.Modal = true;
				infoDialog.TransientFor = this;	

				Manifest.ManifestGenerationFinished += delegate(object EventSender, GenerationFinishedEventArgs EventE)
				{
					infoDialog.Destroy();
				};

				Manifest.GenerateManifest (folderPath + System.IO.Path.DirectorySeparatorChar);								

				infoDialog.Run ();
			}
		}
		protected void OnViewProfilesAction1Activated (object sender, EventArgs e)
		{
			ProfileListDialog profileList = new ProfileListDialog ();
			profileList.Modal = true;
			profileList.TransientFor = this;
			profileList.Run ();
		}
		protected void OnGenerateFileMD5ActionActivated (object sender, EventArgs e)
		{
			FileChooserDialog dialog = new FileChooserDialog ("Select a file", 
			                                                  this, 
			                                                  FileChooserAction.Open,
			                                                  "Cancel", ResponseType.Cancel,
			                                                  "Select", ResponseType.Accept);

			dialog.Modal = true;
			dialog.TransientFor = this;
			if ((ResponseType)dialog.Run () == ResponseType.Accept)
			{
				ManifestHandler Manifest = new ManifestHandler ();
				string filePath = dialog.Filename;
				dialog.Destroy ();

				Stream fileStream = File.OpenRead (filePath);
				string hash = Manifest.GetFileHash (fileStream);
				Console.WriteLine (hash);
				fileStream.Close ();

				MessageDialog completedDialog = new MessageDialog (
					null, DialogFlags.Modal, 
					MessageType.Info, 
					ButtonsType.Ok, 
					"MD5: " + hash);

				completedDialog.Modal = true;
				completedDialog.TransientFor = this;

				if((ResponseType)completedDialog.Run () == ResponseType.Ok)
				{
					completedDialog.Destroy ();
				}
			}
		}
	}
}

