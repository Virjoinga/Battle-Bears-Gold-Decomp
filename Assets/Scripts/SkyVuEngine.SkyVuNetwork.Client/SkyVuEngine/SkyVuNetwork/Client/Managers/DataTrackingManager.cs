using System;
using System.Collections.Generic;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class DataTrackingManager
	{
		private GameManager _gameManager;

		private ISkyVuNetworkProxy _proxy;

		private Dictionary<string, string> _funnelData;

		private int _funnelOccurrence;

		public UserEntity CurrentUser { get; set; }

		public DateTime SessionStartTime { get; set; }

		public DataTrackingManager(ISkyVuNetworkProxy proxy, GameManager gameManager)
		{
			_gameManager = gameManager;
			_proxy = proxy;
			_funnelData = new Dictionary<string, string>();
			CurrentUser = UserManager.AnonymousUser;
		}

		public void LogEvent(DataTrackingEventTypes type, string label, Dictionary<string, string> data = null)
		{
			if (CurrentUser.UserId > UserManager.AnonymousUser.UserId)
			{
				if (data == null)
				{
					data = new Dictionary<string, string>();
				}
				_proxy.CallService(Services.DataTracking, new DataTrackingEntity
				{
					Version = _gameManager.GameVersion,
					DeviceSerial = _gameManager.DeviceSerial,
					Catalog = CurrentUser.Catalog,
					UserId = CurrentUser.UserId,
					DataTrackingTypeId = (int)type,
					GameId = (int)_gameManager.Game,
					LabelName = label,
					Data = data
				}, delegate
				{
				}, delegate
				{
				});
			}
		}

		public void RecordFunnelEntry(string label, string data)
		{
			_funnelData.Add(label + _funnelOccurrence++, data);
		}

		public void ClearFunnelData()
		{
			_funnelData.Clear();
			_funnelOccurrence = 0;
		}

		public void SendFunnelData(DataTrackingEventTypes type, string label)
		{
			LogEvent(type, label, _funnelData);
			ClearFunnelData();
		}
	}
}
