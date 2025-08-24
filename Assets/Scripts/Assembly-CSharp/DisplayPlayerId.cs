using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerId : MonoBehaviour
{
	[SerializeField]
	private Text _playerIdText;

	private void Awake()
	{
		_playerIdText.text = ServiceManager.Instance.GetStats().pid.ToString();
	}
}
