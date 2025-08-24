using UnityEngine;

public class OpenOtherPopupsAfterPopup : Popup
{
	[SerializeField]
	private GameObject[] _nextPopUps;

	protected override void Start()
	{
		base.Start();
		PutNextPopupsInQueue();
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		switch (b.name)
		{
		case "closeButton":
			OnClose();
			break;
		}
	}

	private void PutNextPopupsInQueue()
	{
		GameObject[] nextPopUps = _nextPopUps;
		foreach (GameObject prefab in nextPopUps)
		{
			MainMenu.Instance.TryCreatePopup(new SetupPopup(prefab));
		}
	}
}
