using System;
using Gtk;
using System.Collections.Generic;

namespace Launchpad.LauncherUtil
{
	public sealed partial class GameUploadWizard : Dialog
	{
		int pagePosition;
		List<WizardPageWidget> Pages = new List<WizardPageWidget>();

		GameUpload_Profile profilePage;
		GameUpload_Metadata metaDataPage;
		GameUpload_Files filesPage;
		GameUpload_Upload uploadPage;


		public GameUploadWizard ()
		{
			this.Build ();
			profilePage = new GameUpload_Profile ();
			metaDataPage = new GameUpload_Metadata ();
			filesPage = new GameUpload_Files ();
			uploadPage = new GameUpload_Upload ();

			//the order the pages are added here is the order they will appear in
			//in the wizard
			Pages.Add (profilePage);
			Pages.Add (metaDataPage);
			Pages.Add (filesPage);
			Pages.Add (uploadPage);

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

		void LoadPage(WizardPageWidget Page)
		{
			//first, clear the vbox
			foreach (Widget child in vbox2.Children)
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

			//bind our required property event so that we can prevent progression
			//until we have all the required information.
			Page.RequiredPropertyChanged += OnRequiredPropertyChanged;
		}

		void OnRequiredPropertyChanged(object sender, RequiredPropertyChangedEventArgs e)
		{
			if (e.bAreAllPropertiesFilled)
			{
				buttonOk.Sensitive = true;
			}
			else
			{
				buttonOk.Sensitive = false;
			}
		}

		void OnButtonOkClicked (object sender, EventArgs e)
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
					Destroy();
				}
				//if it was the same, destroy the dialog
			}
			catch (Exception ex)
			{
				Console.WriteLine ("GameUploadWizardButton: " + ex.Message);
			}

			//throw new NotImplementedException ();
		}

		void OnButtonCancelClicked (object sender, EventArgs e)
		{
			if (pagePosition > 0)
			{
				var confirmExitDialog = new MessageDialog (
					null, DialogFlags.Modal, 
					MessageType.Question, 
					ButtonsType.YesNo, 
					"Are you certain you want to quit the wizard?");

				confirmExitDialog.Modal = true;
				confirmExitDialog.TransientFor = this;

				if((ResponseType)confirmExitDialog.Run () == ResponseType.Yes)
				{
					confirmExitDialog.Destroy ();
					Destroy ();
				}
				else
				{
					confirmExitDialog.Destroy ();
				}
			}

		}
	}
}

