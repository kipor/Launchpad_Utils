using System;
using System.Collections.Generic;

namespace Launchpad.LauncherUtil
{
	public partial class GameUploadWizard : Gtk.Dialog
	{
		int pagePosition = 0;
		private List<WizardPageWidget> Pages = new List<WizardPageWidget>();

		GameUpload_Metadata metaDataPage;
		GameUploadWidget_Files filesPage;

		public GameUploadWizard ()
		{
			this.Build ();
			metaDataPage = new GameUpload_Metadata ();
			filesPage = new GameUploadWidget_Files ();

			Pages.Add (metaDataPage);
			Pages.Add (filesPage);

			try
			{
				//load the first page
				LoadPage (Pages [0]);
			}
			catch (Exception ex)
			{
				Console.WriteLine ("UploadWizardConstructor(): " + ex.Message);
			}
		}

		private void LoadPage(WizardPageWidget Page)
		{
			//first, clear the vbox
			foreach (Gtk.Widget child in vbox2.Children)
			{
				vbox2.Remove (child);
			}

			vbox2.PackStart (Page, false, false, 0);
			vbox2.ShowAll();

			if (pagePosition + 1 == Pages.Count)
			{
				//we're at the last page, so replace Next with OK
				buttonOk.Label = "OK";	
			}
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			try
			{
				//this is both our OK and Next button
				//set our page position to the next page

				//first, what page were we at? If it's less than the max amount of pages, load the next
				//and increment the page count
				if (pagePosition + 1 < Pages.Count)
				{
					++pagePosition;
					LoadPage(Pages[pagePosition]);
				}
				else if (pagePosition + 1 == Pages.Count)
				{
					this.Destroy();
				}
				//if it was the same, destroy the dialog
			}
			catch (Exception ex)
			{
				Console.WriteLine ("GameUploadWizardButton: " + ex.Message);
			}

			//throw new NotImplementedException ();
		}
	}
}

