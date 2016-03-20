using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Newtonsoft.Json;

namespace Hearthstone_Deck_Tracker.HsReplay.API
{
	internal class AccountSettings
	{
		public static async Task<bool> UpdateReplayPrivacy(bool isPublic)
		{
			Log.Info($"Setting replay privacy to public={isPublic}");
			var json = JsonConvert.SerializeObject(new { replays_are_public = isPublic });
			var response = await Web.PutAsync(await ApiManager.GetAccountUrl(), json, ApiManager.ApiKeyHeader);
			Log.Info($"Status={response.StatusCode}");
			return response.StatusCode == HttpStatusCode.OK;
		}

		public static async Task ClaimAccount()
		{
			try
			{
				Log.Info("Opening browser to claim account");
				Process.Start(await ApiManager.GetClaimAccountUrl());
			}
			catch(Exception)
			{
				Log.Error("Could not open browser.");
			}
		}
	}
}
