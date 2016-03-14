#region

using System.Windows;
using Hearthstone_Deck_Tracker.HsReplay;
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
	}
}