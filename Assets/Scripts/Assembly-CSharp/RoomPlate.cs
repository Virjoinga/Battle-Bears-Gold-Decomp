using UnityEngine;

public class RoomPlate : MonoBehaviour
{
	private RoomInfo _room;

	[SerializeField]
	private TextBlock _name;

	[SerializeField]
	private TextMesh _players;

	[SerializeField]
	private GameObject _passwordProtected;

	[SerializeField]
	private GUIButton _selected;

	[SerializeField]
	private Material _selectedMaterial;

	[SerializeField]
	private Material _unselectedMaterial;

	public RoomInfo Room
	{
		get
		{
			return _room;
		}
	}

	public bool PasswordProtected
	{
		get
		{
			return _passwordProtected.activeSelf;
		}
	}

	public void ConstructRoomPlate(RoomInfo room)
	{
		_room = room;
		_name.OnSetText(room.name, string.Empty);
		_players.text = room.playerCount + "/" + room.maxPlayers;
		if (room.customProperties.ContainsKey("password"))
		{
			if (!string.IsNullOrEmpty((string)room.customProperties["password"]))
			{
				_passwordProtected.SetActive(true);
			}
			else
			{
				_passwordProtected.SetActive(false);
			}
		}
		else
		{
			_passwordProtected.SetActive(false);
		}
		SetSelection(false);
	}

	public void SetSelection(bool isSelected)
	{
		if (isSelected)
		{
			_name.GetComponent<Renderer>().material = _selectedMaterial;
			_players.GetComponent<Renderer>().material = _selectedMaterial;
			_selected.enable();
		}
		else
		{
			_name.GetComponent<Renderer>().material = _unselectedMaterial;
			_players.GetComponent<Renderer>().material = _unselectedMaterial;
			_selected.disable();
		}
	}
}
