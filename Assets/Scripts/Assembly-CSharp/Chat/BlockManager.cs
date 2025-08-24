using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;
using UnityEngine;

namespace Chat
{
	public class BlockManager
	{
		private static readonly BlockManager _instance;

		private BlockedUsers _blockedUsers = new BlockedUsers();

		public static BlockManager Instance
		{
			get
			{
				return _instance;
			}
		}

		public IList<BlockedUser> BlockedUsers
		{
			get
			{
				return new List<BlockedUser>(_blockedUsers.Users);
			}
		}

		private BlockManager()
		{
			LoadBlockedUsers();
		}

		static BlockManager()
		{
			_instance = new BlockManager();
		}

		public void BlockUser(BlockedUser user)
		{
			if (!UserIsBlocked(user) && user.Id != ServiceManager.Instance.GetStats().pid.ToString())
			{
				_blockedUsers.Users.Add(user);
				SerializeBlockedUsers();
			}
		}

		public void UnblockUser(BlockedUser blockedUser)
		{
			if (UserIsBlocked(blockedUser))
			{
				_blockedUsers.Users.RemoveAll((BlockedUser u) => u.Id == blockedUser.Id);
				SerializeBlockedUsers();
			}
		}

		public bool UserIsBlocked(BlockedUser user)
		{
			return UserIsBlocked(user.Id);
		}

		public bool UserIsBlocked(string userId)
		{
			return _blockedUsers.Users.Any((BlockedUser u) => u.Id == userId);
		}

		private void LoadBlockedUsers()
		{
			_blockedUsers = new BlockedUsers();
			string @string = PlayerPrefs.GetString("blocked", string.Empty);
			if (!string.IsNullOrEmpty(@string))
			{
				_blockedUsers = JsonReader.Deserialize<BlockedUsers>(@string);
			}
		}

		private void SerializeBlockedUsers()
		{
			PlayerPrefs.SetString("blocked", JsonWriter.Serialize(_blockedUsers));
		}
	}
}
