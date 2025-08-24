using UnityEngine;
using UnityEngine.UI;

public class ServerMessageController : MonoBehaviour
{
	[SerializeField]
	private Text _text;

	private void Awake()
	{
		string val = string.Empty;
		ServiceManager.Instance.UpdateProperty("main_menu_server_message_bar", ref val);
		if (string.IsNullOrEmpty(val))
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		_text.text = val;
	}
}
