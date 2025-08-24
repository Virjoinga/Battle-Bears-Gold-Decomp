using System.Collections.Generic;
using System.Linq;
using Chat.Node;
using UnityEngine;

namespace Chat.UI
{
	public class ChatController : MonoBehaviour
	{
		private int _chatMinLevel;

		private string UNDER_CHAT_MIN_MESSAGE;

		[SerializeField]
		private MainMenu _mainMenu;

		[SerializeField]
		private Gearup _gearUp;

		[SerializeField]
		private ChatViewOptions[] _viewOptions;

		[SerializeField]
		private ChatView _viewPrefab;

		[SerializeField]
		private string _serverUrl = "http://10.0.1.121:3000/socket.io/";

		private List<ChatView> _views = new List<ChatView>();

		private ChatClient _chatClient;

		private bool _chatIsOpen = true;

		private bool _loggedIn;

		private bool ChatIsOpen
		{
			get
			{
				return _chatIsOpen;
			}
			set
			{
				_chatIsOpen = value;
				PlayerPrefs.SetInt("chatOpen", _chatIsOpen ? 1 : 0);
			}
		}

		private bool _playerCanChat
		{
			get
			{
				return ServiceManager.Instance.GetStats().level >= (double)_chatMinLevel;
			}
		}

		private void Awake()
		{
			ServiceManager.Instance.UpdateProperty("chat_min_level", ref _chatMinLevel);
			UNDER_CHAT_MIN_MESSAGE = "Chat enabled at level " + _chatMinLevel;
			if (_playerCanChat)
			{
				GUIController.ActiveStateChanged += HandleGUIControllerActiveStateChanged;
				_gearUp.DescriptionPlateVisibilityChanged += HandleDescriptionPlateVisibilityChanged;
			}
			LobbyController.RoomAnnouncementRequested += HandleMessageRequest;
			_mainMenu.BeganMenuSwitch += HandleBeganMenuSwitch;
			_mainMenu.EndedMenuSwitch += HandleEndedMenuSwitch;
			_mainMenu.MatchmakingCompleted += HideAllViews;
			_chatIsOpen = PlayerPrefs.GetInt("chatOpen", 1) == 1;
			SpawnViews(_chatIsOpen);
			StartChat();
			if (!_playerCanChat)
			{
				SetViewInteractability(false, UNDER_CHAT_MIN_MESSAGE);
			}
		}

		private void HandleDescriptionPlateVisibilityChanged(bool isVisible)
		{
			if (_loggedIn)
			{
				SetViewInteractability(!isVisible, string.Empty);
			}
		}

		private void HandleGUIControllerActiveStateChanged(bool isActive, LayerMask controllingMask)
		{
			if (_loggedIn)
			{
				int num = 1 << base.gameObject.layer;
				if ((controllingMask.value & num) == num)
				{
					SetViewInteractability(isActive, string.Empty);
				}
			}
		}

		private void HandleUnblockRequest(BlockedUser user)
		{
			BlockManager.Instance.UnblockUser(user);
			SetMessageVisibilityForUserOnViews(user, true);
		}

		private void HandleBlockRequest(BlockedUser user)
		{
			BlockManager.Instance.BlockUser(user);
			SetMessageVisibilityForUserOnViews(user, false);
		}

		private void SetMessageVisibilityForUserOnViews(BlockedUser user, bool shouldBeVisible)
		{
			foreach (ChatView view in _views)
			{
				view.SetVisibilityOfMessagesFromUser(user.Id, shouldBeVisible);
			}
		}

		private void Start()
		{
			if (PlayerNicknamePopupManager.Instance != null)
			{
				PlayerNicknamePopupManager.Instance.TextFilter = _chatClient;
			}
		}

		private void HideAllViews()
		{
			foreach (ChatView view in _views)
			{
				view.SetActive(false);
			}
		}

		private void HandleBeganMenuSwitch(MainMenu.Menu menu)
		{
			foreach (ChatView view in _views)
			{
				if (!view.Menus.Contains(menu))
				{
					view.SetActive(false);
				}
			}
		}

		private void HandleEndedMenuSwitch(MainMenu.Menu menu)
		{
			foreach (ChatView view in _views)
			{
				if (view.Menus.Contains(menu))
				{
					view.SetActive(true);
				}
			}
		}

