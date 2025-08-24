using System;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI;

namespace Chat.UI
{
	public class ChatUserPopup : UIPopup
	{
		private const string EMAIL_RECIPIENT = "support@battlebears.com";

		[SerializeField]
		private Text _sender;

		[SerializeField]
		private Text _message;

		[SerializeField]
		private Button _blockButton;

		[SerializeField]
		private Button _reportButton;

		[SerializeField]
		private Button _banButton;

		[SerializeField]
		private GameObject _banButtonContainer;

		[SerializeField]
		private Button _shadowBanButton;

		[SerializeField]
		private GameObject _shadowBanButtonContainer;

		[SerializeField]
		private Button _unBanButton;

		[SerializeField]
		private GameObject _unBanButtonContainer;

		[SerializeField]
		private UnityEngine.UI.InputField _banLengthInput;

		[SerializeField]
		private GameObject _banLengthContainer;

		private Action<int> _banAction;

		private ChatMessage _chatMessage;

		private string _subject
		{
			get
			{
				return Uri.EscapeDataString("Chat Report - " + _chatMessage.UniqueId);
			}
		}

		private string _body
		{
			get
			{
				return Uri.EscapeDataString("Player " + _chatMessage.UniqueId + " sent the following message that I'm reporting: " + _chatMessage.Message);
			}
		}

		private event Action<BlockedUser> _blockRequested;

		public event Action<BlockedUser> BlockRequested
		{
			add
			{
				this._blockRequested = (Action<BlockedUser>)Delegate.Combine(this._blockRequested, value);
			}
			remove
			{
				this._blockRequested = (Action<BlockedUser>)Delegate.Remove(this._blockRequested, value);
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

		protected void RaiseBlockRequested()
		{
			if (this._blockRequested != null)
			{
				this._blockRequested(new BlockedUser(_chatMessage.UniqueId, _chatMessage.Sender));
			}
		}

		protected void RaiseBanRequested(int banLength)
		{
			if (this._banRequested != null)
			{
				this._banRequested(_chatMessage.UniqueId, banLength);
			}
		}

		protected void RaiseShadowBanRequested(int banLength)
		{
			if (this._shadowBanRequested != null)
			{
				this._shadowBanRequested(_chatMessage.UniqueId, banLength);
			}
		}

		protected void RaiseUnBanRequested()
		{
			if (this._unBanRequested != null)
			{
				this._unBanRequested(_chatMessage.UniqueId);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_blockButton.onClick.AddListener(delegate
			{
				RaiseBlockRequested();
				_blockButton.interactable = false;
			});
			_reportButton.onClick.AddListener(PromptEmailReport);
			_banLengthInput.onEndEdit.AddListener(HandleBanLength);
			_banButton.onClick.AddListener(delegate
			{
				_banAction = RaiseBanRequested;
				_banLengthContainer.SetActive(true);
			});
			_shadowBanButton.onClick.AddListener(delegate
			{
				_banAction = RaiseShadowBanRequested;
				_banLengthContainer.SetActive(true);
			});
			_unBanButton.onClick.AddListener(RaiseUnBanRequested);
		}

		public void Show(ChatMessage chatMessage, bool iAmAdmin)
		{
			_blockButton.interactable = true;
			_banButtonContainer.SetActive(iAmAdmin);
			_shadowBanButtonContainer.SetActive(iAmAdmin);
			_unBanButtonContainer.SetActive(iAmAdmin);
			_banLengthContainer.SetActive(false);
			_banLengthInput.text = string.Empty;
			_chatMessage = chatMessage;
			_sender.text = chatMessage.Sender + " : " + chatMessage.UniqueId;
			_message.text = chatMessage.Message;
			base.Show();
		}

		private void HandleBanLength(string banLength)
		{
			int result = 0;
			if (int.TryParse(banLength, out result))
			{
				_banLengthInput.text = string.Empty;
				_banLengthContainer.SetActive(false);
				if (_banAction != null)
				{
					_banAction(result);
				}
			}
		}

		private void PromptEmailReport()
		{
			Application.OpenURL("mailto:support@battlebears.com?subject=" + _subject + "&body=" + _body);
		}
	}
}
