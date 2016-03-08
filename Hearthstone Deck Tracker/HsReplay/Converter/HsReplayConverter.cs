#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Utility.Logging;
using static Hearthstone_Deck_Tracker.HsReplay.Constants;

#endregion

namespace Hearthstone_Deck_Tracker.HsReplay.Converter
{
	internal class HsReplayConverter
	{
		private static readonly List<string> Converting = new List<string>();
		private static int _fallbackIndex = 1;

		public static async Task<string> Convert(List<string> log, GameStats stats, GameMetaData gameMetaData, bool includeDeck = false)
		{
			var id = gameMetaData?.GameId ?? stats?.GameId.ToString() ?? (_fallbackIndex++).ToString();
			if(Converting.Contains(id))
			{
				Log.Error($"Converting {id} already in progress.");
				return null;
			}
			Converting.Add(id);
			var output = await ConvertInternal(log, stats, gameMetaData, includeDeck);
			Converting.Remove(id);
			return output;
		}

		private static async Task<string> ConvertInternal(List<string> log, GameStats stats, GameMetaData gameMetaData, bool includeDeck)
		{
			Log.Info($"Converting hsreplay, game={{{stats}}}");
			if(!File.Exists(HsReplayExe))
			{
				var setup = await HsReplayManager.Setup();
				if(!setup)
					return null;
			}
			var result = LogValidator.Validate(log);
			if(!result.Valid)
				return null;
			var tmpFile = Helper.GetValidFilePath(TmpDirPath, "tmp", "log");
			try
			{
				using(var sw = new StreamWriter(tmpFile))
				{
					foreach(var line in log)
						sw.WriteLine(line);
				}
			}
			catch(Exception e)
			{
				Log.Error(e);
				return null;
			}
			var output = await RunExeAsync(tmpFile, stats?.StartTime, result.IsPowerTaskList);
			if(string.IsNullOrEmpty(output))
			{
				Log.Error("Converter output is empty.");
				return null;
			}
			output = XmlHelper.AddData(output, gameMetaData, stats, includeDeck);
			try
			{
				File.Delete(tmpFile);
			}
			catch(Exception e)
			{
				Log.Error(e);
			}
			return output;
		}

		private static async Task<string> RunExeAsync(string file, DateTime? time, bool usePowerTaskList)
		{
			try
			{
				return await Task.Run(() => RunExe(file, time, usePowerTaskList));
			}
			catch(Exception e)
			{
				Log.Error(e);
				return null;
			}
		}

		private static string RunExe(string file, DateTime? time, bool usePowerTaskList)
		{
			var dateString = time?.ToString("yyyy-MM-dd");
			var defaultDateArg = time.HasValue ? $"--default-date={dateString} " : "";
			var processorArg = usePowerTaskList ? "--processor=PowerTaskList " : "";
			try
			{
				var procInfo = new ProcessStartInfo
				{
					FileName = HsReplayExe,
					Arguments = defaultDateArg + processorArg + file,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					UseShellExecute = false
				};
				using(var proc = Process.Start(procInfo))
					return proc?.StandardOutput.ReadToEnd();
			}
			catch(Exception e)
			{
				Log.Error(e);
				return null;
			}
		}
	}
}