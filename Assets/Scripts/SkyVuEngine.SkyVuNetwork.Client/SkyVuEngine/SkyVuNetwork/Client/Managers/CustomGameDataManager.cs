using System;
using System.Collections.Generic;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;
using UnityEngine;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class CustomGameDataManager
	{
		private ISkyVuNetworkProxy _proxy;

		private GameManager _gameManager;

		private CustomGameDataEntity _customGameDataEntity;

		private Dictionary<string, string> _customGameDataDictionary;

		public CustomGameDataManager(ISkyVuNetworkProxy proxy, GameManager gameManager)
		{
			_proxy = proxy;
			_gameManager = gameManager;
			_customGameDataDictionary = new Dictionary<string, string>();
		}

		public void PopulateCustomGameData()
		{
			_proxy.CallService(Services.GetCustomGameData, new CustomGameDataEntity
			{
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				GameId = (int)_gameManager.Game,
				Catalog = UserManager.AnonymousUser.Catalog,
				MethodType = 4
			}, Success, Failure);
		}

		public void AssignCustomGameData(List<string> customGameData)
		{
			_customGameDataEntity = new CustomGameDataEntity
			{
				CustomGameData = customGameData
			};
			CustomGameDataToDictionary();
		}

		public List<string> GetCustomGameData()
		{
			List<string> result = new List<string>();
			if (_customGameDataEntity != null)
			{
				result = _customGameDataEntity.CustomGameData;
			}
			return result;
		}

		public string GetCustomGameData(string label)
		{
			string result = string.Empty;
			if (_customGameDataDictionary.ContainsKey(label))
			{
				result = _customGameDataDictionary[label];
			}
			return result;
		}

		private void Success(string json)
		{
			_customGameDataEntity = _proxy.GetEntity<CustomGameDataEntity>(json);
			CustomGameDataToDictionary();
		}

		private void Failure(string message)
		{
			Debug.Log("failed to get custom game data: " + message);
		}

		private void CustomGameDataToDictionary()
		{
			_customGameDataDictionary = new Dictionary<string, string>();
			foreach (string customGameDatum in _customGameDataEntity.CustomGameData)
			{
				if (!string.IsNullOrEmpty(customGameDatum))
				{
					string[] array = customGameDatum.Split(new string[1] { "::" }, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length == 2)
					{
						_customGameDataDictionary.Add(array[0], array[1]);
					}
				}
			}
		}
	}
}
