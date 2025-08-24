using UnityEngine;

public class DreamcatcherMine : Mine
{
	protected override void Start()
	{
		ConfigureObject();
		FollowTransform followTransform = base.gameObject.AddComponent<FollowTransform>();
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position, Vector3.down, out hitInfo))
		{
			followTransform.FollowTransformAtStartPoint(hitInfo.collider.transform, hitInfo.point);
		}
	}

	public override void triggerMine(PlayerController triggeringPlayer)
	{
		if (triggeringPlayer != null)
		{
			triggeringPlayer.OnGetDreamy(effectDuration);
		}
		base.triggerMine(triggeringPlayer);
	}
}
