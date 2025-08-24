using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using JsonFx.Json;

namespace Chat
{
	[Serializable]
	public class ChatMessage
	{
		private const string MY_DELIMITER = "~";

		private const string RICH_TEXT_PATTERN = "(?:<(b|i)>)|(?:<\\/(b|i)>)|(?:<size=\\d+?>)|(?:<\\/size>)|(?:<color=\\S+?>)|(?:<\\/color>)|(?:<material=\\d+?>)|(?:<\\/material>)|(?:<quad.*?\\/>)";

		public string Message;

		public string Sender;

		public string Channel;

		public string UniqueId;

		public bool SystemMessage;

		public bool IsAdmin;

		public ChatActions Action = ChatActions.None;

		private Dictionary<string, string> _actionParameters = new Dictionary<string, string>();

		public IDictionary<string, string> ActionParameters
		{
			get
			{
				return new Dictionary<string, string>(_actionParameters);
			}
		}

		public ChatMessage(string message, string sender, string channel, string uniqueId, bool systemMessage = false, bool isAdmin = false)
		{
			Message = ScrubRichText(message);
			Channel = channel;
			Sender = sender;
			UniqueId = uniqueId;
			SystemMessage = systemMessage;
			IsAdmin = isAdmin;
		}

		public ChatMessage(string message)
			: this(message, Bootloader.Instance.socialName, string.Empty, ServiceManager.Instance.GetStats().pid.ToString())
		{
		}

		public ChatMessage(string sender, string channel, MessageData messageData)
			: this(messageData.m, sender, channel, messageData.i, false, messageData.a)
		{
		}

		public static byte[] Serialize(object customObject)
		{
			return Protocol.Serialize(ToString((ChatMessage)customObject));
		}

		public override string ToString()
		{
			return ToString(this);
		}

		private static string ToString(ChatMessage chatMessage)
		{
			string[] value = new string[7]
			{
				chatMessage.Message,
				chatMessage.Sender,
				chatMessage.Channel,
				chatMessage.UniqueId,
				chatMessage.IsAdmin.ToString(),
				chatMessage.Action.ToString(),
				JsonWriter.Serialize(chatMessage.ActionParameters)
			};
			return string.Join("~", value);
		}

		private static ChatMessage StringToChatMessage(string str)
		{
			ChatMessage chatMessage = new ChatMessage(string.Empty);
			chatMessage._actionParameters = JsonReader.Deserialize<Dictionary<string, string>>(EndOfStringParam(str));
			str = str.Remove(str.LastIndexOf("~"));
			chatMessage.Action = (ChatActions)(int)Enum.Parse(typeof(ChatActions), EndOfStringParam(str));
			str = str.Remove(str.LastIndexOf("~"));
			chatMessage.IsAdmin = bool.Parse(EndOfStringParam(str));
			str = str.Remove(str.LastIndexOf("~"));
			chatMessage.UniqueId = EndOfStringParam(str);
			str = str.Remove(str.LastIndexOf("~"));
			chatMessage.Channel = EndOfStringParam(str);
			str = str.Remove(str.LastIndexOf("~"));
			chatMessage.Sender = EndOfStringParam(str);
			str = str.Remove(str.LastIndexOf("~"));
			chatMessage.Message = str;
			return chatMessage;
		}

		private static string EndOfStringParam(string str)
		{
			return str.Substring(str.LastIndexOf("~") + 1);
		}

		public static object Deserialize(byte[] bytes)
		{
			object obj = Protocol.Deserialize(bytes);
			string str = obj as string;
			return StringToChatMessage(str);
		}

		private static string ScrubRichText(string str)
		{
			while (Regex.IsMatch(str, "(?:<(b|i)>)|(?:<\\/(b|i)>)|(?:<size=\\d+?>)|(?:<\\/size>)|(?:<color=\\S+?>)|(?:<\\/color>)|(?:<material=\\d+?>)|(?:<\\/material>)|(?:<quad.*?\\/>)"))
			{
				str = Regex.Replace(str, "(?:<(b|i)>)|(?:<\\/(b|i)>)|(?:<size=\\d+?>)|(?:<\\/size>)|(?:<color=\\S+?>)|(?:<\\/color>)|(?:<material=\\d+?>)|(?:<\\/material>)|(?:<quad.*?\\/>)", string.Empty);
			}
			return str;
		}
	}
}
