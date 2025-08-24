using System.Collections.Generic;
using UnityEngine;

public class RoyaleGameEndPopup : MonoBehaviour
{
	[SerializeField]
	private Transform _headMount;

	[SerializeField]
	private TextMesh _endMessage;

	public void Init(List<PlayerLoadout> winningLoadouts)
	{
		DisplayResults(winningLoadouts);
	}

	private void DisplayResults(List<PlayerLoadout> winningLoadouts)
	{
		if (winningLoadouts != null && winningLoadouts[0].pid == GameManager.Instance.localPlayerID)
		{
			_endMessage.text = "#1 Winner!";
		}
		else
		{
			int num = GameManager.Instance.RoyalePlacement(GameManager.Instance.localPlayerID);
			_endMessage.text = "#" + num + " Eliminated. Try Again.";
		}
		InstantiateYourIcon(LoadoutManager.Instance.CurrentLoadout);
	}

	private void InstantiateYourIcon(PlayerLoadout myLoadout)
	{
		Object @object = Resources.Load("Icons/Characters/" + myLoadout.model.name + "/" + myLoadout.skin.name + "_red");
		if (@object != null)
		{
			GameObject gameObject = Object.Instantiate(@object) as GameObject;
			gameObject.transform.parent = _headMount;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.layer = LayerMask.NameToLayer("HUD");
		}
	}
}
