using UnityEngine;

public class CharacterSkinTest : MonoBehaviour
{
	[SerializeField]
	private CharacterSkin _skin;

	[SerializeField]
	private GameObject _target;

	private void Start()
	{
		_skin.AddTo(_target.GetComponent<CharacterHandle>());
	}
}
