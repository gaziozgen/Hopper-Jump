namespace FateGames
{
    public static class AnalyticsManager
    {
        public static void Initialize()
        {
            //GameAnalytics.Initialize();
        }

        public static void ReportStartProgress()
        {
            //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level" + PlayerProgression.CurrentLevel);
            TinySauce.OnGameStarted(PlayerProgression.CurrentLevel.ToString());
        }

        public static void ReportFinishProgress(bool success)
        {
            //GameAnalytics.NewProgressionEvent(success ? GAProgressionStatus.Complete : GAProgressionStatus.Fail, "level" + PlayerProgression.CurrentLevel);
            TinySauce.OnGameFinished(success, 100, PlayerProgression.CurrentLevel.ToString());
        }
    }
}

