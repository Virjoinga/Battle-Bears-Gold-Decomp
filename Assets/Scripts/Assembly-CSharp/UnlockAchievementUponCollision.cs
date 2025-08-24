using UnityEngine;

public class UnlockAchievementUponCollision : MonoBehaviour
{
	public string achievement = "TUTORIAL_SECRET";

	private void OnCollisionEnter()
	{
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements[achievement]);
	}

	private void OnTriggerEnter()
	{
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements[achievement]);
	}
}
