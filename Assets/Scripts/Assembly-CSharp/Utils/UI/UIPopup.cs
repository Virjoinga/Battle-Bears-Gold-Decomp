using UnityEngine;
using UnityEngine.UI;

namespace Utils.UI
{
	public abstract class UIPopup : MonoBehaviour
	{
		[SerializeField]
		protected Button _closeButton;

		protected virtual void Awake()
		{
			_closeButton.onClick.AddListener(Hide);
		}

		public virtual void Show()
		{
			base.gameObject.SetActive(true);
		}

		public virtual void Hide()
		{
			base.gameObject.SetActive(false);
		}
	}
}
