using UnityEngine;

internal class Tutorial1Camera : MonoBehaviour
{
	private float lerpValue = 0.05f;

	public Transform target;

	public Vector3 targetOffset = new Vector3(0f, 0f, 0f);

	private Vector3 prevLookDir = Vector3.zero;

	private void Update()
	{
		if (target != null)
		{
			Vector3 to = target.transform.position + targetOffset - base.transform.position;
			if (prevLookDir == Vector3.zero)
			{
				prevLookDir = to;
			}
			Vector3 view = Vector3.Lerp(prevLookDir, to, lerpValue);
			Quaternion identity = Quaternion.identity;
			identity.SetLookRotation(view, new Vector3(0f, 1f, 0f));
			base.transform.rotation = identity;
			prevLookDir = view;
		}
	}
}
