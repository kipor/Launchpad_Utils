
// This file has been generated by the GUI designer. Do not modify.
namespace Launchpad.LauncherUtil
{
	public partial class GameUploadWizard
	{
		private global::Gtk.Alignment pageAlignment;
		private global::Gtk.VBox vbox2;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Launchpad.LauncherUtil.GameUploadWizard
			this.Name = "Launchpad.LauncherUtil.GameUploadWizard";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child Launchpad.LauncherUtil.GameUploadWizard.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.pageAlignment = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.pageAlignment.Name = "pageAlignment";
			// Container child pageAlignment.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.pageAlignment.Add (this.vbox2);
			w1.Add (this.pageAlignment);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(w1 [this.pageAlignment]));
			w3.Position = 0;
			// Internal child Launchpad.LauncherUtil.GameUploadWizard.ActionArea
			global::Gtk.HButtonBox w4 = this.ActionArea;
			w4.Name = "dialog1_ActionArea";
			w4.Spacing = 10;
			w4.BorderWidth = ((uint)(5));
			w4.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w5 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w4 [this.buttonCancel]));
			w5.Expand = false;
			w5.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = global::Mono.Unix.Catalog.GetString ("_Next");
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w6 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w4 [this.buttonOk]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			this.Show ();
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}