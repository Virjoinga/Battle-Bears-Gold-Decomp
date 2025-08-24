using UnityEngine;

public class StretchToAspect : MonoBehaviour
{
	public float originalAspect;

	private void Start()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = num / originalAspect;
		Vector3 localScale = base.transform.localScale;
		localScale.x *= num2;
		base.transform.localScale = localScale;
	}
}
