using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI;

namespace Chat.UI
{
	public class ChatView : MonoBehaviour
	{
		private const string DISCONNECTED_STRING = "Connecting to chat...";

		private const string CONNECTED_STRING = "Tap to chat";

		private const int NUM_VISIBLE_MESSAGES = 100;

		[SerializeField]
		private ChatMessageView _messagePrefab;

		[SerializeField]
		private Transform _messageParent;

		[SerializeField]
		private UnityEngine.UI.InputField _input;

		[SerializeField]
		private Button _hideButton;

		[SerializeField]
		private Button _blocklistButton;

		[SerializeField]
		private RectTransformAnimatedDrawer _drawer;

		[SerializeField]
		private ChatUserPopup _userPopupPrefab;

		[SerializeField]
		private BlockedUsersPopup _blockedUsersPopupPrefab;

		[SerializeField]
		private ScrollRect _scroll;

		private ChatUserPopup _userPopup;

		private BlockedUsersPopup _blockedUsersPopup;

		private Queue<ChatMessageView> _messageQueue = new Queue<ChatMessageView>();

		private bool _isOpen = true;

		private int _fontSize;

		public PlayerData Me { get; set; }

		public int FontSize
		{
			set
			{
				_fontSize = value;
				Text[] componentsInChildren = GetComponentsInChildren<Text>();
				foreach (Text text in componentsInChildren)
				{
					text.fontSize = _fontSize;
				}
			}
		}

		public MainMenu.Menu[] Menus { get; private set; }

		private event Action<string> _sendMessageRequested;

		public event Action<string> SendMessageRequested
		{
			add
			{
				this._sendMessageRequested = (Action<string>)Delegate.Combine(this._sendMessageRequested, value);
			}
			remove
			{
				this._sendMessageRequested = (Action<string>)Delegate.Remove(this._sendMessageRequested, value);
			}
		}

		private event Action _hideButtonClicked;

		public event Action HideButtonClicked
		{
			add
			{
				this._hideButtonClicked = (Action)Delegate.Combine(this._hideButtonClicked, value);
			}
			remove
			{
				this._hideButtonClicked = (Action)Delegate.Remove(this._hideButtonClicked, value);
			}
		}

		private event Action<BlockedUser> _userBlockRequested;

		public event Action<BlockedUser> UserBlockRequested
		{
			add
			{
				this._userBlockRequested = (Action<BlockedUser>)Delegate.Combine(this._userBlockRequested, value);
			}
			remove
			{
				this._userBlockRequested = (Action<BlockedUser>)Delegate.Remove(this._userBlockRequested, value);
			}
		}

		private event Action<BlockedUser> _unblockUserRequested;

		public event Action<BlockedUser> UnblockUserRequested
		{
			add
			{
				this._unblockUserRequested = (Action<BlockedUser>)Delegate.Combine(this._unblockUserRequested, value);
			}
			remove
			{
				this._unblockUserRequested = (Action<BlockedUser>)Delegate.Remove(this._unblockUserRequested, value);
			}
		}

		private event Action<string, int> _banRequested;

		public event Action<string, int> BanRequested
		{
			add
			{
				this._banRequested = (Action<string, int>)Delegate.Combine(this._banRequested, value);
			}
			remove
			{
				this._banRequested = (Action<string, int>)Delegate.Remove(this._banRequested, value);
			}
		}

		private event Action<string, int> _shadowBanRequested;

		public event Action<string, int> ShadowBanRequested
		{
			add
			{
				this._shadowBanRequested = (Action<string, int>)Delegate.Combine(this._shadowBanRequested, value);
			}
			remove
			{
				this._shadowBanRequested = (Action<string, int>)Delegate.Remove(this._shadowBanRequested, value);
			}
		}

		private event Action<string> _unBanRequested;

		public event Action<string> UnBanRequested
		{
			add
			{
				this._unBanRequested = (Action<string>)Delegate.Combine(this._unBanRequested, value);
			}
			remove
			{
				this._unBanRequested = (Action<string>)Delegate.Remove(this._unBanRequested, value);
			}
		}

		private void RaiseHideButtonClicked()
		{
			if (this._hideButtonClicked != null)
			{
				this._hideButtonClicked();
			}
		}

		protected void RaiseUserBlockRequested(BlockedUser user)
		{
			if (this._userBlockRequested != null)
			{
				this._userBlockRequested(user);
			}
		}

		protected void RaiseUnblockUserRequested(BlockedUser blockedUser)
		{
			if (this._unblockUserRequested != null)
			{
				this._unblockUserRequested(blockedUser);
			}
		}

		protected void RaiseBanRequested(string userId, int banLength)
		{
			if (this._banRequested != null)
			{
				this._banRequested(userId, banLength);
			}
		}

		protected void RaiseShadowBanRequested(string userId, int banLength)
		{
			if (this._shadowBanRequested != null)
			{
				this._shadowBanRequested(userId, banLength);
			}
		}

		protected void RaiseUnBanRequested(string userId)
		{
			if (this._unBanRequested != null)
			{
				this._unBanRequested(userId);
			}
		}

