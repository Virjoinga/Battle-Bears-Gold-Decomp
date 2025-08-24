using UnityEngine;

public class ToggleAnimateOtherInteractionPoint : InteractionPointBase
{
	public InteractionAnimationBundle[] animations;

	public float cooldownTime;

	private bool _isInToggledState;

	private float _timeOfLastTrigger;

	public override void InteractionPointTriggered(int owner)
	{
		if (!(Time.realtimeSinceStartup - _timeOfLastTrigger > cooldownTime))
		{
			return;
		}
		_timeOfLastTrigger = Time.realtimeSinceStartup;
		InteractionAnimationBundle[] array = animations;
		foreach (InteractionAnimationBundle interactionAnimationBundle in array)
		{
			if (interactionAnimationBundle.toggledToStart != null)
			{
				if (_isInToggledState)
				{
					interactionAnimationBundle.objectToAnimate[interactionAnimationBundle.toggledToStart.name].speed = 1f;
					interactionAnimationBundle.objectToAnimate[interactionAnimationBundle.toggledToStart.name].time = 0f;
					interactionAnimationBundle.objectToAnimate.Blend(interactionAnimationBundle.toggledToStart.name);
				}
				else
				{
					interactionAnimationBundle.objectToAnimate[interactionAnimationBundle.startToToggled.name].speed = 1f;
					interactionAnimationBundle.objectToAnimate[interactionAnimationBundle.startToToggled.name].time = 0f;
					interactionAnimationBundle.objectToAnimate.Blend(interactionAnimationBundle.startToToggled.name);
				}
			}
			else if (_isInToggledState)
			{
				interactionAnimationBundle.objectToAnimate[interactionAnimationBundle.startToToggled.name].speed = -1f;
				interactionAnimationBundle.objectToAnimate[interactionAnimationBundle.startToToggled.name].time = interactionAnimationBundle.objectToAnimate[interactionAnimationBundle.startToToggled.name].length;
				interactionAnimationBundle.objectToAnimate.Blend(interactionAnimationBundle.startToToggled.name);
			}
			else
			{
				interactionAnimationBundle.objectToAnimate[interactionAnimationBundle.startToToggled.name].speed = 1f;
				interactionAnimationBundle.objectToAnimate[interactionAnimationBundle.startToToggled.name].time = 0f;
				interactionAnimationBundle.objectToAnimate.Blend(interactionAnimationBundle.startToToggled.name);
			}
			if (interactionAnimationBundle.audioSource != null)
			{
				if (interactionAnimationBundle.soundToPlayOnActivation != null)
				{
					interactionAnimationBundle.audioSource.PlayOneShot(interactionAnimationBundle.soundToPlayOnActivation);
				}
				if (_isInToggledState && interactionAnimationBundle.toggledToStartSound != null)
				{
					interactionAnimationBundle.audioSource.PlayOneShot(interactionAnimationBundle.toggledToStartSound);
				}
				if (!_isInToggledState && interactionAnimationBundle.startToToggledSound != null)
				{
					interactionAnimationBundle.audioSource.PlayOneShot(interactionAnimationBundle.startToToggledSound);
				}
			}
		}
		_isInToggledState = !_isInToggledState;
	}
}
