using UnityEngine;

public abstract class ControllerDirector : MonoBehaviour
{
	public Vector2 Movement { get; protected set; }

	public bool Jump { get; protected set; }

	public Vector2 Aiming { get; protected set; }

	public bool Focus { get; protected set; }

	public bool Fire { get; protected set; }

	public bool FireSecondary { get; protected set; }

	public bool Reload { get; protected set; }

	public bool Switch { get; protected set; }

	public bool Melee { get; protected set; }

	public bool Special { get; protected set; }

	public bool Pause { get; protected set; }

	public bool Teamspeak { get; protected set; }

	public abstract void UpdateControls(float delta);

	public abstract void UpdateTextValues();

	public void ClearDirectives()
	{
		Movement = Vector2.zero;
		Jump = false;
		Aiming = Vector2.zero;
		Focus = false;
		Fire = false;
		FireSecondary = false;
		Reload = false;
		Switch = false;
		Melee = false;
		Special = false;
		Pause = false;
		Teamspeak = false;
	}
}
