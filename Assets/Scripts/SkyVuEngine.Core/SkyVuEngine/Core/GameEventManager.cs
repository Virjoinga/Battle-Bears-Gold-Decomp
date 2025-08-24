using System.Collections.Generic;

namespace SkyVuEngine.Core
{
	public class GameEventManager
	{
		protected static GameEventManager _instance = null;

		private List<IGameEvents> _gameEvents = null;

		private void OnAppStart(object[] arg)
		{
		}

		private void OnAppEnd(object[] arg)
		{
		}

		private void OnGameStart(object[] arg)
		{
			_gameEvents.ForEach(delegate(IGameEvents e)
			{
				e.OnGameStart(arg);
			});
		}

		private void OnGameEnd(object[] arg)
		{
			_gameEvents.ForEach(delegate(IGameEvents e)
			{
				e.OnGameEnd(arg);
			});
		}

		private void OnMenuStart(object[] arg)
		{
			_gameEvents.ForEach(delegate(IGameEvents e)
			{
				e.OnMenuStart(arg);
			});
		}

		private void OnMenuPress(object[] arg)
		{
			_gameEvents.ForEach(delegate(IGameEvents e)
			{
				e.OnMenuPress(arg);
			});
		}

		private void OnIapView(object[] arg)
		{
			_gameEvents.ForEach(delegate(IGameEvents e)
			{
				e.OnIapView(arg);
			});
		}

		private void OnIapBought(object[] arg)
		{
			_gameEvents.ForEach(delegate(IGameEvents e)
			{
				e.OnIapBought(arg);
			});
		}

		private void OnPushNotificationsViewed(object[] arg)
		{
			_gameEvents.ForEach(delegate(IGameEvents e)
			{
				e.OnPushNotificationsViewed(arg);
			});
		}

		private void OnNewsfeedViewed(object[] arg)
		{
			_gameEvents.ForEach(delegate(IGameEvents e)
			{
				e.OnNewsfeedViewed(arg);
			});
		}
	}
}