		private void ToggleShowForAllViews()
		{
			foreach (ChatView view in _views)
			{
				view.ToggleShow();
			}
			ChatIsOpen = !ChatIsOpen;
		}

		private void SpawnViews(bool shouldBeOpen)
		{
			ChatViewOptions[] viewOptions = _viewOptions;
			foreach (ChatViewOptions chatViewOptions in viewOptions)
			{
				ChatView chatView = Object.Instantiate(_viewPrefab) as ChatView;
				chatView.transform.SetParent(chatViewOptions.transform);
				chatView.transform.ResetLocally();
				chatView.Init(chatViewOptions);
				chatView.SendMessageRequested += HandleMessageRequest;
				chatView.HideButtonClicked += ToggleShowForAllViews;
				chatView.UserBlockRequested += HandleBlockRequest;
				chatView.UnblockUserRequested += HandleUnblockRequest;
				chatView.BanRequested += HandleBanRequest;
				chatView.ShadowBanRequested += HandleShadowBanRequest;
				chatView.UnBanRequested += HandleUnBanRequest;
				RectTransform rectTransform = chatView.transform as RectTransform;
				Vector2 offsetMax = (rectTransform.offsetMin = Vector2.zero);
				rectTransform.offsetMax = offsetMax;
				chatView.SetOpen(shouldBeOpen);
				chatView.SetInteractable(false, string.Empty);
				_views.Add(chatView);
			}
		}

		private void HandleBanRequest(string userId, int banLength)
		{
			_chatClient.Ban(userId, banLength);
		}

		private void HandleShadowBanRequest(string userId, int banLength)
		{
			_chatClient.ShadowBan(userId, banLength);
		}

		private void HandleUnBanRequest(string userId)
		{
			_chatClient.UnBan(userId);
		}

		private void StartChat()
		{
			_chatClient = new ChatClient(_serverUrl);
			_chatClient.Connected += AuthenticateChatClient;
			_chatClient.ReceivedPublicMessage += ShowMessageOnViews;
			_chatClient.LoggedIn += HandleLogin;
			_chatClient.Disconnected += HandleDisconnect;
		}

		private void HandleMessageRequest(string message)
		{
			_chatClient.PublishMessage(new ChatMessage(message));
		}

		private void ShowMessageOnViews(ChatMessage chatMessage)
		{
			foreach (ChatView view in _views)
			{
				view.ShowMessage(chatMessage, BlockManager.Instance.UserIsBlocked(chatMessage.UniqueId));
			}
		}

		private void HandleLogin(PlayerData playerData)
		{
			_loggedIn = true;
			foreach (ChatView view in _views)
			{
				view.Me = playerData;
			}
			if (_playerCanChat)
			{
				SetViewInteractability(true, string.Empty);
			}
		}

		private void HandleDisconnect()
		{
			_loggedIn = false;
			SetViewInteractability(false, string.Empty);
		}

		private void SetViewInteractability(bool isActive, string placeholder = "")
		{
			foreach (ChatView view in _views)
			{
				view.SetInteractable(isActive, placeholder);
			}
		}

		private void AuthenticateChatClient()
		{
			_chatClient.Authenticate(Bootloader.Instance.socialName, ServiceManager.Instance.GetStats().pid.ToString());
		}

		private void OnDestroy()
		{
			GUIController.ActiveStateChanged -= HandleGUIControllerActiveStateChanged;
			if (_mainMenu != null)
			{
				_mainMenu.BeganMenuSwitch -= HandleBeganMenuSwitch;
				_mainMenu.EndedMenuSwitch -= HandleEndedMenuSwitch;
				_mainMenu.MatchmakingCompleted -= HideAllViews;
			}
			if (_gearUp != null)
			{
				_gearUp.DescriptionPlateVisibilityChanged -= HandleDescriptionPlateVisibilityChanged;
			}
			if (_chatClient != null)
			{
				_chatClient.Connected -= AuthenticateChatClient;
				_chatClient.ReceivedPublicMessage -= ShowMessageOnViews;
				_chatClient.LoggedIn -= HandleLogin;
				_chatClient.Disconnected -= HandleDisconnect;
				_chatClient.Disconnect();
			}
		}
	}
}
