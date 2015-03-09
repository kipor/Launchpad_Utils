using System;
using System.IO;
using Gtk;

namespace Launchpad.LauncherUtil
{
	public partial class ProfileListDialog : Gtk.Dialog
	{
		ConfigHandler Config = new ConfigHandler ();

		TreeViewColumn profileNameColumn = new TreeViewColumn ();

		ListStore profileStore = new ListStore (typeof(string));

		CellRendererText profileNameRenderer = new CellRendererText ();

		public ProfileListDialog ()
		{
			this.Build ();
			profileNameColumn.Title = "Profile Name";

			profileNameColumn.PackStart (profileNameRenderer, true);
			profileNameColumn.AddAttribute (profileNameRenderer, "text", 0);

			treeview2.AppendColumn (profileNameColumn);
			treeview2.Model = profileStore;

			UpdateProfiles ();
		}

		private void UpdateProfiles()
		{
			string [] profiles = Directory.GetFiles (Config.GetProfilesDir ());

			profileStore.Clear ();
			foreach (string profile in profiles)
			{
				profileStore.AppendValues (System.IO.Path.GetFileNameWithoutExtension (profile));
			}
		}

		private string GetSelectedProfilePath()
		{
			ConfigHandler Config = new ConfigHandler ();

			TreeModel model = treeview2.Model;
			TreeIter iter;

			treeview2.Selection.GetSelected(out iter);
			string fileName = (string)model.GetValue (iter, 0);
			string profilePath = Config.GetProfilesDir() + fileName + ".lprofile";

			return profilePath;
		}

		protected void OnTreeview2RowActivated (object o, RowActivatedArgs args)
		{
			EditProfileDialog newDialog = new EditProfileDialog (new GameProfile());
			newDialog.Modal = true;
			newDialog.TransientFor = this;

			ProfileHandler Profiles = new ProfileHandler ();

			newDialog.LoadProfile (Profiles.LoadProfile(GetSelectedProfilePath ()));
			newDialog.Run ();

			UpdateProfiles ();
		}

		protected void OnEditButtonClicked (object sender, EventArgs e)
		{
			EditProfileDialog newDialog = new EditProfileDialog (new GameProfile());
			newDialog.Modal = true;
			newDialog.TransientFor = this;

			ProfileHandler Profiles = new ProfileHandler ();

			newDialog.LoadProfile (Profiles.LoadProfile(GetSelectedProfilePath ()));
			newDialog.Run ();

			UpdateProfiles ();
		}

		protected void OnNewButtonClicked (object sender, EventArgs e)
		{
			EditProfileDialog newDialog = new EditProfileDialog (new GameProfile());
			newDialog.Run ();

			UpdateProfiles ();
		}

		protected void OnDeleteButtonClicked (object sender, EventArgs e)
		{
			MessageDialog dialog = new MessageDialog (
				null, DialogFlags.Modal, 
				MessageType.Warning, 
				ButtonsType.YesNo, 
				"Are you certain that you want to delete this profile? It's permanent!");

			ResponseType response = (ResponseType)dialog.Run ();
			if (response == ResponseType.Yes)
			{
				dialog.Destroy ();

				File.Delete(GetSelectedProfilePath ());
				UpdateProfiles ();
			}
			else
			{
				dialog.Destroy ();
			}
		}

		protected void OnButtonOKClicked (object sender, EventArgs e)
		{
			this.Destroy ();
		}
	}
}

