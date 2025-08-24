using UnityEngine;

public class WASDSatelliteControllerDirector : WASDControllerDirector
{
	public override void UpdateControls(float delta)
	{
		base.UpdateControls(delta);
		base.FireSecondary = Input.GetKey(KeyCode.Tab);
	}
}
