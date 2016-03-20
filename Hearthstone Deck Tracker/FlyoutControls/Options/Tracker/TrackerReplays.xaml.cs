#region

using System;
using System.Windows;
using System.Windows.Input;
using Hearthstone_Deck_Tracker.Controls.Error;
using Hearthstone_Deck_Tracker.HsReplay;
using Hearthstone_Deck_Tracker.HsReplay.API;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.Logging;
using static System.Windows.Visibility;
using static Hearthstone_Deck_Tracker.HsReplay.Enums.AccountStatus;

#endregion

namespace Hearthstone_Deck_Tracker.FlyoutControls.Options.Tracker
{
	/// <summary>
	/// Interaction logic for TrackerReplays.xaml
	/// </summary>
	public partial class TrackerReplays
	{
		public TrackerReplays()
		{
			InitializeComponent();
		}

		public Visibility TextClaimVisibility => HsReplayUtil.AccountStatus == Unclaimed ? Visible : Collapsed;
		public Visibility AccountNameVisibility => HsReplayUtil.AccountStatus == Unclaimed ? Visible : Collapsed;

		public bool UploadPublic
		{
			get { return Config.Instance.HsReplayUploadPublic; }
			set { UpdatePrivacySetting(value); }
		}

		private async void UpdatePrivacySetting(bool value)
		{
			if(Config.Instance.HsReplayUploadPublic == value)
				return;
			CheckBoxReplayPrivacy.IsEnabled = false;
			Cursor = Cursors.Wait;
			try
			{
				if(await AccountSettings.UpdateReplayPrivacy(value))
				{
					Config.Instance.HsReplayUploadPublic = value;
					Config.Save();
				}
			}
			catch(Exception e)
			{
				Log.Error(e);
				ErrorManager.AddError("Could not update replay privacy setting.", e.ToString());
			}
			finally
			{
				CheckBoxReplayPrivacy.IsEnabled = true;
				Cursor = Cursors.Arrow;
			}
		}

		private void ButtonClaimAccount_OnClick(object sender, RoutedEventArgs e) => AccountSettings.ClaimAccount().Forget();
	}
}