using System;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI;

namespace Chat.UI
{
	public class ChatMessageView : MonoBehaviour
	{
		[SerializeField]
		private Text _message;

		[SerializeField]
		private Text _sender;

		[SerializeField]
		private ChatMessageViewOptions _options;

		[SerializeField]
		private Button _chatPopupButton;

		private ChatMessage _chatMessage;

		private bool _isMe;

		public ChatMessage ChatMessage
		{
			get
			{
				return _chatMessage;
			}
		}

		private event Action<ChatMessage> _chatPopupRequested;

		public event Action<ChatMessage> ChatPopupRequested
		{
			add
			{
				this._chatPopupRequested = (Action<ChatMessage>)Delegate.Combine(this._chatPopupRequested, value);
			}
			remove
			{
				this._chatPopupRequested = (Action<ChatMessage>)Delegate.Remove(this._chatPopupRequested, value);
			}
		}

		private void RaiseChatPopupRequested()
		{
			if (this._chatPopupRequested != null)
			{
				this._chatPopupRequested(_chatMessage);
			}
		}

		public void DisplayMessage(ChatMessage chatMessage, int fontSize, bool isMe)
		{
			_isMe = isMe;
			if (!isMe)
			{
				_chatPopupButton.onClick.AddListener(RaiseChatPopupRequested);
			}
			_chatPopupButton.interactable = !isMe;
			_chatMessage = chatMessage;
			_message.fontSize = fontSize;
			_sender.fontSize = fontSize;
			_message.text = FormattedMessage(chatMessage);
			_sender.text = FormattedSender(chatMessage, isMe);
		}

		public void SetInteractable(bool active)
		{
			if (!_isMe)
			{
				_chatPopupButton.interactable = active;
			}
		}

		private string FormattedMessage(ChatMessage chatMessage)
		{
			return chatMessage.Message.Colorize(_options.MessageColor);
		}

		private string FormattedSender(ChatMessage chatMessage, bool isMe)
		{
			return chatMessage.Sender.Colorize(isMe ? _options.MeColor : ((!chatMessage.IsAdmin) ? _options.SenderColor : _options.AdminColor));
		}
	}
}
