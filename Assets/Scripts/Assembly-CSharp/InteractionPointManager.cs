using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class InteractionPointManager : MonoBehaviour
{
	public Dictionary<int, InteractionPointBase> InteractionPoints { get; private set; }

	public static InteractionPointManager Instance { get; private set; }

	private void Awake()
	{
		InteractionPoints = new Dictionary<int, InteractionPointBase>();
		Instance = this;
	}

	public void TriggerInteractionPoint(int index, int characterIndex)
	{
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(characterIndex);
		if (playerCharacterManager == null)
		{
			return;
		}
		PlayerController playerController = playerCharacterManager.PlayerController;
		if (!playerController.isRemote)
		{
			InteractionPointBase value;
			if (InteractionPoints.TryGetValue(index, out value))
			{
				value.InteractionPointTriggered(characterIndex);
				Hashtable hashtable = new Hashtable();
				hashtable[(byte)0] = index;
				hashtable[(byte)1] = characterIndex;
				playerController.NetSync.SetAction(43, hashtable);
			}
			else
			{
				Debug.LogError("No interaction point found for index: " + index);
			}
		}
	}

	public void OnRemoteTriggerInteractionPoint(int index, int characterIndex)
	{
		InteractionPointBase value;
		if (InteractionPoints.TryGetValue(index, out value))
		{
			value.InteractionPointTriggered(characterIndex);
		}
		else
		{
			Debug.LogError("No interaction point found for index: " + index);
		}
	}
}
