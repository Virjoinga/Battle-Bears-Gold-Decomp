using UnityEngine;

public abstract class ControllerPerformer : MonoBehaviour
{
	public PlayerController PlayerController { get; set; }

	public abstract void PerformControls(ControllerDirector director, float delta);
}
