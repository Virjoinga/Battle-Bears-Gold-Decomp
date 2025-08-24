using System;
using System.Collections.Generic;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class MessageManager
	{
		private ISkyVuNetworkProxy _proxy;

		private GameManager _gameManager;

		private List<MessageEntity> _messageEntities;

		public List<MessageEntity> Messages
		{
			get
			{
				return _messageEntities;
			}
		}

		public MessageManager(ISkyVuNetworkProxy proxy, GameManager gameManager)
		{
			_proxy = proxy;
			_gameManager = gameManager;
			_messageEntities = new List<MessageEntity>();
		}

		public void GetUserMessages(UserEntity user, Action<List<MessageEntity>> success = null, Action<string> failure = null)
		{
			_proxy.CallService(Services.GetMessages, new MessageEntity
			{
				MethodType = 4,
				Catalog = user.Catalog,
				UserId = user.UserId,
				Version = _gameManager.GameVersion,
				DeviceSerial = _gameManager.DeviceSerial,
				To = 1,
				From = 1
			}, delegate(string messageList)
			{
				_messageEntities = _proxy.GetEntities<MessageEntity>(messageList);
				if (_messageEntities == null)
				{
					_messageEntities = new List<MessageEntity>();
				}
				if (success != null)
				{
					success(_messageEntities);
				}
			}, failure);
		}

		public void SetReadMessage(MessageEntity message, UserEntity currentUser, Action<string> success = null, Action<string> failure = null)
		{
			message.MethodType = 2;
			message.MessageStatus = 2;
			message.DeviceSerial = _gameManager.DeviceSerial;
			message.Catalog = currentUser.Catalog;
			message.UserId = currentUser.UserId;
			_proxy.CallService(Services.SetMessageRead, message, delegate(string obj)
			{
				if (success != null)
				{
					success(obj);
				}
			}, delegate(string errorMessage)
			{
				if (failure != null)
				{
					failure(errorMessage);
				}
			});
		}

		public void DeleteMessage(MessageEntity message, string catalog, Action<string> success, Action<string> failure = null)
		{
			message.MethodType = 3;
			message.DeviceSerial = _gameManager.DeviceSerial;
			message.UserId = message.To;
			message.Catalog = catalog;
			_proxy.CallService(Services.DeleteMessage, message, success, failure);
		}

		public void SendMessage(UserEntity from, int to, string message, string subject, MessageTypes messageType, Action<string> success, Action<string> failure = null)
		{
			_proxy.CallService(Services.CreateMessage, new MessageEntity
			{
				MethodType = 1,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				MessageType = (int)messageType,
				MessageStatus = 1,
				Catalog = from.Catalog,
				UserId = from.UserId,
				To = to,
				From = from.UserId,
				Message = message,
				Subject = subject,
				FromGamerTag = from.GamerTag
			}, success, failure);
		}

		public void SendMessage(UserEntity from, int to, string message, string subject, Action<string> success, Action<string> failure = null)
		{
			SendMessage(from, to, message, subject, MessageTypes.Personal, success, failure);
		}

		public void BanMessages(UserEntity you, int otherId, Action<string> success = null, Action<string> failure = null)
		{
			_proxy.CallService(Services.CreateMessageBan, new MessageEntity
			{
				MethodType = 1,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				From = otherId,
				To = you.UserId,
				UserId = you.UserId,
				Catalog = you.Catalog
			}, success, failure);
		}

		public void AllowMessages(UserEntity you, int otherId, Action<string> success = null, Action<string> failure = null)
		{
			_proxy.CallService(Services.DeleteMessageBan, new MessageEntity
			{
				MethodType = 3,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				From = otherId,
				To = you.UserId,
				UserId = you.UserId,
				Catalog = you.Catalog
			}, success, failure);
		}

		public void GetBanStatus(UserEntity you, Action<List<int>> success = null, Action<string> failure = null)
		{
			_proxy.CallService(Services.GetMessageBans, new MessageEntity
			{
				MethodType = 4,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				From = you.UserId,
				To = you.UserId,
				UserId = you.UserId,
				Catalog = you.Catalog
			}, delegate(string json)
			{
				List<MessageEntity> entities = _proxy.GetEntities<MessageEntity>(json);
				List<int> list = new List<int>();
				if (entities != null)
				{
					foreach (MessageEntity item in entities)
					{
						list.Add(item.From);
					}
				}
				if (success != null)
				{
					success(list);
				}
			}, failure);
		}

		public void SendGameMessage(UserEntity user, string subject, string body, Action<string> success = null, Action<string> failure = null)
		{
			_proxy.CallService(Services.SendGameMessage, new GameMessageEntity
			{
				UserId = user.UserId,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				GameId = (int)_gameManager.Game,
				Subject = subject,
				Body = body,
				To = "SkyVu",
				From = user.GamerTag,
				Catalog = user.Catalog
			}, success, failure);
		}
	}
}
