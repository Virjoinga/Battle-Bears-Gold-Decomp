using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerPlate : MonoBehaviour
{
	private int _playerID;

	private GameObject _icon;

	[SerializeField]
	private GameObject[] _teamColor;

	[SerializeField]
	private TextBlock _nickName;

	[SerializeField]
	private GameObject _skin;

	[SerializeField]
	private TextMesh _levelClass;

	[SerializeField]
	private TextMesh _playerIdText;

	[SerializeField]
	private GUIButton _kickButton;

	[SerializeField]
	private Transform _iconMount;

	public int PlayerID
	{
		get
		{
			return _playerID;
		}
	}

	public bool KickButtonEnabled
	{
		get
		{
			return _kickButton.enabled;
		}
		set
		{
			_kickButton.gameObject.SetActive(value);
		}
	}

	private void Awake()
	{
		_kickButton.enable();
		for (int i = 0; i < _teamColor.Length; i++)
		{
			_teamColor[i].SetActive(false);
		}
	}

	public void ConstructPlayerPlate(PlayerParameterModel playerData, bool isMaster)
	{
		int playerTeam = playerData.PlayerTeam;
		if (Preferences.Instance.CurrentGameMode != 0)
		{
			_teamColor[playerTeam].SetActive(true);
		}
		_nickName.OnSetText(playerData.SocialName, playerData.ReputationColor);
		string text = ((Preferences.Instance.CurrentGameMode != 0 && playerTeam == 0) ? "_red" : "_blue");
		Object @object = Resources.Load("Icons/Characters/" + playerData.Character + "/" + playerData.Skin + text);
		if (@object != null)
		{
			_icon = Object.Instantiate(@object) as GameObject;
		}
		if (_icon != null && _iconMount != null)
		{
			_icon.transform.parent = _iconMount;
			_icon.transform.localPosition = Vector3.zero;
			_icon.transform.localEulerAngles = Vector3.zero;
			_icon.transform.localScale = Vector3.one;
			_icon.layer = LayerMask.NameToLayer("HUD");
		}
		_levelClass.text = "lvl " + playerData.Level;
		_playerID = playerData.PlayerId;
		_playerIdText.text = _playerID.ToString();
		KickButtonEnabled = isMaster;
	}

	public void SetKickToGrey()
	{
		_kickButton.disable();
	}

	private void OnGUIButtonClicked(GUIButton button)
	{
		if (button.name == "kick")
		{
			SendMessageUpwards("BootPlayer", new Hashtable
			{
				{
					0,
					_nickName.GetComponent<TextMesh>().text
				},
				{ 1, _playerID }
			}, SendMessageOptions.DontRequireReceiver);
		}
	}
}
