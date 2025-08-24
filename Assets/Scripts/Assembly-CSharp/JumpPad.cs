using UnityEngine;

public class JumpPad : MonoBehaviour
{
	public Vector3 jumpPower = new Vector3(0f, 1000f, 0f);

	public AudioClip jumpSound;

	private void OnTriggerEnter(Collider col)
	{
		PlayerController playerController = col.GetComponent(typeof(PlayerController)) as PlayerController;
		if (playerController != null)
		{
			playerController.OnJumpPad(jumpPower);
			if (jumpSound != null)
			{
				base.audio.PlayOneShot(jumpSound, SoundManager.Instance.getEffectsVolume());
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(base.transform.position, base.transform.position + jumpPower / 2f);
	}
}
