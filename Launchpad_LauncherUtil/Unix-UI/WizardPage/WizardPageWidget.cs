using System;

namespace Launchpad.LauncherUtil
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class WizardPageWidget : Gtk.Bin
	{

		public delegate void RequiredPropertyChangedEvent (object sender, RequiredPropertyChangedEventArgs e);

		public event RequiredPropertyChangedEvent RequiredPropertyChanged;

		public RequiredPropertyChangedEventArgs PropertyChangedArgs;

		public WizardPageWidget ()
		{
			this.Build ();
			PropertyChangedArgs = new RequiredPropertyChangedEventArgs ();
		}

		void OnRequiredPropertyChanged()
		{
			if (RequiredPropertyChanged != null)
			{
				//raise the event
				RequiredPropertyChanged (this, PropertyChangedArgs);
			}
		}
	}

	public class RequiredPropertyChangedEventArgs : EventArgs
	{
		public bool bAreAllPropertiesFilled 
		{
			get;
			set;
		}
	}
}

