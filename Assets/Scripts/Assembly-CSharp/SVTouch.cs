using UnityEngine;

public abstract class SVTouch
{
	public int fingerId { get; protected set; }

	public Vector2 position { get; protected set; }

	public Vector2 deltaPosition { get; protected set; }

	public float deltaTime { get; protected set; }

	public int tapCount { get; protected set; }

	public TouchPhase phase { get; protected set; }
}
