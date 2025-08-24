using UnityEngine;

public abstract class InteractionPointBase : MonoBehaviour
{
	public int interactionPointIndex;

	private void Start()
	{
		InteractionPointManager.Instance.InteractionPoints.Add(interactionPointIndex, this);
	}

	public abstract void InteractionPointTriggered(int characterIndex);
}
