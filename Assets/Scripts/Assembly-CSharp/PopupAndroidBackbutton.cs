using UnityEngine;

public class PopupAndroidBackbutton : MonoBehaviour
{
	private Transform _trans;

	private GUIButton _androidBackbutton;

	private void Awake()
	{
		_trans = base.gameObject.transform;
	}

	private void Start()
	{
		if (GameObject.Find("BackButtonWrapper") != null)
		{
			_androidBackbutton = GameObject.Find("BackButtonWrapper").GetComponent<GUIButton>();
		}
		if (_androidBackbutton == null)
		{
			_androidBackbutton = new GameObject("BackButtonWrapper").AddComponent<GUIButton>();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && _trans.childCount >= 2 && !InputFieldMatchVerifier.IsOpen)
		{
			_androidBackbutton.name = "backBtn";
			base.gameObject.BroadcastMessage("OnGUIButtonClicked", _androidBackbutton, SendMessageOptions.DontRequireReceiver);
		}
	}
}
