namespace SkyVuEngine.Core
{
	public interface IGameEvents
	{
		void OnGameStart(object[] arg);

		void OnGameEnd(object[] arg);

		void OnMenuStart(object[] arg);

		void OnMenuPress(object[] arg);

		void OnSinglePlayerGameStart(object[] arg);

		void OnSinglePlayerGameEnd(object[] arg);

		void OnCoopGameStart(object[] arg);

		void OnCoopGameEnd(object[] arg);

		void OnDeathMatchStart(object[] arg);

		void OnDeathMatchEnd(object[] arg);

		void OnCtfStart(object[] arg);

		void OnCtfEnd(object[] arg);

		void OnFfaStart(object[] arg);

		void OnFfaEnd(object[] arg);

		void OnIapView(object[] arg);

		void OnIapBought(object[] arg);

		void OnPushNotificationsViewed(object[] arg);

		void OnNewsfeedViewed(object[] arg);
	}
}
