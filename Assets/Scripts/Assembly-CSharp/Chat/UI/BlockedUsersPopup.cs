using System;
using UnityEngine;
using Utils.UI;

namespace Chat.UI
{
	public class BlockedUsersPopup : UIPopup
	{
		[SerializeField]
		private BlockedUserView _viewPrefab;

		[SerializeField]
		private Transform _userParent;

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

		protected void RaiseUnblockUserRequested(BlockedUser blockedUser)
		{
			if (this._unblockUserRequested != null)
			{
				this._unblockUserRequested(blockedUser);
			}
		}

		private void OnEnable()
		{
			foreach (BlockedUser blockedUser in BlockManager.Instance.BlockedUsers)
			{
				BlockedUserView blockedUserView = UnityEngine.Object.Instantiate(_viewPrefab) as BlockedUserView;
				blockedUserView.transform.SetParent(_userParent);
				blockedUserView.transform.ResetLocally();
				blockedUserView.Show(blockedUser);
				blockedUserView.UnblockUserRequested += RaiseUnblockUserRequested;
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < _userParent.childCount; i++)
			{
				UnityEngine.Object.Destroy(_userParent.GetChild(i).gameObject);
			}
		}
	}
}