		private void Awake()
		{
			_fontSize = _input.textComponent.fontSize;
			_input.interactable = false;
			(_input.placeholder as Text).text = "Connecting to chat...";
			_input.onEndEdit.AddListener(HandleInput);
			_hideButton.onClick.AddListener(RaiseHideButtonClicked);
			_blocklistButton.onClick.AddListener(ShowBlocklistPopup);
		}

		public void Init(ChatViewOptions options)
		{
			FontSize = options.FontSize;
			Menus = options.Menus;
		}

		public void SetInteractable(bool active, string placeholder = "")
		{
			(_input.placeholder as Text).text = ((!string.IsNullOrEmpty(placeholder)) ? placeholder : ((!active) ? string.Empty : "Tap to chat"));
			_input.interactable = active;
			_blocklistButton.interactable = active;
			if (_blockedUsersPopup != null)
			{
				_blockedUsersPopup.Hide();
			}
			if (_userPopup != null)
			{
				_userPopup.Hide();
			}
			foreach (ChatMessageView item in _messageQueue)
			{
				item.SetInteractable(active);
			}
			_scroll.enabled = active;
		}

		public void SetActive(bool isActive)
		{
			Graphic[] componentsInChildren = GetComponentsInChildren<Graphic>();
			foreach (Graphic graphic in componentsInChildren)
			{
				graphic.enabled = isActive;
			}
			GetComponent<BoxCollider>().enabled = isActive;
		}

		public void ShowMessage(ChatMessage message, bool userIsBlocked)
		{
			ChatMessageView chatMessageView = UnityEngine.Object.Instantiate(_messagePrefab) as ChatMessageView;
			chatMessageView.transform.SetParent(_messageParent);
			chatMessageView.transform.ResetLocally();
			chatMessageView.DisplayMessage(message, _fontSize, Me != null && message.UniqueId == Me.PlayerId);
			chatMessageView.SetInteractable(_input.interactable);
			chatMessageView.ChatPopupRequested += ShowChatUserPopup;
			_messageQueue.Enqueue(chatMessageView);
			chatMessageView.gameObject.SetActive(!userIsBlocked);
			while (_messageQueue.Count >= 100)
			{
				UnityEngine.Object.Destroy(_messageQueue.Dequeue().gameObject);
			}
		}

		public void ToggleShow()
		{
			bool open = !_isOpen;
			SetOpen(open);
		}

		public void SetOpen(bool shouldBeOpen)
		{
			SetHideButtonScale(shouldBeOpen);
			SetViewVisibility(shouldBeOpen);
			_isOpen = shouldBeOpen;
		}

		private void SetHideButtonScale(bool shouldBeOpen)
		{
			Vector3 localScale = _hideButton.transform.localScale;
			if ((shouldBeOpen && localScale.y < 0f) || (!shouldBeOpen && localScale.y > 0f))
			{
				localScale.y *= -1f;
			}
			_hideButton.transform.localScale = localScale;
		}

		private void SetViewVisibility(bool shouldBeOpen)
		{
			if (shouldBeOpen && !_isOpen)
			{
				_drawer.Open();
			}
			else if (!shouldBeOpen && _isOpen)
			{
				_drawer.Close();
			}
		}

		private void HandleInput(string input)
		{
			if (!string.IsNullOrEmpty(input) && this._sendMessageRequested != null)
			{
				this._sendMessageRequested(input);
			}
			_input.text = string.Empty;
		}

		public void SetVisibilityOfMessagesFromUser(string userId, bool shouldBeVisible)
		{
			foreach (ChatMessageView item in _messageQueue)
			{
				if (item.ChatMessage.UniqueId == userId)
				{
					item.gameObject.SetActive(shouldBeVisible);
				}
			}
		}

		private void ShowChatUserPopup(ChatMessage chatMessage)
		{
			if (_userPopup == null)
			{
				_userPopup = UnityEngine.Object.Instantiate(_userPopupPrefab) as ChatUserPopup;
				_userPopup.BlockRequested += RaiseUserBlockRequested;
				_userPopup.BanRequested += RaiseBanRequested;
				_userPopup.ShadowBanRequested += RaiseShadowBanRequested;
				_userPopup.UnBanRequested += RaiseUnBanRequested;
				RectTransform rectTransform = _userPopup.transform as RectTransform;
				rectTransform.SetParent(base.transform);
				rectTransform.ResetLocally();
				Vector2 offsetMin = (rectTransform.offsetMax = Vector2.zero);
				rectTransform.offsetMin = offsetMin;
			}
			_userPopup.Show(chatMessage, Me.IsAdmin);
		}

		private void ShowBlocklistPopup()
		{
			if (_blockedUsersPopup == null)
			{
				_blockedUsersPopup = UnityEngine.Object.Instantiate(_blockedUsersPopupPrefab) as BlockedUsersPopup;
				_blockedUsersPopup.UnblockUserRequested += RaiseUnblockUserRequested;
				RectTransform rectTransform = _blockedUsersPopup.transform as RectTransform;
				rectTransform.SetParent(base.transform);
				rectTransform.ResetLocally();
				Vector2 offsetMin = (rectTransform.offsetMax = Vector2.zero);
				rectTransform.offsetMin = offsetMin;
			}
			_blockedUsersPopup.Show();
		}
	}
}
