using UnityEngine;

public class SetFFALeaderName : MonoBehaviour
{
	[SerializeField]
	private TextMesh _leaderText;

	private void Start()
	{
		if (Preferences.Instance != null && Preferences.Instance.CurrentGameMode == GameMode.FFA && GameManager.Instance != null)
		{
			PlayerStats fFALeader = GameManager.Instance.GetFFALeader();
			if (fFALeader != null)
			{
				_leaderText.text = fFALeader.greeName + " Wins!";
			}
		}
	}
}
