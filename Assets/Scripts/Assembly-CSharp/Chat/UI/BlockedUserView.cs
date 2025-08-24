using System;
using UnityEngine;
using UnityEngine.UI;

namespace Chat.UI
{
	public class BlockedUserView : MonoBehaviour
	{
		[SerializeField]
		private Text _idText;

		[SerializeField]
		private Text _nameText;

		[SerializeField]
		private Button _unblockButton;

		private BlockedUser _blockedUser;

		private event Action<BlockedUser> _unBlockUserRequested;

		public event Action<BlockedUser> UnblockUserRequested
		{
			add
			{
				this._unBlockUserRequested = (Action<BlockedUser>)Delegate.Combine(this._unBlockUserRequested, value);
			}
			remove
			{
				this._unBlockUserRequested = (Action<BlockedUser>)Delegate.Remove(this._unBlockUserRequested, value);
			}
		}

		protected void RaiseUnblockUserRequested()
		{
			if (this._unBlockUserRequested != null)
			{
				this._unBlockUserRequested(_blockedUser);
			}
		}

		private void Awake()
		{
			_unblockButton.onClick.AddListener(delegate
			{
				RaiseUnblockUserRequested();
				UnityEngine.Object.Destroy(base.gameObject);
			});
		}

		public void Show(BlockedUser blockedUser)
		{
			_blockedUser = blockedUser;
			_idText.text = blockedUser.Id;
			_nameText.text = blockedUser.Name;
		}
	}
}
