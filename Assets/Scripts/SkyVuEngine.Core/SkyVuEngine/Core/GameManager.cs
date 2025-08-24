using SkyVu.Common.Enums;
using SkyVuEngine.Core.Enums;
using UnityEngine;

namespace SkyVuEngine.Core
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField]
		private int _gameVersion = 0;

		[SerializeField]
		private string _appID = "d975558d-8a41-4a24-a294-30f9dde30e50";

		protected static GameManager _instance = null;

		protected int _editorGamePlatform = -1;

		public GameState GameState { get; set; }

		public GameMode CurrentGameMode { get; set; }

		public GameType CurrentGameType { get; set; }

		public Games Game { get; set; }

		public bool IsMultiplayer { get; set; }

		public bool IsPaused { get; set; }

		public int PatchVersion { get; set; }

		public string DeviceSerial { get; set; }

		public int GamePlatform
		{
			get
			{
				if (_editorGamePlatform != -1)
				{
					return _editorGamePlatform;
				}
				switch (Application.platform)
				{
				case RuntimePlatform.IPhonePlayer:
					return 1;
				case RuntimePlatform.Android:
					return 2;
				case RuntimePlatform.OSXWebPlayer:
				case RuntimePlatform.WindowsWebPlayer:
					return 3;
				case RuntimePlatform.WindowsPlayer:
					return 4;
				case RuntimePlatform.OSXPlayer:
					return 5;
				default:
					return 8;
				}
			}
		}

		public int GameVersion
		{
			get
			{
				return _gameVersion;
			}
		}

		public string AppID
		{
			get
			{
				return _appID;
			}
		}
	}
}
